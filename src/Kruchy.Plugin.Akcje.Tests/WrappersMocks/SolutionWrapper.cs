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
            CurrentFile = new Mock<IFileWrapper>().Object;
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
            CurrentProject = projekt;
        }

        public void DodajProjekt(IProjectWrapper projekt)
        {
            projekty.Add(projekt);
        }

        public string FullName { get { return "A"; } }

        public string Name { get { return "A"; } }

        public string Directory { get { return "A"; } }

        public IFileWrapper CurrentFile { get; set; }

        public IProjectWrapper CurrentProject { get; set; }

        public IDocumentWrapper CurentDocument { get { return dokument; } }

        public IList<IProjectWrapper> Projects { get { return projekty.ToList(); } }

        public IProjectWrapper FindProject(string nazwa)
        {
            throw new NotImplementedException();
        }

        public void OtworzPlik(string sciezka)
        {
            CurrentProject = Projects.SingleOrDefault(o => ZawieraPlik(o, sciezka));
            CurrentFile = CurrentProject.Files.SingleOrDefault(o => o.FullPath == sciezka);
            dokument = new DokumentWrapper(File.ReadAllText(sciezka, Encoding.UTF8));
        }

        private bool ZawieraPlik(IProjectWrapper projekt, string sciezka)
        {
            return projekt.Files.Any(o => o.FullPath == sciezka);
        }
    }
}
