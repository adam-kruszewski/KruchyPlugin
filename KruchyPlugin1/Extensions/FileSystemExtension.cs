using System.IO;
using System.Linq;

namespace KruchyCompany.KruchyPlugin1.Extensions
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