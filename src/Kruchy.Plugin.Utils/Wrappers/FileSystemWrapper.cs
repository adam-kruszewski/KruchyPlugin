using System.IO;

namespace Kruchy.Plugin.Utils.Wrappers
{
    public class FileSystemWrapper
    {
        public static void CreateDirectoryForPathIfNeeded(string sciezka)
        {
            var fi = new FileInfo(sciezka);
            if (!Directory.Exists(fi.Directory.FullName))
                Directory.CreateDirectory(fi.Directory.FullName);
        }
    }
}