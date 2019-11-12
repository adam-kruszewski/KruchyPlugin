using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Tests.WrappersMocks
{
    class SolutionWrapper : ISolutionWrapper
    {
        IDokumentWrapper dokument;

        public SolutionWrapper(string aktualnaZawartosc)
        {
            dokument = new DokumentWrapper(aktualnaZawartosc);
        }

        public string PelnaNazwa { get { return "A"; } }

        public string Nazwa { get { return "A"; } }

        public string Katalog { get { return "A"; } }

        public IPlikWrapper AktualnyPlik { get { throw new NotImplementedException(); } }

        public IProjektWrapper AktualnyProjekt { get { throw new NotImplementedException(); } }

        public IDokumentWrapper AktualnyDokument { get { return dokument; } }

        public IList<IProjektWrapper> Projekty { get { throw new NotImplementedException(); } }

        public IProjektWrapper ZnajdzProjekt(string nazwa)
        {
            throw new NotImplementedException();
        }
    }
}
