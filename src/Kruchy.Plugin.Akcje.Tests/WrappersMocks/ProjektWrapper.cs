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
            pliki = new List<IPlikWrapper>();
        }

        public string Name { get; set; }

        public string Path => System.IO.Path.Combine(DirectoryPath, Name) + ".csproj";

        public string DirectoryPath { get; private set; }

        public void DodajPlik(IPlikWrapper plik)
        {
            pliki.Add(plik);
        }

        private List<IPlikWrapper> pliki { get; set; }

        public IPlikWrapper[] Files { get { return pliki.ToArray(); } }

        public IEnumerable<string> GetFilesFromNamespace(string nazwaNamespace)
        {
            throw new NotImplementedException();
        }

        public IPlikWrapper AddFile(string sciezka)
        {
            var plik = new PlikWrapper(sciezka);

            plik.Projekt = this;

            pliki.Add(plik);

            return plik;
        }

        public IPlikWrapper DodajPustyPlik(string nazwaWzgledna)
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
