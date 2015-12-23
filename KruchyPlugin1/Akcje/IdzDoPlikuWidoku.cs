using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using KruchyCompany.KruchyPlugin1.Extensions;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class IdzDoPlikuWidoku
    {
        private readonly string nazwaPliku;
        private readonly SolutionWrapper solution;

        public IdzDoPlikuWidoku(string nazwaPliku, SolutionWrapper solution)
        {
            this.nazwaPliku = nazwaPliku;
            this.solution = solution;
        }

        public void PrzejdzLubStworz()
        {
            if (!solution.CzyPlikControllera())
                MessageBox.Show("To nie jest plik controllera");

            var aktualny = solution.AktualnyPlik;
            var nazwaControllera =
                aktualny.NazwaBezRozszerzenia.DajNazweControllera();

            var listaSciezek = new List<string>();
            if (CzyJestWArea(aktualny))
                listaSciezek.Add(DajSciezkeWArea(aktualny, nazwaControllera));
            listaSciezek.Add(DajSciezkeWOgolnych(aktualny.Projekt, nazwaControllera));

            foreach (var sciezka in listaSciezek)
            {
                if (File.Exists(sciezka))
                {
                    new SolutionExplorerWrapper(solution)
                        .OtworzPlik(sciezka);
                    return;
                }
            }

            if (MessageBox
                .Show(
                    "Plik " + nazwaPliku + " nie istnieje. Czy chcesz go utworzyć?",
                    "Przejście do pliku widoku",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var sciezka = listaSciezek.First();
                UtworzKatalogDlaSciezkiJesliTrzeba(sciezka);
                File.WriteAllText(sciezka, "", Encoding.UTF8);
                aktualny.Projekt.DodajPlik(sciezka);
                new SolutionExplorerWrapper(solution).OtworzPlik(sciezka);
            }
        }

        private void UtworzKatalogDlaSciezkiJesliTrzeba(string sciezka)
        {
            var fi = new FileInfo(sciezka);
            if (!Directory.Exists(fi.Directory.FullName))
                Directory.CreateDirectory(fi.Directory.FullName);
        }

        private string DajSciezkeWOgolnych(ProjektWrapper projekt, string nazwaControllera)
        {
            var sciezka = Path.Combine(
                projekt.SciezkaDoKatalogu,
                "Views",
                nazwaControllera,
                nazwaPliku);
            return sciezka;
        }

        private string DajSciezkeWArea(PlikWrapper aktualny, string nazwaControllera)
        {
            var katalogWArea = Directory.GetParent(aktualny.Katalog);
            var sciezkaDoPliku =
                Path.Combine(
                    katalogWArea.FullName,
                    "Views",
                    nazwaControllera,
                    nazwaPliku);
            return sciezkaDoPliku;
        }

        private bool CzyJestWArea(PlikWrapper aktualny)
        {
            var elementySciezki = aktualny.SciezkaPelna.Split(
                System.IO.Path.DirectorySeparatorChar);
            var indeks = elementySciezki.Length - 4;
            if (elementySciezki.Length > 4 &&
                elementySciezki[indeks].ToLower() == "areas")
                return true;

            return false;
        }
    }
}