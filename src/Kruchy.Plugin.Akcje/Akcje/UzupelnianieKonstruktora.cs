using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.Utils;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
using KruchyParserKodu.ParserKodu.Models.Instructions;

namespace Kruchy.Plugin.Akcje.Akcje
{
    public class UzupelnianieKonstruktora
    {
        private readonly ISolutionWrapper solution;
        private static Dictionary<string, int> kolejnoscWgTypu =
            PrzygotujKolejnoscWgTypow();

        private static Dictionary<string, int> PrzygotujKolejnoscWgTypow()
        {
            var wynik = new Dictionary<string, int>();
            wynik["service"] = 1;
            wynik["list<service>"] = 2;
            wynik["factory"] = 3;
            wynik["validator"] = 4;
            wynik["list<validator>"] = 5;

            return wynik;
        }

        public UzupelnianieKonstruktora(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Uzupelnij()
        {
            if (solution.AktualnyDokument == null)
                return;

            FileWithCode parsowanyPlik = GetParsedCode();

            var obiekt = SzukajKlasy(parsowanyPlik);

            if (obiekt == null)
            {
                MessageBox.Show(
                    "Nie udało się ustalić klasy do uzupełniania konstruktora");
                return;
            }

            if (obiekt.Constructors.Count > 1)
            {
                MessageBox.Show("Klasa ma więcej niż jeden konstruktor");
                return;
            }
            var konstruktor = obiekt.Constructors.FirstOrDefault();

            var polaDoDodania =
                WyliczPolaDoDodaniaDoKonstruktora(obiekt.Fields, konstruktor);

            if (polaDoDodania.Count > 0)
            {
                var polaReadOnly = obiekt.Fields.Where(
                    o => InicjowaneWKontruktorze(o));

                var polaDoKonstruktoraNadklasy =
                    WyliczPolaPotrzebneDoKonstruktoraNadklasy(konstruktor);

                var nowyKonstruktor =
                    GenerujKonstruktor(
                        polaReadOnly,
                        obiekt.Name,
                        polaDoKonstruktoraNadklasy,
                        konstruktor?.InitializationKeyWord,
                        konstruktor);

                if (konstruktor != null)
                {
                    solution.AktualnyDokument.Remove(
                        konstruktor.StartPosition.Row,
                        1,
                        konstruktor.EndPosition.Row,
                        konstruktor.EndPosition.Column);
                    solution.AktualnyDokument.InsertInLine(
                        nowyKonstruktor, konstruktor.StartPosition.Row);
                }
                else
                {
                    //tu zawsze będzie jakieś pole lub właściwośc
                    //, bo są pola które czekają na dodanie do konstruktora
                    int maksymalnyNumerLiniiPol = obiekt.StartingBrace.Row + 1;

                    if (obiekt.Fields.Any())
                        maksymalnyNumerLiniiPol =
                            obiekt.Fields.Select(o => o.EndPosition.Row).Max();
                    else
                        maksymalnyNumerLiniiPol =
                            obiekt.Properties.Select(o => o.EndPosition.Row).Max();

                    var dodatek = new StringBuilder().AppendLine().ToString();
                    solution.AktualnyDokument.InsertInLine(
                        PrzygotujTekstDoWstawienia(nowyKonstruktor, dodatek),
                        maksymalnyNumerLiniiPol + 1);
                }
                if (Konfiguracja.GetInstance(solution).SortowacZaleznosciSerwisu())
                    PosortujZdefiniowanePola(obiekt.SzukajPolPrivateReadOnly());
            }
        }

        private bool IsSimpleParameterAssignment(
            Instruction instruction,
            IEnumerable<Field> readonlyFields,
            IEnumerable<Parameter> parametry)
        {
            var assignmentInstruction = instruction as AssignmentInstruction;

            if (assignmentInstruction == null)
                return false;


            var regexPattern = $"^(this\\.)*(?<field>\\w+)\\s*=\\s(?<parameter>\\w+);$";

            var regex = new Regex(regexPattern);

            var match = regex.Match(assignmentInstruction.Text);

            if (match.Success)
            {
                var fieldName = match.Groups["field"].Value;

                var parameterName = match.Groups["parameter"].Value;

                if (readonlyFields.Select(o => o.Name).Contains(fieldName) &&
                    parametry.Select(o => o.ParameterName).Contains(parameterName))
                    return true;
            }

            return false;
        }

        private IList<Parameter> WyliczPolaPotrzebneDoKonstruktoraNadklasy(Constructor konstruktor)
        {
            if (konstruktor == null || konstruktor.ParentClassContructorParameters == null)
                return null;
            var p = konstruktor.Parametry
                .Where(o => konstruktor.ParentClassContructorParameters.Contains(o.ParameterName));
            return p.ToList();
        }

        private void PosortujZdefiniowanePola(IEnumerable<Field> list)
        {
            var liniaPierwszego =
                list.OrderBy(o => o.StartPosition.Row).First().StartPosition.Row;

            foreach (var p in list.OrderByDescending(o => o.StartPosition.Row))
                UsunPole(p);

            var builder = new StringBuilder();
            foreach (var p in SortujPola(list))
            {
                var poleBuilder =
                    new PoleBuilder()
                        .ZNazwa(p.Name)
                        .ZNazwaTypu(p.TypeName)
                        .DodajModyfikatorem("private")
                        .DodajModyfikatorem("readonly");

                builder.Append(poleBuilder.Build(StaleDlaKodu.WciecieDlaMetody));
            }

            solution.AktualnyDokument.InsertInLine(builder.ToString(), liniaPierwszego);
        }

        private void UsunPole(Field p)
        {
            solution.AktualnyDokument.Remove(
                p.StartPosition.Row,
                //p.Poczatek.Kolumna,
                1,
                p.EndPosition.Row,
                p.EndPosition.Column);

            //if (p.Poczatek.Wiersz != p.Koniec.Wiersz)
            solution.AktualnyDokument.RemoveLine(p.EndPosition.Row);
        }

        private string PrzygotujTekstDoWstawienia(
            string nowyKonstruktor,
            string dodatek)
        {
            var builder = new StringBuilder();
            return
                builder
                    .AppendLine()
                    .Append(nowyKonstruktor)
                    .Append(dodatek)
                    .ToString();
        }

        private DefinedItem SzukajKlasy(FileWithCode parsowanyPlik)
        {
            var klasy =
                parsowanyPlik
                    .DefinedItems
                        .Where(o => o.KindOfItem == KindOfItem.Class);

            if (klasy.Count() == 1)
                return klasy.First();

            return
                parsowanyPlik
                    .FindClassByLineNumber(
                        solution.AktualnyDokument.GetCursorLineNumber());
        }

        private FileWithCode GetParsedCode()
        {
            var kod = solution.AktualnyDokument.GetContent();

            var parsowanyPlik = Parser.Parsuj(kod);
            return parsowanyPlik;
        }

        private string GenerujKonstruktor(
            IEnumerable<Field> pola,
            string nazwaKlasy,
            IEnumerable<Parameter> parametryDlaKonstruktoraNadklasy,
            string slowoKluczowe,
            Constructor constructor)
        {
            var builder = new MetodaBuilder();
            builder.JedenParametrWLinii(true);
            builder.ZNazwa(nazwaKlasy);
            builder.ZTypemZwracanym("");
            builder.DodajModyfikator("public");

            foreach (var pole in SortujPola(pola))
            {
                var nazwaParametru = pole.Name;
                if (nazwaParametru.StartsWith("_"))
                    nazwaParametru = pole.Name.Substring(1);

                builder.DodajParametr(pole.TypeName, nazwaParametru);

                var napisThisJesliTrzeba = "this.";
                if (pole.Name != nazwaParametru)
                    napisThisJesliTrzeba = "";

                builder.DodajLinie(napisThisJesliTrzeba + pole.Name + " = " + nazwaParametru + ";");
            }

            if (constructor != null)
                AddOtherInstructionsFromOriginalConstructor(
                    constructor,
                    pola,
                    builder);

            if (parametryDlaKonstruktoraNadklasy != null)
            {
                foreach (var parametrDlaNadklasy in parametryDlaKonstruktoraNadklasy)
                {
                    builder.DodajParametr(parametrDlaNadklasy.TypeName, parametrDlaNadklasy.ParameterName);
                }

                builder.DodajInicjalizacjeKonstruktora(
                    slowoKluczowe,
                    parametryDlaKonstruktoraNadklasy.Select(o => o.ParameterName));
            }

            return builder.Build(StaleDlaKodu.WciecieDlaMetody).TrimEnd();
        }

        private void AddOtherInstructionsFromOriginalConstructor(
            Constructor constructor,
            IEnumerable<Field> fields,
            MetodaBuilder builder)
        {
            Instruction previousOtherInstruction = null;

            foreach (var originalInstruction in constructor.Instructions)
            {
                if (!IsSimpleParameterAssignment(originalInstruction, fields, constructor.Parametry))
                {
                    if (previousOtherInstruction == null)
                    {
                        builder.DodajLinie("");
                    }
                    else
                    {
                        var difference =
                            originalInstruction.StartPosition.Row - previousOtherInstruction.StartPosition.Row;

                        if (difference > 1)
                            builder.DodajLinie("");

                    }

                    builder.DodajLinie(originalInstruction.Text);

                    previousOtherInstruction = originalInstruction;
                }
            }
        }

        private IEnumerable<Instruction> GetOtherInstructionsFromOriginalConstructor(
            Constructor constructor, IEnumerable<Field> readonlyFields)
        {
            return constructor.Instructions
                .Where(o => !IsSimpleParameterAssignment(o, readonlyFields, constructor.Parametry));
        }

        private IEnumerable<Field> SortujPola(IEnumerable<Field> pola)
        {
            var konf = Konfiguracja.GetInstance(solution);

            if (!konf.SortowacZaleznosciSerwisu())
                return pola;
            return
                pola
                    .OrderBy(o => DajKolejnoscWgRodzajuPola(o))
                        .ThenBy(o => o.Name);
        }

        private int DajKolejnoscWgRodzajuPola(Field pole)
        {
            var rodzajPola = DajRodzajPola(pole);
            if (kolejnoscWgTypu.ContainsKey(rodzajPola))
                return kolejnoscWgTypu[rodzajPola];
            return 9999;
        }

        private string DajRodzajPola(Field pole)
        {
            var lowerNazwaTypu = pole.TypeName.ToLower();

            if (pole.TypeName.Contains("<"))
            {
                //generic - domyślamy się, że lista
                var t = pole.TypeName.Substring(pole.TypeName.IndexOf("<") + 1);
                t = t.Substring(0, t.IndexOf(">"));
                return "list<" + DajRodzajPolaZNazwyTypu(t) + ">";
            }
            else
            {
                return DajRodzajPolaZNazwyTypu(pole.TypeName);
            }
        }

        private string DajRodzajPolaZNazwyTypu(string nazwaTypu)
        {
            for (int i = nazwaTypu.Length - 1; i >= 0; i--)
            {
                if (char.IsUpper(nazwaTypu[i]))
                    return nazwaTypu.Substring(i).ToLower();
            }
            return nazwaTypu;
        }

        private List<Field> WyliczPolaDoDodaniaDoKonstruktora(
            IList<Field> pola,
            Constructor konstruktor)
        {
            var wynik = new List<Field>();
            var polaReadOnly =
                pola.Where(o => InicjowaneWKontruktorze(o));
            foreach (var pole in polaReadOnly)
            {
                if (!KonstruktorMaWParametrzePole(konstruktor, pole))
                    wynik.Add(pole);
            }
            return wynik;
        }

        private static bool InicjowaneWKontruktorze(Field pole)
        {
            var stringiModyfikatorow = pole.Modifiers.Select(m => m.Name);
            return
                stringiModyfikatorow.Contains("readonly")
                && !stringiModyfikatorow.Contains("static");
        }

        private bool KonstruktorMaWParametrzePole(
            Constructor konstruktor,
            Field pole)
        {
            if (konstruktor == null)
                return false;

            return konstruktor.Parametry
                .Any(o => o.ParameterName == pole.Name
                    && o.TypeName == pole.TypeName);
        }
    }
}
