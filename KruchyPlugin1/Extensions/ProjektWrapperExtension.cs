using System.IO;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Extensions
{
    static class ProjektWrapperExtension
    {
        public static string SciezkaDoService(this ProjektWrapper projekt)
        {
            return Path.Combine(projekt.SciezkaDoKatalogu, "Services");
        }

        public static string SciezkaDoServiceImpl(this ProjektWrapper projekt)
        {
            return Path.Combine(SciezkaDoService(projekt), "Impl");
        }

        public static string SciezkaDoUnitTests(this ProjektWrapper projekt)
        {
            return Path.Combine(projekt.SciezkaDoKatalogu, "Unit");
        }

        public static bool Testowy(this ProjektWrapper projekt)
        {
            return projekt.Nazwa.ToLower().EndsWith(".tests");
        }

        public static bool Modul(this ProjektWrapper projekt)
        {
            return !projekt.Nazwa.ToLower().EndsWith(".tests");
        }
    }
}
