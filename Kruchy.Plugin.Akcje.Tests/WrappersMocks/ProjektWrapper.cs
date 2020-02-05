using System;
using System.Collections.Generic;
using System.IO;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Tests.WrappersMocks
{
    class ProjektWrapper : IProjektWrapper, IDisposable
    {
        public ProjektWrapper(string krotkaNazwa)
        {
            Nazwa = krotkaNazwa;

            SciezkaDoKatalogu = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString());

            Directory.CreateDirectory(SciezkaDoKatalogu);
            pliki = new List<IPlikWrapper>();
        }

        public string Nazwa { get; set; }

        public string Sciezka => Path.Combine(SciezkaDoKatalogu, Nazwa) + ".csproj";

        public string SciezkaDoKatalogu { get; private set; }

        public void DodajPlik(IPlikWrapper plik)
        {
            pliki.Add(plik);
        }

        private List<IPlikWrapper> pliki { get; set; }

        public IPlikWrapper[] Pliki { get { return pliki.ToArray(); } }

        public IEnumerable<string> DajPlikiZNamespace(string nazwaNamespace)
        {
            throw new NotImplementedException();
        }

        public IPlikWrapper DodajPlik(string sciezka)
        {
            var plik = new PlikWrapper(sciezka);

            plik.Projekt = this;

            pliki.Add(plik);

            return plik;
        }

        public IPlikWrapper DodajPustyPlik(string nazwaWzgledna)
        {
            var pelnaSciezka = Path.Combine(SciezkaDoKatalogu, nazwaWzgledna);
            File.WriteAllText(pelnaSciezka, "");
            return DodajPlik(pelnaSciezka);
        }

        public bool NamespaceNalezyDoProjektu(string nazwaNamespace)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Directory.Delete(SciezkaDoKatalogu, true);
        }
    }
}
