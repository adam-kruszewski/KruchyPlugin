using System.IO;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Utils.Extensions
{
    public static class ProjektWrapperExtension
    {
        public static string SciezkaDoService(this IProjektWrapper projekt)
        {
            return Path.Combine(projekt.SciezkaDoKatalogu, "Services");
        }

        public static string SciezkaDoServiceImpl(this IProjektWrapper projekt)
        {
            return Path.Combine(SciezkaDoService(projekt), "Impl");
        }

        public static string SciezkaDoUnitTests(this IProjektWrapper projekt)
        {
            return Path.Combine(projekt.SciezkaDoKatalogu, "Unit");
        }

        public static string SciezkaDoIntegrationTests(this IProjektWrapper projekt)
        {
            return Path.Combine(projekt.SciezkaDoKatalogu, "Integration");
        }

        public static bool Testowy(this IProjektWrapper projekt)
        {
            return projekt.Nazwa.ToLower().EndsWith(".tests");
        }

        public static bool Modul(this IProjektWrapper projekt)
        {
            return !projekt.Nazwa.ToLower().EndsWith(".tests");
        }

        public static string KatalogSharedViews(this IProjektWrapper projekt)
        {
            return Path.Combine(
                projekt.SciezkaDoKatalogu,
                "Views",
                "Shared");
        }

        public static string SciezkaDoPlikuWShared(this IProjektWrapper projekt, string nazwa)
        {
            return Path.Combine(
                projekt.KatalogSharedViews(),
                nazwa);
        }
    }
}
