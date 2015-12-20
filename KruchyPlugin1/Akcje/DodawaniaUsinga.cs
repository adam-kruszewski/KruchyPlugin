using System.Windows;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class DodawaniaUsinga
    {
        private readonly SolutionWrapper solution;

        public DodawaniaUsinga(SolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Dodaj(string nazwaUsinga)
        {
            if (solution.AktualnyPlik == null)
            {
                MessageBox.Show("Brak otwartego pliku");
                return;
            }
            solution
                .AktualnyPlik
                    .Dokument
                        .DodajUsingaJesliTrzeba(nazwaUsinga);
        }

        public void Dodaj(params string[] usingi)
        {
            if (solution.AktualnyPlik == null)
            {
                MessageBox.Show("Brak otwartego pliku");
                return;
            }
            foreach (var nazwaUsinga in usingi)
                solution
                    .AktualnyPlik
                        .Dokument
                            .DodajUsingaJesliTrzeba(nazwaUsinga);

        }
    }
}