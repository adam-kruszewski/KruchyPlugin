using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Akcje.Akcje.Generowanie.Buildera.Komponenty;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace Kruchy.Plugin.Akcje.Akcje
{
    public class GenerowanieBuildera
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public GenerowanieBuildera(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public void Generuj(
            IParametryGenerowaniaBuildera parametry)
        {
            var zawartoscAktualnego = solution.AktualnyDokument.DajZawartosc();

            var sparsowane = Parser.Parsuj(zawartoscAktualnego);

            Obiekt obiektDoZbudowania = DajObiektDoZbudowania(sparsowane);

            if (obiektDoZbudowania == null)
            {
                MessageBox.Show("Nie udało się ustalić klasy budowanej");
                return;
            }

            var wlasciwosciDlaBuildera = SzukajWlasciwosciDlaBuildera(obiektDoZbudowania);

            var nazwaKlasyBuildera = obiektDoZbudowania.Nazwa + "Builder";
            var nazwaPlikuBuildera = nazwaKlasyBuildera + ".cs";

            var projektTestow =
                    solution.SzukajProjektuTestowego(solution.AktualnyProjekt);

            if (projektTestow == null)
            {
                MessageBox.Show("Nie udało się znaleźć projektu testowego");
                return;
            }

            var katalogBuilderow = Path.Combine(projektTestow.SciezkaDoKatalogu, "Builders");

            if (!Directory.Exists(katalogBuilderow))
                Directory.CreateDirectory(katalogBuilderow);

            var sciezkaDoPlikuBuilder =
                Path.Combine(katalogBuilderow, nazwaPlikuBuildera);

            if (!File.Exists(sciezkaDoPlikuBuilder))
            {
                string zawartosc = GenerujZawartosc(
                    projektTestow,
                    obiektDoZbudowania,
                    wlasciwosciDlaBuildera,
                    nazwaKlasyBuildera,
                    parametry);

                File.WriteAllText(sciezkaDoPlikuBuilder, zawartosc, Encoding.UTF8);
                projektTestow.DodajPlik(sciezkaDoPlikuBuilder);
            }

            solutionExplorer.OtworzPlik(sciezkaDoPlikuBuilder);

            UzupelnijMetody(wlasciwosciDlaBuildera, nazwaKlasyBuildera);
        }

        private List<WlasciwoscDlaBuildera> SzukajWlasciwosciDlaBuildera(Obiekt obiektDoZbudowania)
        {
            var polaBedaceReferencja =
                obiektDoZbudowania
                    .Propertiesy
                        .Where(o => JestKluczemObcym(o));

            var polaPrzechowujaceObiektReferencji =
                obiektDoZbudowania
                    .Propertiesy
                        .Where(o => PrzechowujeObiektReferencji(o));

            var polaZwykle =
                obiektDoZbudowania
                    .Propertiesy
                        .Where(o => !JestKluczemObcym(o) && !PrzechowujeObiektReferencji(o));

            var wlasciwosciDlaBuildera = new List<WlasciwoscDlaBuildera>();

            wlasciwosciDlaBuildera.AddRange(
                polaPrzechowujaceObiektReferencji
                    .Select(o => new WlasciwoscDlaBuildera(o, true)));

            wlasciwosciDlaBuildera.AddRange(
                polaZwykle.Select(o => new WlasciwoscDlaBuildera(o, false)));
            return wlasciwosciDlaBuildera;
        }

        private Obiekt DajObiektDoZbudowania(Plik sparsowane)
        {
            Obiekt obiektDoZbudowania = null;

            if (sparsowane.DefiniowaneObiekty.Count(o => o.Rodzaj == RodzajObiektu.Klasa) == 1)
            {
                obiektDoZbudowania =
                    sparsowane.DefiniowaneObiekty.Single(o => o.Rodzaj == RodzajObiektu.Klasa);
            }
            else
            {
                obiektDoZbudowania =
                    sparsowane
                        .SzukajKlasyWLinii(
                            solution.AktualnyDokument.DajNumerLiniiKursora());
            }

            return obiektDoZbudowania;
        }

        private void UzupelnijMetody(
            List<WlasciwoscDlaBuildera> wlasciwosciDlaBuildera,
            string nazwaKlasyBuildera)
        {
            var sparsowane = Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());

            var klasaBuildera =
                sparsowane.DefiniowaneObiekty.Single(o => o.Nazwa == nazwaKlasyBuildera);

            var metodaSave = klasaBuildera.Metody.FirstOrDefault(o => o.Nazwa == "Save");

            var miejsceWstawiania = metodaSave?.Poczatek;

            if (miejsceWstawiania == null)
                miejsceWstawiania = klasaBuildera.KoncowaKlamerka;

            foreach (var wlasciwosc in wlasciwosciDlaBuildera)
            {
                if (JestMetodaDlaWlasciwosci(klasaBuildera, wlasciwosc))
                    continue;

                var nazwaParametru = DajNazweParametru(wlasciwosc.Property.Nazwa);

                var tesktMetodyBuilder =
                    new MetodaBuilder()
                        .DodajModyfikator("public")
                        .ZNazwa("Z" + wlasciwosc.Property.Nazwa)
                        .ZTypemZwracanym(nazwaKlasyBuildera)
                        .DodajParametr(wlasciwosc.Property.NazwaTypu, nazwaParametru);

                tesktMetodyBuilder.DodajLinie(
                    DajLinieUstawiajacaWartosc(wlasciwosc, nazwaParametru));
                tesktMetodyBuilder.DodajLinie("return this;");

                var tesktMetody = tesktMetodyBuilder.Build(StaleDlaKodu.WciecieDlaMetody);

                solution.AktualnyDokument.WstawWLinii(
                    tesktMetody + new StringBuilder().AppendLine().ToString(),
                    miejsceWstawiania.Wiersz);
            }
        }

        private string DajLinieUstawiajacaWartosc(
            WlasciwoscDlaBuildera wlasciwosc,
            string nazwaParametru)
        {
            if (!wlasciwosc.Referencyjne)
                return string.Format("Object.{0} = {1};",
                    wlasciwosc.Property.Nazwa,
                    nazwaParametru);
            else
                return string.Format(
                    "SetReferencedObject(o => o.{0}, {1});",
                    wlasciwosc.Property.Nazwa,
                    nazwaParametru);
        }

        private bool JestMetodaDlaWlasciwosci(
            Obiekt klasaBuildera,
            WlasciwoscDlaBuildera wlasciwosc)
        {
            var metodyDoAnalizy = klasaBuildera.Metody.Where(
                o => new[] { "Save", "Init" }.Contains(o.Nazwa));

            var napisUstawiajacyWartoscPola =
                string.Format("Object.{0} =", wlasciwosc.Property.Nazwa);

            return metodyDoAnalizy.Any(o => ZawieraNapis(o, napisUstawiajacyWartoscPola));
        }

        private bool ZawieraNapis(Metoda metoda, string napisUstawiajacyWartoscPola)
        {
            for (int i = metoda.Poczatek.Wiersz; i < metoda.Koniec.Wiersz; i++)
            {
                if (solution.AktualnyDokument.DajZawartoscLinii(i)
                    .Contains(napisUstawiajacyWartoscPola))
                    return true;
            }

            return false;
        }

        private string DajNazweParametru(string nazwaTypu)
        {
            var builder = new StringBuilder();
            builder.Append(char.ToLower(nazwaTypu[0]));
            builder.Append(nazwaTypu.Substring(1));

            return builder.ToString();
        }

        private string GenerujZawartosc(
            IProjektWrapper projektTestow,
            Obiekt obiektDoZbudowania,
            List<WlasciwoscDlaBuildera> wlasciwosciDlaBuildera,
            string nazwaKlasyBuildera,
            IParametryGenerowaniaBuildera parametry)
        {
            var plik = new PlikClassBuilder();
            plik.WNamespace(projektTestow.Nazwa + "Builders");
            plik.DodajUsing("Pincasso.Core.Tests.Builders");
            plik.ZObiektem(
                new ClassBuilder()
                    .ZModyfikatorem("public")
                    .ZNazwa(nazwaKlasyBuildera)
                    .ZNadklasa(
                        string.Format("Builder<{0}, {1}>",
                        parametry.NazwaInterfejsuService,
                        obiektDoZbudowania.Nazwa))
                    .DodajMetode(
                        new MetodaBuilder()
                            .ZNazwa("Init")
                            .ZTypemZwracanym("void")
                            .DodajModyfikator("protected")
                            .DodajModyfikator("override")
                            .DodajLinie(
                            string.Format("this.Object = new {0}()", obiektDoZbudowania.Nazwa)))
                    .DodajMetode(
                        new MetodaBuilder()
                            .ZNazwa("Save")
                            .ZTypemZwracanym("void")
                            .DodajModyfikator("protected")
                            .DodajModyfikator("override")
                            .DodajParametr("IValidationResultListener", "validationListener"))
                        );

            //protected override void Save(IValidationResultListener validationListener)
            return plik.Build();
        }

        private bool PrzechowujeObiektReferencji(Property p)
        {
            return p.Atrybuty.Any(o => o.Nazwa == "ReferencedObject");
        }

        private bool JestKluczemObcym(Property p)
        {
            return p.Atrybuty.Any(o => o.Nazwa == "ForeignKey");
        }

        private class WlasciwoscDlaBuildera
        {
            public Property Property { get; set; }

            public bool Referencyjne { get; set; }

            public WlasciwoscDlaBuildera(Property property, bool referencyjne)
            {
                Property = property;
                Referencyjne = referencyjne;
            }
        }
    }
}