using System.IO;
using System.Linq;

namespace Kruchy.Plugin.Utils.Extensions
{
    public static class FileSystemExtension
    {
        public static void DodajJesliTrzebaKatalogImpl(this string sciezkaDoKatalogu)
        {
            var katalogImpl =
            Directory
                .GetFiles(sciezkaDoKatalogu)
                    .Where(o => o.ToLower() == "impl")
                        .FirstOrDefault();
            if (katalogImpl == null)
                Directory.CreateDirectory(Path.Combine(sciezkaDoKatalogu, "Impl"));
        }
    }
}
