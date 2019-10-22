using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje.DodawanieMapowanElementy;
using KruchyParserKodu.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class DodawanieMapowan
    {
        private readonly ISolutionWrapper solution;
        private string[] nazwyAtrybutow = { "Map", "MapFrom", "MapTo" };

        public DodawanieMapowan(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Generuj()
        {
            var parsowane =
                Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());

            var obiekt = parsowane.DefiniowaneObiekty.First();

            if (!obiekt.Atrybuty.Any(o => AtrybutMapowan(o)))
                return;

            var nazwaKlasyMapowanej = SzukajNazwyKlasyMapowanej(obiekt);

            var sciezkaDoKlasyMapowanej = 
                SzukajSciezkiKlasy(
                    nazwaKlasyMapowanej,
                    PrzygotujRozwazaneNamespacy(parsowane).ToList());
            var opisMapowan =
                SzukajAtrybutowDlaMapowan(
                    sciezkaDoKlasyMapowanej,
                    nazwaKlasyMapowanej,
                    string.Empty);

            var okno = new WyborMapowanForm(opisMapowan);
            okno.ShowDialog();
            if (!okno.Wybrane.Any())
                return;
            DodajMapowaniaWgOpisow(obiekt, okno.Wybrane);
        }

        private void DodajMapowaniaWgOpisow(
            Obiekt obiekt,
            IList<MapowanyProperty> opisMapowan,
            string prefix = "")
        {
            var builder = new StringBuilder();
            DodajMapowaniaWgOpisow(builder, obiekt, opisMapowan);
            var tekst = builder.ToString();

            int numerLiniiDodawania = obiekt.PoczatkowaKlamerka.Wiersz + 1;
            if (obiekt.Propertiesy.Any())
                numerLiniiDodawania =
                    obiekt
                        .Propertiesy
                            .Select(o => o.Poczatek.Wiersz)
                                .Max() + 2;
            solution.AktualnyDokument.WstawWMiejscu(
                tekst, numerLiniiDodawania, 1);
        }

        private void DodajMapowaniaWgOpisow(
            StringBuilder builder,
            Obiekt obiekt,
            IList<MapowanyProperty> opisMapowan)
        {
            foreach (var opis in opisMapowan)
            {
                if (!obiekt.Propertiesy.Any(o => o.Nazwa == opis.Nazwa))
                {
                    var propBuilder = new PropertyBuilder();
                    var napis = propBuilder
                        .ZNazwa(opis.Prefix + opis.Nazwa)
                            .ZNazwaTypu(opis.NazwaTypu)
                                .Build(StaleDlaKodu.WciecieDlaMetody);
                    builder.AppendLine(napis);
                }
                DodajMapowaniaWgOpisow(builder, obiekt, opis.Podobiekty);
            }
        }

        private IList<MapowanyProperty> SzukajAtrybutowDlaMapowan(
            string sciezkaDoKlasyMapowanej,
            string nazwaKlasyMapowanej,
            string prefix)
        {
            var parsowane = Parser.ParsujPlik(sciezkaDoKlasyMapowanej);
            var klasa = parsowane.DefiniowaneObiekty.
                Where(o => o.Nazwa == nazwaKlasyMapowanej).First();

            var wynik = new List<MapowanyProperty>();

            wynik.AddRange(
                klasa.Propertiesy
                    .Select(o => DajMapowanieDlaProperty(o, prefix, parsowane)));

            return wynik;
        }

        private MapowanyProperty DajMapowanieDlaProperty(
            Property property,
            string prefix,
            Plik parsowane)
        {
            var wynik =
                new MapowanyProperty(
                    property.Nazwa,
                    property.NazwaTypu,
                    prefix);
            var sciezka =
                SzukajSciezkiKlasy(
                    property.NazwaTypu,
                    PrzygotujRozwazaneNamespacy(parsowane).ToList());
            if (sciezka != null)
            {
                wynik.Podobiekty.AddRange(
                    SzukajAtrybutowDlaMapowan(
                       sciezka,
                        property.NazwaTypu,
                        prefix + property.Nazwa));
            }
            return wynik;
        }

        private string SzukajSciezkiKlasy(
            string nazwaKlasyMapowanej,
            List<string> listaNamespacow)
        {
            var pliki = SzukajPlikowZNamespacow(listaNamespacow).ToList();
            return pliki.Where(o => PlikKlasy(o, nazwaKlasyMapowanej))
                .FirstOrDefault();
        }

        private bool PlikKlasy(string nazwaPliku, string nazwaKlasyMapowanej)
        {
            var szukanaNazwa = nazwaKlasyMapowanej.ToLower() + ".cs";
            FileInfo fi = new FileInfo(nazwaPliku);
            return fi.FullName.ToLower().EndsWith(szukanaNazwa);
        }

        private IEnumerable<string> DajPlikiDomainZProjektu(
            Kruchy.Plugin.Utils.Wrappers.ProjektWrapper projekt)
        {
            var plikiProjektu = projekt.Pliki;
            var sciezkaDoKataloguDomain =
                Path.Combine(projekt.SciezkaDoKatalogu, "Domain")
                    .ToLower();
            return plikiProjektu.Where(
                o => o.SciezkaPelna.ToLower().StartsWith(sciezkaDoKataloguDomain))
                    .Select(o => o.SciezkaPelna)
                        .ToList();
        }

        private string SzukajNazwyKlasyMapowanej(Obiekt obiekt)
        {
            var atrybut = obiekt.Atrybuty.Where(o => AtrybutMapowan(o)).First();
            var wartosc = atrybut.Parametry.First().Wartosc;
            if (!wartosc.StartsWith("typeof("))
                throw new ApplicationException("Błędna wartość opisującą klasę");
            var wynik = wartosc.Replace("typeof(", "");
            return wynik.Substring(0, wynik.Length - 1);
        }

        private bool AtrybutMapowan(Atrybut atrybut)
        {
            return nazwyAtrybutow.Any(o => o == atrybut.Nazwa);
        }

        private IEnumerable<string> PrzygotujRozwazaneNamespacy(Plik parsowane)
        {
            foreach (var u in parsowane.Usingi)
                yield return u.Nazwa;
            yield return parsowane.Namespace;
        }

        private IEnumerable<string> SzukajPlikowZNamespacow(
            IEnumerable<string> namespacy)
        {
            var wynik = new List<string>();
            foreach (var n in namespacy)
            {
                wynik.AddRange(SzukajPlikowZNamespace(n));
            }
            return wynik.Distinct();
        }

        private IEnumerable<string> SzukajPlikowZNamespace(
            string nazwaNamespace)
        {
            foreach (var p in solution.Projekty)
            {
                if (p.NamespaceNalezyDoProjektu(nazwaNamespace))
                    return p.DajPlikiZNamespace(nazwaNamespace);
            }
            return new List<string>();
        }
    }
}