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

            Plik parsowanyPlik = GetParsedCode();

            var obiekt = SzukajKlasy(parsowanyPlik);

            if (obiekt == null)
            {
                MessageBox.Show(
                    "Nie udało się ustalić klasy do uzupełniania konstruktora");
                return;
            }

            if (obiekt.Konstruktory.Count > 1)
            {
                MessageBox.Show("Klasa ma więcej niż jeden konstruktor");
                return;
            }
            var konstruktor = obiekt.Konstruktory.FirstOrDefault();

            var polaDoDodania =
                WyliczPolaDoDodaniaDoKonstruktora(obiekt.Pola, konstruktor);

            if (polaDoDodania.Count > 0)
            {
                var polaReadOnly = obiekt.Pola.Where(
                    o => InicjowaneWKontruktorze(o));

                var polaDoKonstruktoraNadklasy =
                    WyliczPolaPotrzebneDoKonstruktoraNadklasy(konstruktor);

                var nowyKonstruktor =
                    GenerujKonstruktor(
                        polaReadOnly,
                        obiekt.Name,
                        polaDoKonstruktoraNadklasy,
                        konstruktor?.SlowoKluczoweInicjalizacji,
                        konstruktor);

                if (konstruktor != null)
                {
                    solution.AktualnyDokument.Remove(
                        konstruktor.Poczatek.Wiersz,
                        1,
                        konstruktor.Koniec.Wiersz,
                        konstruktor.Koniec.Kolumna);
                    solution.AktualnyDokument.InsertInLine(
                        nowyKonstruktor, konstruktor.Poczatek.Wiersz);
                }
                else
                {
                    //tu zawsze będzie jakieś pole lub właściwośc
                    //, bo są pola które czekają na dodanie do konstruktora
                    int maksymalnyNumerLiniiPol = obiekt.StartingBrace.Wiersz + 1;

                    if (obiekt.Pola.Any())
                        maksymalnyNumerLiniiPol =
                            obiekt.Pola.Select(o => o.Koniec.Wiersz).Max();
                    else
                        maksymalnyNumerLiniiPol =
                            obiekt.Propertiesy.Select(o => o.Koniec.Wiersz).Max();

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
            IEnumerable<Pole> readonlyFields,
            IEnumerable<Parametr> parametry)
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

                if (readonlyFields.Select(o => o.Nazwa).Contains(fieldName) &&
                    parametry.Select(o => o.NazwaParametru).Contains(parameterName))
                    return true;
            }

            return false;
        }

        private IList<Parametr> WyliczPolaPotrzebneDoKonstruktoraNadklasy(Konstruktor konstruktor)
        {
            if (konstruktor == null || konstruktor.ParametryKonstruktoraZNadKlasy == null)
                return null;
            var p = konstruktor.Parametry
                .Where(o => konstruktor.ParametryKonstruktoraZNadKlasy.Contains(o.NazwaParametru));
            return p.ToList();
        }

        private void PosortujZdefiniowanePola(IEnumerable<Pole> list)
        {
            var liniaPierwszego =
                list.OrderBy(o => o.Poczatek.Wiersz).First().Poczatek.Wiersz;

            foreach (var p in list.OrderByDescending(o => o.Poczatek.Wiersz))
                UsunPole(p);

            var builder = new StringBuilder();
            foreach (var p in SortujPola(list))
            {
                var poleBuilder =
                    new PoleBuilder()
                        .ZNazwa(p.Nazwa)
                        .ZNazwaTypu(p.NazwaTypu)
                        .DodajModyfikatorem("private")
                        .DodajModyfikatorem("readonly");

                builder.Append(poleBuilder.Build(StaleDlaKodu.WciecieDlaMetody));
            }

            solution.AktualnyDokument.InsertInLine(builder.ToString(), liniaPierwszego);
        }

        private void UsunPole(Pole p)
        {
            solution.AktualnyDokument.Remove(
                p.Poczatek.Wiersz,
                //p.Poczatek.Kolumna,
                1,
                p.Koniec.Wiersz,
                p.Koniec.Kolumna);

            //if (p.Poczatek.Wiersz != p.Koniec.Wiersz)
            solution.AktualnyDokument.RemoveLine(p.Koniec.Wiersz);
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

        private Obiekt SzukajKlasy(Plik parsowanyPlik)
        {
            var klasy =
                parsowanyPlik
                    .DefiniowaneObiekty
                        .Where(o => o.Rodzaj == RodzajObiektu.Klasa);

            if (klasy.Count() == 1)
                return klasy.First();

            return
                parsowanyPlik
                    .SzukajKlasyWLinii(
                        solution.AktualnyDokument.GetCursorLineNumber());
        }

        private Plik GetParsedCode()
        {
            var kod = solution.AktualnyDokument.GetContent();

            var parsowanyPlik = Parser.Parsuj(kod);
            return parsowanyPlik;
        }

        private string GenerujKonstruktor(
            IEnumerable<Pole> pola,
            string nazwaKlasy,
            IEnumerable<Parametr> parametryDlaKonstruktoraNadklasy,
            string slowoKluczowe,
            Konstruktor constructor)
        {
            var builder = new MetodaBuilder();
            builder.JedenParametrWLinii(true);
            builder.ZNazwa(nazwaKlasy);
            builder.ZTypemZwracanym("");
            builder.DodajModyfikator("public");

            foreach (var pole in SortujPola(pola))
            {
                var nazwaParametru = pole.Nazwa;
                if (nazwaParametru.StartsWith("_"))
                    nazwaParametru = pole.Nazwa.Substring(1);

                builder.DodajParametr(pole.NazwaTypu, nazwaParametru);

                var napisThisJesliTrzeba = "this.";
                if (pole.Nazwa != nazwaParametru)
                    napisThisJesliTrzeba = "";

                builder.DodajLinie(napisThisJesliTrzeba + pole.Nazwa + " = " + nazwaParametru + ";");
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
                    builder.DodajParametr(parametrDlaNadklasy.NazwaTypu, parametrDlaNadklasy.NazwaParametru);
                }

                builder.DodajInicjalizacjeKonstruktora(
                    slowoKluczowe,
                    parametryDlaKonstruktoraNadklasy.Select(o => o.NazwaParametru));
            }

            return builder.Build(StaleDlaKodu.WciecieDlaMetody).TrimEnd();
        }

        private void AddOtherInstructionsFromOriginalConstructor(
            Konstruktor constructor,
            IEnumerable<Pole> fields,
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
                            originalInstruction.Poczatek.Wiersz - previousOtherInstruction.Poczatek.Wiersz;

                        if (difference > 1)
                            builder.DodajLinie("");

                    }

                    builder.DodajLinie(originalInstruction.Text);

                    previousOtherInstruction = originalInstruction;
                }
            }
        }

        private IEnumerable<Instruction> GetOtherInstructionsFromOriginalConstructor(
            Konstruktor constructor, IEnumerable<Pole> readonlyFields)
        {
            return constructor.Instructions
                .Where(o => !IsSimpleParameterAssignment(o, readonlyFields, constructor.Parametry));
        }

        private IEnumerable<Pole> SortujPola(IEnumerable<Pole> pola)
        {
            var konf = Konfiguracja.GetInstance(solution);

            if (!konf.SortowacZaleznosciSerwisu())
                return pola;
            return
                pola
                    .OrderBy(o => DajKolejnoscWgRodzajuPola(o))
                        .ThenBy(o => o.Nazwa);
        }

        private int DajKolejnoscWgRodzajuPola(Pole pole)
        {
            var rodzajPola = DajRodzajPola(pole);
            if (kolejnoscWgTypu.ContainsKey(rodzajPola))
                return kolejnoscWgTypu[rodzajPola];
            return 9999;
        }

        private string DajRodzajPola(Pole pole)
        {
            var lowerNazwaTypu = pole.NazwaTypu.ToLower();

            if (pole.NazwaTypu.Contains("<"))
            {
                //generic - domyślamy się, że lista
                var t = pole.NazwaTypu.Substring(pole.NazwaTypu.IndexOf("<") + 1);
                t = t.Substring(0, t.IndexOf(">"));
                return "list<" + DajRodzajPolaZNazwyTypu(t) + ">";
            }
            else
            {
                return DajRodzajPolaZNazwyTypu(pole.NazwaTypu);
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

        private List<Pole> WyliczPolaDoDodaniaDoKonstruktora(
            IList<Pole> pola,
            Konstruktor konstruktor)
        {
            var wynik = new List<Pole>();
            var polaReadOnly =
                pola.Where(o => InicjowaneWKontruktorze(o));
            foreach (var pole in polaReadOnly)
            {
                if (!KonstruktorMaWParametrzePole(konstruktor, pole))
                    wynik.Add(pole);
            }
            return wynik;
        }

        private static bool InicjowaneWKontruktorze(Pole pole)
        {
            var stringiModyfikatorow = pole.Modyfikatory.Select(m => m.Nazwa);
            return
                stringiModyfikatorow.Contains("readonly")
                && !stringiModyfikatorow.Contains("static");
        }

        private bool KonstruktorMaWParametrzePole(
            Konstruktor konstruktor,
            Pole pole)
        {
            if (konstruktor == null)
                return false;

            return konstruktor.Parametry
                .Any(o => o.NazwaParametru == pole.Nazwa
                    && o.NazwaTypu == pole.NazwaTypu);
        }
    }
}
