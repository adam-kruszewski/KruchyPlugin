using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using System.Linq;

namespace Kruchy.Plugin.Akcje.Utils
{
    public static class KlasaTestowanaExtensions
    {
        public static IPlikWrapper SzukajPlikuKlasyTestowanej(this ISolutionWrapper solution)
        {
            var projektModulu =
                solution.SzukajProjektuModulu(solution.AktualnyProjekt);

            if (projektModulu == null)
            {
                System.Windows.MessageBox.Show("Nie znaleziono projektu modułu");
                return null;
            }

            var nazwaSzukanegoPliku =
                solution.AktualnyPlik.NazwaBezRozszerzenia.ToLower()
                .Replace("tests", "");
            var plik = SzukajPlikuKlasyTestowanej(projektModulu, nazwaSzukanegoPliku);

            return plik;
        }

        private static IPlikWrapper SzukajPlikuKlasyTestowanej(
            IProjektWrapper projektModulu,
            string nazwaSzukanegoPliku)
        {
            return projektModulu
                    .Pliki
                        .Where(o => o.NazwaBezRozszerzenia.ToLower() == nazwaSzukanegoPliku.ToLower())
                            .FirstOrDefault();
        }
    }
}