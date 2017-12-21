using System.IO;
using Kruchy.Plugin.Utils.Wrappers;

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

        public static string SciezkaDoIntegrationTests(this ProjektWrapper projekt)
        {
            return Path.Combine(projekt.SciezkaDoKatalogu, "Integration");
        }

        public static bool Testowy(this ProjektWrapper projekt)
        {
            return projekt.Nazwa.ToLower().EndsWith(".tests");
        }

        public static bool Modul(this ProjektWrapper projekt)
        {
            return !projekt.Nazwa.ToLower().EndsWith(".tests");
        }

        public static string KatalogSharedViews(this ProjektWrapper projekt)
        {
            return Path.Combine(
                projekt.SciezkaDoKatalogu,
                "Views",
                "Shared");
        }

        public static string SciezkaDoPlikuWShared(this ProjektWrapper projekt, string nazwa)
        {
            return Path.Combine(
                projekt.KatalogSharedViews(),
                nazwa);
        }
    }
}
