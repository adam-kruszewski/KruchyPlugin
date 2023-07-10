using Kruchy.Plugin.Utils.Wrappers;
using System.Linq;
using System.Windows.Forms;

namespace Kruchy.Plugin.Akcje.Utils
{
    public static class KlasaTestowanaExtensions
    {
        public static IFileWrapper SzukajPlikuKlasyTestowanej(this ISolutionWrapper solution)
        {
            var projektModulu = solution.SzukajProjektuModulu();

            if (projektModulu == null)
            {
                MessageBox.Show("Nie znaleziono projektu modułu");
                return null;
            }

            var nazwaSzukanegoPliku =
                solution.CurrentFile.NameWithoutExtension.ToLower()
                .Replace("tests", "");
            var plik = SzukajPlikuKlasyTestowanej(projektModulu, nazwaSzukanegoPliku);

            return plik;
        }

        private static IFileWrapper SzukajPlikuKlasyTestowanej(
            IProjectWrapper projektModulu,
            string nazwaSzukanegoPliku)
        {
            return projektModulu
                    .Files
                        .Where(o => o.NameWithoutExtension.ToLower() == nazwaSzukanegoPliku.ToLower())
                            .FirstOrDefault();
        }
    }
}