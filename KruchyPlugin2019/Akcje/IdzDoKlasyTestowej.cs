using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Kruchy.Plugin.Akcje.Utils;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Akcje
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
            if (solution.AktualnyPlik == null)
                return;

            var parsowane = Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());

            IPlikWrapper plik;
            if (solution.AktualnyProjekt.Modul())
            {
                var projektTestow =
                    solution.SzukajProjektuTestowego(solution.AktualnyProjekt);

                if (projektTestow == null)
                {
                    System.Windows.MessageBox.Show("Nie znaleziono projektu testowego ");
                    return;
                }

                var nazwaSzukanegoPliku =
                    DajRdzenNazwyKlasyTestow(parsowane) + "Tests.cs";

                plik = projektTestow.Pliki
                        .Where(o => o.Nazwa.ToLower() == nazwaSzukanegoPliku.ToLower())
                            .FirstOrDefault();
            }
            else
            {
                var projektModulu =
                    solution.SzukajProjektuModulu(solution.AktualnyProjekt);

                if (projektModulu == null)
                {
                    System.Windows.MessageBox.Show("Nie znaleziono projektu modułu");
                    return;
                }

                var nazwaSzukanegoPliku =
                    solution.AktualnyPlik.NazwaBezRozszerzenia.ToLower()
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
                System.Windows.MessageBox.Show("Nie znaleziono pliku: ");
                return;
            }


            solutionExplorer.OtworzPlik(plik);
        }

        private string SzukajNazwyKlasyTestowanejZServiceTests()
        {
            var parsowane = Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());
            var klasa = parsowane.DefiniowaneObiekty.First();
            if (KlasaServiceTests(klasa))
            {
                var nazwaKlasyLubInterfejsu =
                    klasa.NadklasaIInterfejsy.First().NazwyTypowParametrow.First();
                if (nazwaKlasyLubInterfejsu.StartsWith("I") && char.IsUpper(nazwaKlasyLubInterfejsu[1]))
                    return nazwaKlasyLubInterfejsu.Substring(1);
                else
                    return nazwaKlasyLubInterfejsu;
            }

            return null;
        }

        private bool KlasaServiceTests(Obiekt klasa)
        {
            var nadklasa = klasa.NadklasaIInterfejsy.FirstOrDefault();
            if (nadklasa == null)
                return false;

            if (nadklasa.Nazwa == "ServiceTests")
                return true;

            return false;
        }

        private static IPlikWrapper SzukajPlikiKlasyTestowanej(
            IProjektWrapper projektModulu,
            string nazwaSzukanegoPliku)
        {
            return projektModulu
                    .Pliki
                        .Where(o => o.NazwaBezRozszerzenia.ToLower() == nazwaSzukanegoPliku.ToLower())
                            .FirstOrDefault();
        }

        private string DajRdzenNazwyKlasyTestow(Plik parsowane)
        {
            var nazwa = parsowane.DefiniowaneObiekty.First().Nazwa;
            if (parsowane.DefiniowaneObiekty.First().Rodzaj == RodzajObiektu.Klasa)
                return nazwa;
            else
                return nazwa.Substring(1);
        }
    }
}