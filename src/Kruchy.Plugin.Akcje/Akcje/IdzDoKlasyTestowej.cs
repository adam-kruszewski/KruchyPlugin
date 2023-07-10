using Kruchy.Plugin.Akcje.Utils;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
using System.Linq;
using System.Windows.Forms;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class IdzDoKlasyTestowej
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public IdzDoKlasyTestowej(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public void Przejdz()
        {
            if (solution.CurrentFile == null)
                return;

            var parsowane = Parser.Parse(solution.CurenctDocument.GetContent());

            IFileWrapper plik = null;
            if (solution.CurrentProject.Modul())
            {
                var projektTestow = solution.SzukajProjektuTestowego();

                var nazwaSzukanegoPliku =
                    DajRdzenNazwyKlasyTestow(parsowane) + "Tests.cs";

                if (projektTestow != null)
                    plik = projektTestow.Files
                            .Where(o => o.Name.ToLower() == nazwaSzukanegoPliku.ToLower())
                                .FirstOrDefault();

                if (plik == null)
                    plik = solution.Projects.SelectMany(o => o.Files)
                        .Where(o => o.Name.ToLower() == nazwaSzukanegoPliku.ToLower())
                            .FirstOrDefault();
            }
            else
            {
                var projektModulu = solution.SzukajProjektuModulu();

                var nazwaSzukanegoPliku =
                    solution.CurrentFile.NameWithoutExtension.ToLower()
                    .Replace("tests", "");

                plik = SzukajPlikiKlasyTestowanej(projektModulu, nazwaSzukanegoPliku);

                if (plik == null)
                {
                    var nazwaNaPodstawieKlasyTestowanej =
                        SzukajNazwyKlasyTestowanejZServiceTests();
                    plik = SzukajPlikiKlasyTestowanej(
                        projektModulu,
                        nazwaNaPodstawieKlasyTestowanej);
                }

            }

            if (plik == null)
            {
                MessageBox.Show("Nie znaleziono pliku: ");
                return;
            }


            solutionExplorer.OpenFile(plik);
        }

        private string SzukajNazwyKlasyTestowanejZServiceTests()
        {
            var parsowane = Parser.Parse(solution.CurenctDocument.GetContent());
            var klasa = parsowane.DefinedItems.First();
            if (KlasaServiceTests(klasa))
            {
                var nazwaKlasyLubInterfejsu =
                    klasa.SuperClassAndInterfaces.First().ParameterTypeNames.First();
                if (nazwaKlasyLubInterfejsu.StartsWith("I") && char.IsUpper(nazwaKlasyLubInterfejsu[1]))
                    return nazwaKlasyLubInterfejsu.Substring(1);
                else
                    return nazwaKlasyLubInterfejsu;
            }

            return null;
        }

        private bool KlasaServiceTests(DefinedItem klasa)
        {
            var nadklasa = klasa.SuperClassAndInterfaces.FirstOrDefault();
            if (nadklasa == null)
                return false;

            if (nadklasa.Name == "ServiceTests")
                return true;

            return false;
        }

        private IFileWrapper SzukajPlikiKlasyTestowanej(
            IProjectWrapper projektModulu,
            string nazwaSzukanegoPliku)
        {
            IFileWrapper fileWrapper = null;

            if (projektModulu != null)
                fileWrapper = projektModulu
                        .Files
                            .Where(o => o.NameWithoutExtension.ToLower() == nazwaSzukanegoPliku.ToLower())
                                .FirstOrDefault();

            if (fileWrapper == null)
                fileWrapper = solution.Projects.SelectMany(o => o.Files)
                    .Where(o => o.NameWithoutExtension.ToLower() == nazwaSzukanegoPliku.ToLower())
                        .FirstOrDefault();

            return fileWrapper;
        }

        private string DajRdzenNazwyKlasyTestow(FileWithCode parsowane)
        {
            var nazwa = parsowane.DefinedItems.First().Name;
            if (parsowane.DefinedItems.First().KindOfItem == KindOfItem.Class)
                return nazwa;
            else
                return nazwa.Substring(1);
        }
    }
}
