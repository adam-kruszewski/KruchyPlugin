using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Interfaces;
using KruchyParserKodu.ParserKodu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Kruchy.Plugin.Akcje.Akcje
{
    public class UzupelnianieDokumentacji
    {
        private readonly ISolutionWrapper solution;

        private static Dictionary<Type, Action<UzupelnianieDokumentacji, ParsedUnit>> metodyAnalizy =
            new Dictionary<Type, Action<UzupelnianieDokumentacji, ParsedUnit>>
        {
            { typeof(DefinedItem), (ud, pj) => ud.PrzetworzObiekt(pj as DefinedItem) },
            { typeof(Constructor), (ud, konstruktor) => ud.PrzetworzKonstruktor(konstruktor as Constructor) },
            { typeof(Method), (ud, metoda) => ud.PrzetworzMetode(metoda as Method) },
            { typeof(Field), (ud, pole) => ud.PrzetworzPole(pole as Field) },
            { typeof(Property), (ud, wlasciwosc) => ud.PrzetworzWlasciwosc(wlasciwosc as Property) },
            { typeof(Enumeration), (ud, enumeracja) => ud.PrzetworzEnumeracje(enumeracja as Enumeration)}
        };

        public UzupelnianieDokumentacji(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Uzupelnij()
        {
            var sparsowane = solution.ParsujZawartoscAktualnegoDokumetu();

            var listaParsowanychJednostek = new List<ParsedUnit>();

            var definiowaneObiekty = sparsowane.DefinedItems
                .Union(sparsowane.DefinedItems.SelectMany( o => DajDefiniowanePodObiekty(o)));

            listaParsowanychJednostek.AddRange(definiowaneObiekty);

            listaParsowanychJednostek.AddRange(definiowaneObiekty.SelectMany(o => o.Constructors));

            listaParsowanychJednostek.AddRange(definiowaneObiekty.SelectMany(o => o.Fields));

            listaParsowanychJednostek.AddRange(definiowaneObiekty.SelectMany(o => o.Properties));

            listaParsowanychJednostek.AddRange(definiowaneObiekty.SelectMany(o => o.Methods));

            listaParsowanychJednostek.AddRange(sparsowane.DefinedEnumerations);

            listaParsowanychJednostek.AddRange(sparsowane.DefinedEnumerations.SelectMany(o => o.Fields));

            listaParsowanychJednostek = listaParsowanychJednostek.OrderByDescending(o => o.StartPosition.Row).ToList();

            foreach (var parsowanaJednostka in listaParsowanychJednostek)
            {
                if (metodyAnalizy.ContainsKey(parsowanaJednostka.GetType()))
                    metodyAnalizy[parsowanaJednostka.GetType()](this, parsowanaJednostka);
            }
        }

        private IEnumerable<DefinedItem> DajDefiniowanePodObiekty(DefinedItem o)
        {
            return o.InternalDefinedItems.Union(o.InternalDefinedItems.SelectMany(k => DajDefiniowanePodObiekty(k)));
        }

        private void PrzetworzObiekt(DefinedItem obiekt)
        {
            if (obiekt.Documentation == null)
            {
                var wciecie = (obiekt.StartPosition.Column - 1).Spacji();

                var poczatek = $"{wciecie}/// ";
                var builder = new StringBuilder();

                GennerujSummary(poczatek, builder, GenerujSummaryKlasyLubInterfejsu(obiekt));

                DodajOpisParametrowGenerycznych(obiekt.GenericParameters, poczatek, builder);

                var doWstawienia = builder.ToString();

                var numerLinii = obiekt.StartPosition.Row;

                if (obiekt.Attributes.Any())
                    numerLinii = obiekt.Attributes.OrderBy(o => o.StartPosition.Row).First().StartPosition.Row;

                solution.AktualnyDokument.InsertInLine(doWstawienia, numerLinii);
            }
        }

        private void DodajOpisParametrowGenerycznych(
            IEnumerable<GenericParameter> parametryGeneryczne,
            string poczatek,
            StringBuilder builder)
        {
            foreach (var parametrGeneryczny in parametryGeneryczne)
            {
                builder.AppendLine($"{poczatek}<typeparam name=\"{parametrGeneryczny.Name}\">{DajOpisParametruGenerycznego(parametrGeneryczny)}</typeparam>");
            }
        }

        private string DajOpisParametruGenerycznego(GenericParameter parametr)
        {
            var slowa = parametr.Name.PodzielNaSlowaOdWielkichLiter();

            if (slowa.Count() == 1 && slowa.First().Length == 1)
                return "";


            if (slowa.First() == "T")
                slowa = slowa.Skip(1);

            return string.Join(" ", slowa.Select(o => o.ToLower())).ZacznijDuzaLitera();
        }

        private void GennerujSummary(string poczatek, StringBuilder builder, string summary)
        {
            builder.AppendLine($"{poczatek}<summary>");

            builder.AppendLine($"{poczatek}{summary}");

            builder.AppendLine($"{poczatek}</summary>");
        }

        private string GenerujSummaryKlasyLubInterfejsu(IWithName obiekt)
        {
            var slowa = obiekt.Name.PodzielNaSlowaOdWielkichLiter();

            if (slowa.First() == "I")
                slowa = slowa.Skip(1);

            var wynikBuilder = new StringBuilder();

            foreach (var slowo in slowa)
                wynikBuilder.Append(slowo.ToLower() + " ");

            var wynik = wynikBuilder.ToString().TrimEnd();

            return char.ToUpper(wynik[0]) + wynik.Substring(1);
        }

        private void PrzetworzKonstruktor(Constructor konstruktor)
        {
            if (konstruktor.Documentation == null)
            {
                var wciecie = (konstruktor.StartPosition.Column - 1).Spacji();

                var poczatek = $"{wciecie}/// ";
                var builder = new StringBuilder();

                var konf = Konfiguracja.GetInstance(solution);

                GennerujSummary(poczatek, builder, konf.Dokumentacja().Jezyk.Konstruktor());

                GenerujOpisParametrow(konstruktor.Parametry, poczatek, builder);

                var numerLinii = konstruktor.StartPosition.Row;

                solution.AktualnyDokument.InsertInLine(builder.ToString(), numerLinii);
            }
        }

        private void GenerujOpisParametrow(IEnumerable<Parameter> parametry, string poczatek, StringBuilder builder)
        {
            foreach (var parameter in parametry)
            {
                GenerujDokumentacjeParametru(poczatek, builder, parameter);
            }
        }

        private void GenerujDokumentacjeParametru(string poczatek, StringBuilder builder, Parameter parameter)
        {
            var opis = string.Join(" ", parameter.ParameterName.PodzielNaSlowaOdWielkichLiter().Select(o => o.ToLower()));

            opis = opis.ZacznijDuzaLitera();

            builder.AppendLine($"{poczatek}<param name=\"{parameter.ParameterName}\">{opis}</param>");
        }

        private void PrzetworzMetode(Method metoda)
        {
            if (metoda.Documentation == null)
            {
                var wciecie = (metoda.StartPosition.Column - 1).Spacji();

                var poczatek = $"{wciecie}/// ";
                var builder = new StringBuilder();

                var konf = Konfiguracja.GetInstance(solution);

                var numerLinii = DajNumerLiniiDoWstawienia(metoda, metoda.Atrybuty);

                if (metoda.ZawieraModyfikator("override"))
                {
                    WstawInheritDoc(poczatek, numerLinii);
                }
                else
                {
                    GennerujSummary(poczatek, builder, DajSummaryMetody(konf.Dokumentacja().Jezyk, metoda));

                    GenerujOpisParametrow(metoda.Parametry, poczatek, builder);

                    DodajOpisParametrowGenerycznych(metoda.GenericParameters, poczatek, builder);

                    if (metoda.ReturnType.Nazwa != "void")
                        builder.AppendLine($"{poczatek}<returns>{DajOpisTypuZwracanego(metoda)}</returns>");

                    solution.AktualnyDokument.InsertInLine(builder.ToString(), numerLinii);
                }
            }
        }

        private string DajOpisTypuZwracanego(Method metoda)
        {
            if (metoda.ReturnType.Nazwa == "Task")
                return "Awaitable object";

            var slowaNazwyMetody = metoda.Name.PodzielNaSlowaOdWielkichLiter();

            if (slowaNazwyMetody.First() == "Get")
            {
                var slowaDoBudowy = slowaNazwyMetody.Skip(1).Select(o => o.ToLower());

                var regex = new Regex("Task<[a-zA-Z0-9_]+>");

                if (regex.IsMatch(metoda.ReturnType.Nazwa))
                {
                    slowaDoBudowy = new[] { "Async" }.Union(slowaDoBudowy);
                }

                return string.Join(" ", slowaDoBudowy).ZacznijDuzaLitera();
            }

            if (slowaNazwyMetody.First() == "Is")
            {
                var slowaDoBudowy = new[] { "if it is" }.Union(slowaNazwyMetody.Skip(1).Select(o => o.ToLower()));

                var regex = new Regex("Task<[a-zA-Z0-9_]+>");

                if (regex.IsMatch(metoda.ReturnType.Nazwa))
                {
                    slowaDoBudowy = new[] { "Async" }.Union(slowaDoBudowy);
                }

                return string.Join(" ", slowaDoBudowy).ZacznijDuzaLitera();
            }

            return "";
        }

        private void WstawInheritDoc(string poczatek, int numerLinii)
        {
            solution.AktualnyDokument.InsertInLine($"{poczatek}<inheritdoc/>\n", numerLinii);
        }

        private string DajSummaryMetody(int jezyk, Method metoda)
        {
            var slowa = metoda.Name.PodzielNaSlowaOdWielkichLiter().ToList();

            slowa[0] = DajCzasownikOpisuMetody(jezyk, slowa, metoda);

            return string.Join(" ", slowa.Select(o => o.ToLower())).ZacznijDuzaLitera();
        }

        private string DajCzasownikOpisuMetody(int jezyk, List<string> slowa, Method metoda)
        {
            var konfiguracja = Konfiguracja.GetInstance(solution);

            var czasownik = slowa.First();

            var konfiguracjeCzasownika =
                konfiguracja.Dokumentacja().Czasowniki
                .Where(o => o.Wartosc == slowa.First());

            if (konfiguracjeCzasownika.Count() == 1)
                return konfiguracjeCzasownika.Single().WyjsciowaWartosc;

            if (konfiguracjeCzasownika.Count() > 1)
            {
                var konfiguracjaCzasownikaZRegex =
                    konfiguracjeCzasownika.SingleOrDefault(o => PasujeRegexNazwyKlasy(metoda, o));

                if (konfiguracjaCzasownikaZRegex != null)
                    return konfiguracjaCzasownikaZRegex.WyjsciowaWartosc;

                var konfiguracjaCzasownikaBezRegex =
                    konfiguracjeCzasownika.SingleOrDefault(o => o.RegexNazwyKlasy == null);

                if (konfiguracjaCzasownikaBezRegex != null)
                    return konfiguracjaCzasownikaBezRegex.WyjsciowaWartosc;
            }

            return jezyk.PrzygotujCzasownik(slowa[0]);
        }

        private static bool PasujeRegexNazwyKlasy(Method metoda, Czasownik o)
        {
            return
                o.RegexNazwyKlasy != null &&
                PasujeRegex(metoda.Owner.Name, o.RegexNazwyKlasy);
        }

        private static bool PasujeRegex(string wartosc, string regex)
        {
            if (string.IsNullOrEmpty(wartosc) || string.IsNullOrEmpty(regex))
                return false;

            return new Regex(regex).IsMatch(wartosc);
        }

        private void PrzetworzPole(Field pole)
        {
            if (pole.Documentation == null)
            {
                var nazwaDoPrzetwarzania = pole.Name.TrimStart('_');

                string summary = DajNazwePolaWlasciwosciWgKonfiguracji(nazwaDoPrzetwarzania, pole.Owner, pole.TypeName);

                if (string.IsNullOrEmpty(summary))
                {
                    var slowa = nazwaDoPrzetwarzania
                        .PodzielNaSlowaOdWielkichLiter()
                            .Select(o => o.ToLower())
                                .ToArray();

                    summary = string.Join(" ", slowa).ZacznijDuzaLitera();
                }

                var wciecie = (pole.StartPosition.Column - 1).Spacji();

                var poczatek = $"{wciecie}/// ";
                var builder = new StringBuilder();

                GennerujSummary(poczatek, builder, summary);

                solution.AktualnyDokument.InsertInLine(builder.ToString(), DajNumerLiniiDoWstawienia(pole));
            }
        }

        private void PrzetworzWlasciwosc(Property property)
        {
            if (property.Documentation == null)
            {
                var summary = DajNazwePolaWlasciwosciWgKonfiguracji(property.Nazwa, property.Owner, property.NazwaTypu);

                if (string.IsNullOrEmpty(summary))
                    summary = string.Join(" ", property.Nazwa.PodzielNaSlowaOdWielkichLiter().Select(o => o.ToLower())).ZacznijDuzaLitera();

                var wciecie = (property.StartPosition.Column - 1).Spacji();

                var poczatek = $"{wciecie}/// ";
                var builder = new StringBuilder();

                GennerujSummary(poczatek, builder, summary);

                solution.AktualnyDokument.InsertInLine(builder.ToString(), DajNumerLiniiDoWstawienia(property, property.Atrybuty));
            }
        }

        private void PrzetworzEnumeracje(Enumeration enumeracja)
        {
            if (enumeracja.Documentation == null)
            {
                var summary = GenerujSummaryKlasyLubInterfejsu(enumeracja);

                var wciecie = (enumeracja.StartPosition.Column - 1).Spacji();

                var poczatek = $"{wciecie}/// ";
                var builder = new StringBuilder();

                GennerujSummary(poczatek, builder, summary);

                solution.AktualnyDokument.InsertInLine(builder.ToString(), DajNumerLiniiDoWstawienia(enumeracja, enumeracja.Attributes));

            }
        }

        private string DajNazwePolaWlasciwosciWgKonfiguracji(string nazwa, DefinedItem wlasciciel, string nazwaTypu)
        {
            string summary = null;

            var konfiguracja = Konfiguracja.GetInstance(solution).Dokumentacja();

            var konfiguracjaDokumentacji = konfiguracja.WlasciwosciPola;

            var konfiguracjaDokumentacjiDlaNazwy = konfiguracjaDokumentacji.Where(o => o.Wartosc == nazwa);

            var definicjaZRegexemNazwyKlasy =
                konfiguracjaDokumentacjiDlaNazwy
                    .Where(o => !string.IsNullOrEmpty(o.RegexNazwyKlasy))
                    .SingleOrDefault(o => PasujeRegex(wlasciciel.Name, o.RegexNazwyKlasy));

            if (definicjaZRegexemNazwyKlasy != null)
            {
                summary = definicjaZRegexemNazwyKlasy.WyjsciowaWartosc;
            }

            if (string.IsNullOrEmpty(summary))
            {
                var definicjaBezRegexaNazwyKlasy =
                    konfiguracjaDokumentacjiDlaNazwy
                        .SingleOrDefault(o => string.IsNullOrEmpty(o.RegexNazwyKlasy));

                summary = definicjaBezRegexaNazwyKlasy?.WyjsciowaWartosc;
            }

            if (string.IsNullOrEmpty(summary))
            {
                var definicjaRegexuNazwyTypu =
                    konfiguracjaDokumentacji
                        .SingleOrDefault(o => PasujeRegex(nazwaTypu, o.RegexTypWlasciwosciPola));

                summary = definicjaRegexuNazwyTypu?.WyjsciowaWartosc;
            }

            return summary;
        }

        private int DajNumerLiniiDoWstawienia(ParsedUnit parsowanaJednostka, IEnumerable<KruchyParserKodu.ParserKodu.Models.Attribute> atrybuty = null)
        {
            var numerLinii = parsowanaJednostka.StartPosition.Row;

            if (atrybuty != null && atrybuty.Any())
                numerLinii = atrybuty.OrderBy(o => o.StartPosition.Row).First().StartPosition.Row;

            return numerLinii;
        }
    }
}