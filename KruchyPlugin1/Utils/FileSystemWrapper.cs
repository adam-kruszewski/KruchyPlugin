using System.IO;

namespace KruchyCompany.KruchyPlugin1.Utils
{
    class FileSystemWrapper
    {
        public static void UtworzKatalogDlaSciezkiJesliTrzeba(string sciezka)
        {
            var fi = new FileInfo(sciezka);
            if (!Directory.Exists(fi.Directory.FullName))
                Directory.CreateDirectory(fi.Directory.FullName);
        }
    }
}