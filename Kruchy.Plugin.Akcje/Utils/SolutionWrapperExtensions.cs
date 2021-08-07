using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Utils
{
    public static class SolutionWrapperExtensions
    {
        public static IProjektWrapper SzukajProjektuTestowego(
            this ISolutionWrapper solution,
            IProjektWrapper projekt)
        {
            var nazwaSzukanegoProjektu = solution.AktualnyProjekt.Nazwa + ".Tests";
            var projektTestow = solution.SzukajProjektuWgNazwy(nazwaSzukanegoProjektu);
            return projektTestow;
        }

        public static IProjektWrapper SzukajProjektuModulu(
            this ISolutionWrapper solution,
            IProjektWrapper projekt)
        {
            var nazwaSzukanegoProjektu =
                solution.AktualnyProjekt.Nazwa.Replace(".Tests", "");
            return solution.SzukajProjektuWgNazwy(nazwaSzukanegoProjektu);
        }
    }
}
