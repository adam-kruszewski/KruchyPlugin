﻿using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
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

        private static Dictionary<Type, Action<UzupelnianieDokumentacji, ParsowanaJednostka>> metodyAnalizy =
            new Dictionary<Type, Action<UzupelnianieDokumentacji, ParsowanaJednostka>>
        {
            { typeof(Obiekt), (ud, pj) => ud.PrzetworzObiekt(pj as Obiekt) },
            { typeof(Konstruktor), (ud, konstruktor) => ud.PrzetworzKonstruktor(konstruktor as Konstruktor) },
            { typeof(Metoda), (ud, metoda) => ud.PrzetworzMetode(metoda as Metoda) },
            { typeof(Pole), (ud, pole) => ud.PrzetworzPole(pole as Pole) },
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

            var listaParsowanychJednostek = new List<ParsowanaJednostka>();

            var definiowaneObiekty = sparsowane.DefiniowaneObiekty
                .Union(sparsowane.DefiniowaneObiekty.SelectMany( o => DajDefiniowanePodObiekty(o)));

            listaParsowanychJednostek.AddRange(definiowaneObiekty);

            listaParsowanychJednostek.AddRange(definiowaneObiekty.SelectMany(o => o.Konstruktory));

            listaParsowanychJednostek.AddRange(definiowaneObiekty.SelectMany(o => o.Pola));

            listaParsowanychJednostek.AddRange(definiowaneObiekty.SelectMany(o => o.Propertiesy));

            listaParsowanychJednostek.AddRange(definiowaneObiekty.SelectMany(o => o.Metody));

            listaParsowanychJednostek.AddRange(sparsowane.DefiniowaneEnumeracje);

            listaParsowanychJednostek.AddRange(sparsowane.DefiniowaneEnumeracje.SelectMany(o => o.Fields));

            listaParsowanychJednostek = listaParsowanychJednostek.OrderByDescending(o => o.Poczatek.Row).ToList();

            foreach (var parsowanaJednostka in listaParsowanychJednostek)
            {
                if (metodyAnalizy.ContainsKey(parsowanaJednostka.GetType()))
                    metodyAnalizy[parsowanaJednostka.GetType()](this, parsowanaJednostka);
            }
        }

        private IEnumerable<Obiekt> DajDefiniowanePodObiekty(Obiekt o)
        {
            return o.ObiektyWewnetrzne.Union(o.ObiektyWewnetrzne.SelectMany(k => DajDefiniowanePodObiekty(k)));
        }

        private void PrzetworzObiekt(Obiekt obiekt)
        {
            if (obiekt.Documentation == null)
            {
                var wciecie = (obiekt.Poczatek.Column - 1).Spacji();

                var poczatek = $"{wciecie}/// ";
                var builder = new StringBuilder();

                GennerujSummary(poczatek, builder, GenerujSummaryKlasyLubInterfejsu(obiekt));

                DodajOpisParametrowGenerycznych(obiekt.ParametryGeneryczne, poczatek, builder);

                var doWstawienia = builder.ToString();

                var numerLinii = obiekt.Poczatek.Row;

                if (obiekt.Atrybuty.Any())
                    numerLinii = obiekt.Atrybuty.OrderBy(o => o.Poczatek.Row).First().Poczatek.Row;

                solution.AktualnyDokument.InsertInLine(doWstawienia, numerLinii);
            }
        }

        private void DodajOpisParametrowGenerycznych(
            IEnumerable<ParametrGeneryczny> parametryGeneryczne,
            string poczatek,
            StringBuilder builder)
        {
            foreach (var parametrGeneryczny in parametryGeneryczne)
            {
                builder.AppendLine($"{poczatek}<typeparam name=\"{parametrGeneryczny.Nazwa}\">{DajOpisParametruGenerycznego(parametrGeneryczny)}</typeparam>");
            }
        }

        private string DajOpisParametruGenerycznego(ParametrGeneryczny parametr)
        {
            var slowa = parametr.Nazwa.PodzielNaSlowaOdWielkichLiter();

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

        private void PrzetworzKonstruktor(Konstruktor konstruktor)
        {
            if (konstruktor.Documentation == null)
            {
                var wciecie = (konstruktor.Poczatek.Column - 1).Spacji();

                var poczatek = $"{wciecie}/// ";
                var builder = new StringBuilder();

                var konf = Konfiguracja.GetInstance(solution);

                GennerujSummary(poczatek, builder, konf.Dokumentacja().Jezyk.Konstruktor());

                GenerujOpisParametrow(konstruktor.Parametry, poczatek, builder);

                var numerLinii = konstruktor.Poczatek.Row;

                solution.AktualnyDokument.InsertInLine(builder.ToString(), numerLinii);
            }
        }

        private void GenerujOpisParametrow(IEnumerable<Parametr> parametry, string poczatek, StringBuilder builder)
        {
            foreach (var parameter in parametry)
            {
                GenerujDokumentacjeParametru(poczatek, builder, parameter);
            }
        }

        private void GenerujDokumentacjeParametru(string poczatek, StringBuilder builder, Parametr parameter)
        {
            var opis = string.Join(" ", parameter.NazwaParametru.PodzielNaSlowaOdWielkichLiter().Select(o => o.ToLower()));

            opis = opis.ZacznijDuzaLitera();

            builder.AppendLine($"{poczatek}<param name=\"{parameter.NazwaParametru}\">{opis}</param>");
        }

        private void PrzetworzMetode(Metoda metoda)
        {
            if (metoda.Documentation == null)
            {
                var wciecie = (metoda.Poczatek.Column - 1).Spacji();

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

                    DodajOpisParametrowGenerycznych(metoda.ParametryGeneryczne, poczatek, builder);

                    if (metoda.TypZwracany.Nazwa != "void")
                        builder.AppendLine($"{poczatek}<returns>{DajOpisTypuZwracanego(metoda)}</returns>");

                    solution.AktualnyDokument.InsertInLine(builder.ToString(), numerLinii);
                }
            }
        }

        private string DajOpisTypuZwracanego(Metoda metoda)
        {
            if (metoda.TypZwracany.Nazwa == "Task")
                return "Awaitable object";

            var slowaNazwyMetody = metoda.Nazwa.PodzielNaSlowaOdWielkichLiter();

            if (slowaNazwyMetody.First() == "Get")
            {
                var slowaDoBudowy = slowaNazwyMetody.Skip(1).Select(o => o.ToLower());

                var regex = new Regex("Task<[a-zA-Z0-9_]+>");

                if (regex.IsMatch(metoda.TypZwracany.Nazwa))
                {
                    slowaDoBudowy = new[] { "Async" }.Union(slowaDoBudowy);
                }

                return string.Join(" ", slowaDoBudowy).ZacznijDuzaLitera();
            }

            if (slowaNazwyMetody.First() == "Is")
            {
                var slowaDoBudowy = new[] { "if it is" }.Union(slowaNazwyMetody.Skip(1).Select(o => o.ToLower()));

                var regex = new Regex("Task<[a-zA-Z0-9_]+>");

                if (regex.IsMatch(metoda.TypZwracany.Nazwa))
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

        private string DajSummaryMetody(int jezyk, Metoda metoda)
        {
            var slowa = metoda.Nazwa.PodzielNaSlowaOdWielkichLiter().ToList();

            slowa[0] = DajCzasownikOpisuMetody(jezyk, slowa, metoda);

            return string.Join(" ", slowa.Select(o => o.ToLower())).ZacznijDuzaLitera();
        }

        private string DajCzasownikOpisuMetody(int jezyk, List<string> slowa, Metoda metoda)
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

        private static bool PasujeRegexNazwyKlasy(Metoda metoda, Czasownik o)
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

        private void PrzetworzPole(Pole pole)
        {
            if (pole.Documentation == null)
            {
                var nazwaDoPrzetwarzania = pole.Nazwa.TrimStart('_');

                string summary = DajNazwePolaWlasciwosciWgKonfiguracji(nazwaDoPrzetwarzania, pole.Owner, pole.NazwaTypu);

                if (string.IsNullOrEmpty(summary))
                {
                    var slowa = nazwaDoPrzetwarzania
                        .PodzielNaSlowaOdWielkichLiter()
                            .Select(o => o.ToLower())
                                .ToArray();

                    summary = string.Join(" ", slowa).ZacznijDuzaLitera();
                }

                var wciecie = (pole.Poczatek.Column - 1).Spacji();

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

                var wciecie = (property.Poczatek.Column - 1).Spacji();

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

                var wciecie = (enumeracja.Poczatek.Column - 1).Spacji();

                var poczatek = $"{wciecie}/// ";
                var builder = new StringBuilder();

                GennerujSummary(poczatek, builder, summary);

                solution.AktualnyDokument.InsertInLine(builder.ToString(), DajNumerLiniiDoWstawienia(enumeracja, enumeracja.Attributes));

            }
        }

        private string DajNazwePolaWlasciwosciWgKonfiguracji(string nazwa, Obiekt wlasciciel, string nazwaTypu)
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

        private int DajNumerLiniiDoWstawienia(ParsowanaJednostka parsowanaJednostka, IEnumerable<KruchyParserKodu.ParserKodu.Models.Attribute> atrybuty = null)
        {
            var numerLinii = parsowanaJednostka.Poczatek.Row;

            if (atrybuty != null && atrybuty.Any())
                numerLinii = atrybuty.OrderBy(o => o.Poczatek.Row).First().Poczatek.Row;

            return numerLinii;
        }
    }
}