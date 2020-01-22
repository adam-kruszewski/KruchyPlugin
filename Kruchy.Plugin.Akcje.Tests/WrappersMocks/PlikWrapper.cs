using System;
using System.IO;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Tests.WrappersMocks
{
    class PlikWrapper : IPlikWrapper
    {
        private string sciezkaPelna;

        public PlikWrapper(
            string nazwa,
            string katalog,
            IProjektWrapper projekt,
            string zawartosc = null)
        {
            Nazwa = nazwa;
            Katalog = Path.Combine(projekt.SciezkaDoKatalogu, katalog, nazwa);
            SciezkaWzgledna = Path.Combine(katalog, nazwa);
            Projekt = projekt;
            (Projekt as ProjektWrapper).DodajPlik(this);
            sciezkaPelna = Path.Combine(Katalog, Nazwa);
        }

        public PlikWrapper(string sciezkaPelna)
        {
            var fi = new FileInfo(sciezkaPelna);
            Nazwa = fi.Name;
            Katalog = fi.DirectoryName;
            this.sciezkaPelna = fi.FullName;
        }

        public string Nazwa { get; set; }

        public string NazwaBezRozszerzenia => throw new NotImplementedException();

        public string SciezkaPelna { get { return sciezkaPelna; } }

        public string Katalog { get; set; }

        public string SciezkaWzgledna { get; set; }

        public IProjektWrapper Projekt { get; set; }

        public IDokumentWrapper Dokument => throw new NotImplementedException();
    }
}
