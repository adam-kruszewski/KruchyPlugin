using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using System.Linq;

namespace Kruchy.Plugin.Akcje.Utils
{
    public static class SolutionWrapperExtensions
    {
        public static IProjectWrapper SzukajProjektuTestowego(
            this ISolutionWrapper solution)
        {
            var konfiguracja = Konfiguracja.GetInstance(solution);

            if (konfiguracja != null)
            {
                var powiazanie =
                    konfiguracja?.PowiazaniaProjektowTestowych
                    ?.FirstOrDefault(o => o.NazwaProjektu == solution.CurrentProject.Name);

                if (powiazanie != null)
                    return solution.Projects.Single(o => o.Name == powiazanie.NazwaProjektuTestowego);
            }

            var nazwaSzukanegoProjektu = solution.CurrentProject.Name + ".Tests";
            var projektTestow = solution.SzukajProjektuWgNazwy(nazwaSzukanegoProjektu);
            return projektTestow;
        }

        public static IProjectWrapper SzukajProjektuModulu(
            this ISolutionWrapper solution)
        {
            var konfiguracja = Konfiguracja.GetInstance(solution);

            if (konfiguracja != null)
            {
                var powiazanie =
                    konfiguracja?.PowiazaniaProjektowTestowych
                    ?.FirstOrDefault(o => o.NazwaProjektuTestowego == solution.CurrentProject.Name);

                if (powiazanie != null)
                {
                    return solution.Projects.Single(o => o.Name == powiazanie.NazwaProjektu);
                }
            }

            var nazwaSzukanegoProjektu =
                solution.CurrentProject.Name.Replace(".Tests", "");
            return solution.SzukajProjektuWgNazwy(nazwaSzukanegoProjektu);
        }
    }
}
