﻿using System.IO;

namespace Kruchy.Plugin.Utils.Wrappers
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