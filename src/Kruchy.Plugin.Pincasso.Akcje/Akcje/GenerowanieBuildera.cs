using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Akcje.Utils;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kruchy.Plugin.Pincasso.Akcje.Akcje
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
            var zawartoscAktualnego = solution.AktualnyDokument.GetContent();

            var sparsowane = Parser.Parsuj(zawartoscAktualnego);

            DefinedItem obiektDoZbudowania = DajObiektDoZbudowania(sparsowane);

            if (obiektDoZbudowania == null)
            {
                MessageBox.Show("Nie udało się ustalić klasy budowanej");
                return;
            }

            var wlasciwosciDlaBuildera = SzukajWlasciwosciDlaBuildera(obiektDoZbudowania);

            var nazwaKlasyBuildera = obiektDoZbudowania.Name + "Builder";
            var nazwaPlikuBuildera = nazwaKlasyBuildera + ".cs";

            var projektTestow = solution.SzukajProjektuTestowego();

            if (projektTestow == null)
            {
                MessageBox.Show("Nie udało się znaleźć projektu testowego");
                return;
            }

            var katalogBuilderow = Path.Combine(projektTestow.DirectoryPath, "Builders");

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
                    parametry,
                    sparsowane.Namespace);

                if (string.IsNullOrEmpty(zawartosc))
                    return;

                File.WriteAllText(sciezkaDoPlikuBuilder, zawartosc, Encoding.UTF8);
                projektTestow.AddFile(sciezkaDoPlikuBuilder);
            }

            solutionExplorer.OpenFile(sciezkaDoPlikuBuilder);

            UzupelnijMetody(
                wlasciwosciDlaBuildera,
                nazwaKlasyBuildera,
                sparsowane.Usingi.Select(o => o.Nazwa));
        }

        private List<WlasciwoscDlaBuildera> SzukajWlasciwosciDlaBuildera(DefinedItem obiektDoZbudowania)
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

        private DefinedItem DajObiektDoZbudowania(Plik sparsowane)
        {
            DefinedItem obiektDoZbudowania = null;

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
                            solution.AktualnyDokument.GetCursorLineNumber());
            }

            return obiektDoZbudowania;
        }

        private void UzupelnijMetody(
            List<WlasciwoscDlaBuildera> wlasciwosciDlaBuildera,
            string nazwaKlasyBuildera,
            IEnumerable<string> usingiZObiektuBudowanego)
        {
            var sparsowane = Parser.Parsuj(solution.AktualnyDokument.GetContent());

            var klasaBuildera =
                sparsowane.DefiniowaneObiekty.Single(o => o.Name == nazwaKlasyBuildera);

            PlaceInFile miejsceWstawiania = UstalMiejsceWstawienia(klasaBuildera);

            foreach (var wlasciwosc in wlasciwosciDlaBuildera)
            {
                if (JestMetodaDlaWlasciwosci(klasaBuildera, wlasciwosc))
                    continue;

                string tesktMetody = GenerujTrescMetody(nazwaKlasyBuildera, wlasciwosc);

                solution.AktualnyDokument.InsertInLine(
                    tesktMetody + new StringBuilder().AppendLine().ToString(),
                    miejsceWstawiania.Row);
            }

            foreach (var usingBudowanego in usingiZObiektuBudowanego)
                solution.AktualnyDokument.DodajUsingaJesliTrzeba(usingBudowanego);
        }

        private string GenerujTrescMetody(
            string nazwaKlasyBuildera,
            WlasciwoscDlaBuildera wlasciwosc)
        {
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
            return tesktMetody;
        }

        private PlaceInFile UstalMiejsceWstawienia(DefinedItem klasaBuildera)
        {
            var metodaSave = klasaBuildera.Metody.FirstOrDefault(o => o.Name == "Save");

            var miejsceWstawiania = metodaSave?.Poczatek;

            if (miejsceWstawiania == null)
                miejsceWstawiania = klasaBuildera.ClosingBrace;
            return miejsceWstawiania;
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
            DefinedItem klasaBuildera,
            WlasciwoscDlaBuildera wlasciwosc)
        {
            var metodyDoAnalizy = klasaBuildera.Metody.Where(
                o => !(new[] { "Save", "Init" }.Contains(o.Name)));

            var napisUstawiajacyWartoscPola =
                string.Format("Object.{0} =", wlasciwosc.Property.Nazwa);
            if (wlasciwosc.Referencyjne)
                napisUstawiajacyWartoscPola =
                    string.Format("SetReferencedObject(o => o.{0}", wlasciwosc.Property.Nazwa);

            return metodyDoAnalizy.Any(o => ZawieraNapis(o, napisUstawiajacyWartoscPola));
        }

        private bool ZawieraNapis(Method metoda, string napisUstawiajacyWartoscPola)
        {
            for (int i = metoda.Poczatek.Row; i < metoda.Koniec.Row; i++)
            {
                if (solution.AktualnyDokument.GetLineContent(i)
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
            IProjectWrapper projektTestow,
            DefinedItem obiektDoZbudowania,
            List<WlasciwoscDlaBuildera> wlasciwosciDlaBuildera,
            string nazwaKlasyBuildera,
            IParametryGenerowaniaBuildera parametry,
            string namespaceObiektuBudowanego)
        {
            var plik = new PlikClassBuilder();
            plik.WNamespace(projektTestow.Name + ".Builders");
            plik.DodajUsing("Pincasso.Core.Tests.Builders");
            plik.DodajUsing(namespaceObiektuBudowanego);
            plik.DodajUsing("Piatka.Infrastructure.Tests.Builders");

            var nazwaKlasyInterfejsuSerwisu = parametry.NazwaInterfejsuService;

            if (string.IsNullOrEmpty(nazwaKlasyInterfejsuSerwisu))
                return null;

            plik.ZObiektem(
                new ClassBuilder()
                    .ZModyfikatorem("public")
                    .ZNazwa(nazwaKlasyBuildera)
                    .ZNadklasa(
                        string.Format("Builder<{0}, {1}>",
                        nazwaKlasyInterfejsuSerwisu,
                        obiektDoZbudowania.Name))
                    .DodajMetode(
                        new MetodaBuilder()
                            .ZNazwa("Init")
                            .ZTypemZwracanym("void")
                            .DodajModyfikator("protected")
                            .DodajModyfikator("override")
                            .DodajLinie(
                            string.Format("this.Object = new {0}();", obiektDoZbudowania.Name)))
                    .DodajMetode(
                        new MetodaBuilder()
                            .ZNazwa("Save")
                            .ZTypemZwracanym("void")
                            .DodajModyfikator("protected")
                            .DodajModyfikator("override")
                            .DodajParametr("IValidationResultListener", "validationListener"))
                        );

            return plik.Build();
        }

        private bool PrzechowujeObiektReferencji(Property p)
        {
            return p.Atrybuty.Any(o => o.Name == "ReferencedObject");
        }

        private bool JestKluczemObcym(Property p)
        {
            return p.Atrybuty.Any(o => o.Name == "ForeignKey");
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
