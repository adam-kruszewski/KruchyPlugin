using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using System.Linq;

namespace Kruchy.Plugin.Akcje.Utils
{
    public static class SolutionWrapperExtensions
    {
        public static IProjektWrapper SzukajProjektuTestowego(
            this ISolutionWrapper solution)
        {
            var konfiguracja = Konfiguracja.GetInstance(solution);

            if (konfiguracja != null)
            {
                var powiazanie =
                    konfiguracja?.PowiazaniaProjektowTestowych
                    ?.FirstOrDefault(o => o.NazwaProjektu == solution.AktualnyProjekt.Nazwa);

                if (powiazanie != null)
                    return solution.Projekty.Single(o => o.Nazwa == powiazanie.NazwaProjektuTestowego);
            }

            var nazwaSzukanegoProjektu = solution.AktualnyProjekt.Nazwa + ".Tests";
            var projektTestow = solution.SzukajProjektuWgNazwy(nazwaSzukanegoProjektu);
            return projektTestow;
        }

        public static IProjektWrapper SzukajProjektuModulu(
            this ISolutionWrapper solution)
        {
            var konfiguracja = Konfiguracja.GetInstance(solution);

            if (konfiguracja != null)
            {
                var powiazanie =
                    konfiguracja?.PowiazaniaProjektowTestowych
                    ?.FirstOrDefault(o => o.NazwaProjektuTestowego == solution.AktualnyProjekt.Nazwa);

                if (powiazanie != null)
                {
                    return solution.Projekty.Single(o => o.Nazwa == powiazanie.NazwaProjektu);
                }
            }

            var nazwaSzukanegoProjektu =
                solution.AktualnyProjekt.Nazwa.Replace(".Tests", "");
            return solution.SzukajProjektuWgNazwy(nazwaSzukanegoProjektu);
        }
    }
}
