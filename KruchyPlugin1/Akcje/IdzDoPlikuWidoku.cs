using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using KruchyCompany.KruchyPlugin1.Extensions;
using KruchyCompany.KruchyPlugin1.Utils;
using KruchyParserKodu.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class IdzDoPlikuWidoku
    {
        private readonly SolutionWrapper solution;

        public IdzDoPlikuWidoku(SolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void PrzejdzDoWidokuDlaAktualnejMetody()
        {
            if (solution.AktualnyDokument == null)
                return;
            var zawartosc = solution.AktualnyDokument.DajZawartosc();
            var parsowane = Parser.Parsuj(zawartosc);
            var liniaKursora = solution.AktualnyDokument.DajNumerLiniiKursora();

            var aktualnaMetoda = parsowane.SzukajMetodyWLinii(liniaKursora);

            if (aktualnaMetoda != null)
                PrzejdzLubStworz(aktualnaMetoda.Nazwa + ".cshtml", false);
            else
                MessageBox.Show("Kursor nie znajduje się w żadnej metodzie");
        }

        public void PrzejdzLubStworz(string nazwaPliku, bool tworzJesliNieIstnieje = true)
        {
            if (!solution.CzyPlikControllera())
                MessageBox.Show("To nie jest plik controllera");

            var aktualny = solution.AktualnyPlik;
            var nazwaControllera =
                aktualny.NazwaBezRozszerzenia.DajNazweControllera();

            var listaSciezek = new List<string>();
            if (CzyJestWArea(aktualny))
            {
                listaSciezek.Add(DajSciezkeWArea(aktualny, nazwaControllera, nazwaPliku));
                listaSciezek.Add(DajSciezkeSharedWArea(aktualny, nazwaPliku));
            }
            listaSciezek.Add(DajSciezkeWOgolnych(aktualny.Projekt, nazwaControllera, nazwaPliku));
            listaSciezek.Add(DajSciezkeSharedWOgolnych(aktualny.Projekt, nazwaPliku));

            foreach (var sciezka in listaSciezek)
            {
                if (File.Exists(sciezka))
                {
                    new SolutionExplorerWrapper(solution)
                        .OtworzPlik(sciezka);
                    return;
                }
            }
            if (tworzJesliNieIstnieje)
            {
                SprobujStworzyc(aktualny, listaSciezek, nazwaPliku);
            }
        }

        private void SprobujStworzyc(
            PlikWrapper aktualny,
            List<string> listaSciezek,
            string nazwaPliku)
        {
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

        private string DajSciezkeWOgolnych(
            ProjektWrapper projekt,
            string nazwaControllera,
            string nazwaPliku)
        {
            var sciezka = Path.Combine(
                projekt.SciezkaDoKatalogu,
                "Views",
                nazwaControllera,
                nazwaPliku);
            return sciezka;
        }

        private string DajSciezkeSharedWOgolnych(
            ProjektWrapper projekt,
            string nazwaPliku)
        {
            var sciezka = Path.Combine(
                projekt.SciezkaDoKatalogu,
                "Views",
                "Shared",
                nazwaPliku);
            return sciezka;
        }

        private string DajSciezkeWArea(
            PlikWrapper aktualny,
            string nazwaControllera,
            string nazwaPliku)
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

        private string DajSciezkeSharedWArea(PlikWrapper aktualny, string nazwaPliku)
        {
            var katalogWArea = Directory.GetParent(aktualny.Katalog);
            var sciezkaDoPliku =
                Path.Combine(
                    katalogWArea.FullName,
                    "Views",
                    "Shared",
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