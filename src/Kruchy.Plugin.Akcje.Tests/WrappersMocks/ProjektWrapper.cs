using System;
using System.Collections.Generic;
using System.IO;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Tests.WrappersMocks
{
    class ProjektWrapper : IProjectWrapper, IDisposable
    {
        public ProjektWrapper(string krotkaNazwa)
        {
            Name = krotkaNazwa;

            DirectoryPath = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                Guid.NewGuid().ToString());

            Directory.CreateDirectory(DirectoryPath);
            pliki = new List<IFileWrapper>();
        }

        public string Name { get; set; }

        public string Path => System.IO.Path.Combine(DirectoryPath, Name) + ".csproj";

        public string DirectoryPath { get; private set; }

        public void DodajPlik(IFileWrapper plik)
        {
            pliki.Add(plik);
        }

        private List<IFileWrapper> pliki { get; set; }

        public IFileWrapper[] Files { get { return pliki.ToArray(); } }

        public IEnumerable<string> GetFilesFromNamespace(string nazwaNamespace)
        {
            throw new NotImplementedException();
        }

        public IFileWrapper AddFile(string sciezka)
        {
            var plik = new PlikWrapper(sciezka);

            plik.Project = this;

            pliki.Add(plik);

            return plik;
        }

        public IFileWrapper DodajPustyPlik(string nazwaWzgledna)
        {
            var pelnaSciezka = System.IO.Path.Combine(DirectoryPath, nazwaWzgledna);
            File.WriteAllText(pelnaSciezka, "");
            return AddFile(pelnaSciezka);
        }

        public bool ContainsNamespace(string nazwaNamespace)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Directory.Delete(DirectoryPath, true);
        }
    }
}
