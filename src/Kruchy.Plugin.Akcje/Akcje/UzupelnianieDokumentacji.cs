using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            { typeof(Property), (ud, wlasciwosc) => ud.PrzetworzWlasciwosc(wlasciwosc as Property) }
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

            listaParsowanychJednostek.AddRange(sparsowane.DefiniowaneObiekty);

            listaParsowanychJednostek.AddRange(sparsowane.DefiniowaneObiekty.SelectMany(o => o.Konstruktory));

            listaParsowanychJednostek.AddRange(sparsowane.DefiniowaneObiekty.SelectMany(o => o.Pola));

            listaParsowanychJednostek.AddRange(sparsowane.DefiniowaneObiekty.SelectMany(o => o.Propertiesy));

            listaParsowanychJednostek.AddRange(sparsowane.DefiniowaneObiekty.SelectMany(o => o.Metody));

            listaParsowanychJednostek = listaParsowanychJednostek.OrderByDescending(o => o.Poczatek.Wiersz).ToList();

            foreach (var parsowanaJednostka in listaParsowanychJednostek)
            {
                if (metodyAnalizy.ContainsKey(parsowanaJednostka.GetType()))
                    metodyAnalizy[parsowanaJednostka.GetType()](this, parsowanaJednostka);
            }
        }

        private void PrzetworzObiekt(Obiekt obiekt)
        {
            if (obiekt.Dokumentacja == null)
            {
                var wciecie = (obiekt.Poczatek.Kolumna - 1).Spacji();

                var poczatek = $"{wciecie}/// ";
                var builder = new StringBuilder();

                GennerujSummary(poczatek, builder, GenerujSummaryKlasyLubInterfejsu(obiekt));

                DodajOpisParametrowGenerycznych(obiekt.ParametryGeneryczne, poczatek, builder);

                var doWstawienia = builder.ToString();

                var numerLinii = obiekt.Poczatek.Wiersz;

                if (obiekt.Atrybuty.Any())
                    numerLinii = obiekt.Atrybuty.OrderBy(o => o.Poczatek.Wiersz).First().Poczatek.Wiersz;

                solution.AktualnyDokument.WstawWLinii(doWstawienia, numerLinii);
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

        private string GenerujSummaryKlasyLubInterfejsu(Obiekt obiekt)
        {
            var slowa = obiekt.Nazwa.PodzielNaSlowaOdWielkichLiter();

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
            if (konstruktor.Dokumentacja == null)
            {
                var wciecie = (konstruktor.Poczatek.Kolumna - 1).Spacji();

                var poczatek = $"{wciecie}/// ";
                var builder = new StringBuilder();

                var konf = Konfiguracja.GetInstance(solution);

                GennerujSummary(poczatek, builder, konf.Dokumentacja().Jezyk.Konstruktor());

                GenerujOpisParametrow(konstruktor.Parametry, poczatek, builder);

                var numerLinii = konstruktor.Poczatek.Wiersz;

                solution.AktualnyDokument.WstawWLinii(builder.ToString(), numerLinii);
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
            if (metoda.Dokumentacja == null)
            {
                var wciecie = (metoda.Poczatek.Kolumna - 1).Spacji();

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
                    GennerujSummary(poczatek, builder, DajSummaryMetody(metoda.Nazwa, konf.Dokumentacja().Jezyk));

                    GenerujOpisParametrow(metoda.Parametry, poczatek, builder);

                    DodajOpisParametrowGenerycznych(metoda.ParametryGeneryczne, poczatek, builder);

                    if (metoda.TypZwracany.Nazwa != "void")
                        builder.AppendLine($"{poczatek}<returns></returns>");

                    solution.AktualnyDokument.WstawWLinii(builder.ToString(), numerLinii);
                }
            }
        }

        private void WstawInheritDoc(string poczatek, int numerLinii)
        {
            solution.AktualnyDokument.WstawWLinii($"{poczatek}<inheritdoc/>", numerLinii);
        }

        private string DajSummaryMetody(string nazwa, int jezyk)
        {
            var slowa = nazwa.PodzielNaSlowaOdWielkichLiter().ToList();

            slowa[0] = jezyk.PrzygotujCzasownik(slowa[0]);

            return string.Join(" ", slowa.Select(o => o.ToLower())).ZacznijDuzaLitera();
        }

        private void PrzetworzPole(Pole pole)
        {
            if (pole.Dokumentacja == null)
            {
                var slowa = pole.Nazwa
                    .PodzielNaSlowaOdWielkichLiter()
                        .Select(o => o.ToLower())
                            .ToArray();

                slowa[0] = slowa[0].TrimStart('_');

                var summary = string.Join(" ", slowa).ZacznijDuzaLitera();

                var wciecie = (pole.Poczatek.Kolumna - 1).Spacji();

                var poczatek = $"{wciecie}/// ";
                var builder = new StringBuilder();

                GennerujSummary(poczatek, builder, summary);

                solution.AktualnyDokument.WstawWLinii(builder.ToString(), DajNumerLiniiDoWstawienia(pole));
            }
        }

        private void PrzetworzWlasciwosc(Property property)
        {
            if (property.Dokumentacja == null)
            {
                var summary = string.Join(" ", property.Nazwa.PodzielNaSlowaOdWielkichLiter().Select(o => o.ToLower())).ZacznijDuzaLitera();

                var wciecie = (property.Poczatek.Kolumna - 1).Spacji();

                var poczatek = $"{wciecie}/// ";
                var builder = new StringBuilder();

                GennerujSummary(poczatek, builder, summary);

                solution.AktualnyDokument.WstawWLinii(builder.ToString(), DajNumerLiniiDoWstawienia(property, property.Atrybuty));
            }
        }

        private int DajNumerLiniiDoWstawienia(ParsowanaJednostka parsowanaJednostka, IEnumerable<Atrybut> atrybuty = null)
        {
            var numerLinii = parsowanaJednostka.Poczatek.Wiersz;

            if (atrybuty != null && atrybuty.Any())
                numerLinii = atrybuty.OrderBy(o => o.Poczatek.Wiersz).First().Poczatek.Wiersz;

            return numerLinii;
        }
    }
}