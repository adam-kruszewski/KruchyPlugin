using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Kruchy.Plugin.Utils.Wrappers;
using Moq;

namespace Kruchy.Plugin.Akcje.Tests.WrappersMocks
{
    public class SolutionWrapper : ISolutionWrapper
    {
        IDocumentWrapper dokument;
        private IList<IProjectWrapper> projekty;

        public SolutionWrapper()
        {
            AktualnyPlik = new Mock<IFileWrapper>().Object;
        }

        public SolutionWrapper(string aktualnaZawartosc) : this()
        {
            dokument = new DokumentWrapper(aktualnaZawartosc);
            projekty = new List<IProjectWrapper>();
        }

        public SolutionWrapper(
            IProjectWrapper projekt,
            string aktualnaZawartosc)
            : this(aktualnaZawartosc)
        {
            DodajProjekt(projekt);
            AktualnyProjekt = projekt;
        }

        public void DodajProjekt(IProjectWrapper projekt)
        {
            projekty.Add(projekt);
        }

        public string PelnaNazwa { get { return "A"; } }

        public string Nazwa { get { return "A"; } }

        public string Katalog { get { return "A"; } }

        public IFileWrapper AktualnyPlik { get; set; }

        public IProjectWrapper AktualnyProjekt { get; set; }

        public IDocumentWrapper AktualnyDokument { get { return dokument; } }

        public IList<IProjectWrapper> Projekty { get { return projekty.ToList(); } }

        public IProjectWrapper ZnajdzProjekt(string nazwa)
        {
            throw new NotImplementedException();
        }

        public void OtworzPlik(string sciezka)
        {
            AktualnyProjekt = Projekty.SingleOrDefault(o => ZawieraPlik(o, sciezka));
            AktualnyPlik = AktualnyProjekt.Files.SingleOrDefault(o => o.FullPath == sciezka);
            dokument = new DokumentWrapper(File.ReadAllText(sciezka, Encoding.UTF8));
        }

        private bool ZawieraPlik(IProjectWrapper projekt, string sciezka)
        {
            return projekt.Files.Any(o => o.FullPath == sciezka);
        }
    }
}
