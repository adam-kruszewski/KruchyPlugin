using System.Linq;
using System.Windows;
using KruchyCompany.KruchyPlugin1.ParserKodu;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class IdzDoKlasyTestowej
    {
        private readonly SolutionWrapper solution;

        public IdzDoKlasyTestowej(SolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Przejdz()
        {
            if (solution.AktualnyPlik == null)
                return;

            if (solution.AktualnyProjekt.Nazwa.EndsWith(".Tests"))
                return;

            var parsowane = Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());

            var nazwaSzukanegoProjektu = solution.AktualnyProjekt.Nazwa + ".Tests";
            var projektTestow =
                solution
                    .Projekty
                        .Where(o => o.Nazwa == nazwaSzukanegoProjektu)
                            .FirstOrDefault();

            if (projektTestow == null)
            {
                MessageBox.Show("Nieznaleziono projektu testowego " + nazwaSzukanegoProjektu);
                return;
            }

            var nazwaSzukanegoPliku =
                parsowane.DefiniowaneObiekty.First().Nazwa + "Tests.cs";

            var plik = projektTestow.Pliki
                    .Where(o => o.Nazwa.ToLower() == nazwaSzukanegoPliku.ToLower())
                        .FirstOrDefault();

            if (plik == null)
            {
                MessageBox.Show("Nieznaleziono pliku: " + nazwaSzukanegoPliku);
                return;
            }

            new SolutionExplorerWrapper(solution).OtworzPlik(plik);
        }
    }
}