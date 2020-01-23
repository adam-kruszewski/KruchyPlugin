using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Kruchy.Plugin.Utils.Wrappers;
using Moq;

namespace Kruchy.Plugin.Akcje.Tests.WrappersMocks
{
    class SolutionWrapper : ISolutionWrapper
    {
        IDokumentWrapper dokument;
        private IList<IProjektWrapper> projekty;

        public SolutionWrapper(string aktualnaZawartosc)
        {
            dokument = new DokumentWrapper(aktualnaZawartosc);
            projekty = new List<IProjektWrapper>();
        }

        public SolutionWrapper(
            IProjektWrapper projekt,
            string aktualnaZawartosc)
            : this(aktualnaZawartosc)
        {
            AktualnyProjekt = projekt;
        }

        public void DodajProjekt(IProjektWrapper projekt)
        {
            projekty.Add(projekt);
        }

        public string PelnaNazwa { get { return "A"; } }

        public string Nazwa { get { return "A"; } }

        public string Katalog { get { return "A"; } }

        public IPlikWrapper AktualnyPlik { get { return new Mock<IPlikWrapper>().Object; } }

        public IProjektWrapper AktualnyProjekt { get; private set; }

        public IDokumentWrapper AktualnyDokument { get { return dokument; } }

        public IList<IProjektWrapper> Projekty { get { return projekty.ToList(); } }

        public IProjektWrapper ZnajdzProjekt(string nazwa)
        {
            throw new NotImplementedException();
        }

        public void OtworzPlik(string sciezka)
        {
            AktualnyProjekt = Projekty.SingleOrDefault(o => ZawieraPlik(o, sciezka));
            dokument = new DokumentWrapper(File.ReadAllText(sciezka, Encoding.UTF8));
        }

        private bool ZawieraPlik(IProjektWrapper projekt, string sciezka)
        {
            return projekt.Pliki.Any(o => o.SciezkaPelna == sciezka);
        }
    }
}
