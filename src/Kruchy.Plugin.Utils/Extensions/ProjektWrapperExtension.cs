using System.IO;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Utils.Extensions
{
    public static class ProjektWrapperExtension
    {
        public static string SciezkaDoService(this IProjectWrapper projekt)
        {
            return Path.Combine(projekt.DirectoryPath, "Services");
        }

        public static string SciezkaDoServiceImpl(this IProjectWrapper projekt)
        {
            return Path.Combine(SciezkaDoService(projekt), "Impl");
        }

        public static string SciezkaDoUnitTests(this IProjectWrapper projekt)
        {
            return Path.Combine(projekt.DirectoryPath, "Unit");
        }

        public static string SciezkaDoIntegrationTests(this IProjectWrapper projekt)
        {
            return Path.Combine(projekt.DirectoryPath, "Integration");
        }

        public static bool Testowy(this IProjectWrapper projekt)
        {
            return projekt.Name.ToLower().EndsWith(".tests");
        }

        public static bool Modul(this IProjectWrapper projekt)
        {
            return !projekt.Name.ToLower().EndsWith(".tests");
        }

        public static string KatalogSharedViews(this IProjectWrapper projekt)
        {
            return Path.Combine(
                projekt.DirectoryPath,
                "Views",
                "Shared");
        }

        public static string SciezkaDoPlikuWShared(this IProjectWrapper projekt, string nazwa)
        {
            return Path.Combine(
                projekt.KatalogSharedViews(),
                nazwa);
        }
    }
}
