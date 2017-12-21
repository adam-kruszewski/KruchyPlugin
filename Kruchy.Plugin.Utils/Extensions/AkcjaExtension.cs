using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Utils.Extensions
{
    public static class AkcjaExtension
    {
        public static bool CzyPlikControllera(this SolutionWrapper solution)
        {
            var aktualny = solution.AktualnyPlik;
            if (aktualny == null)
                return false;

            if (!aktualny.Nazwa.ToLower().EndsWith("controller.cs"))
                return false;

            return true;
        }

        public static string DajNazweControllera(this string nazwaKlasyControllera)
        {
            var dl = "Controller".Length;
            return nazwaKlasyControllera.Substring(
                0,
                nazwaKlasyControllera.Length - dl);
        }
    }
}
