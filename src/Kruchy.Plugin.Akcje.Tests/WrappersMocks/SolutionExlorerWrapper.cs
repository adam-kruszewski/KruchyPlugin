using System.Collections.Generic;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Tests.WrappersMocks
{
    class SolutionExlorerWrapper : ISolutionExplorerWrapper
    {
        public string OtwartyPlik { get; private set; }

        private readonly SolutionWrapper solution;

        public IList<string> PoprzednieZawartosciDokumentow { get; private set; }

        public SolutionExlorerWrapper(
            ISolutionWrapper solution)
        {
            this.solution = solution as SolutionWrapper;
            PoprzednieZawartosciDokumentow = new List<string>();
        }

        public void OpenFile(string sciezka)
        {
            OtwartyPlik = sciezka;
            if (solution.CurenctDocument != null)
                PoprzednieZawartosciDokumentow.Add(
                    solution.CurenctDocument.GetContent());
            solution.OtworzPlik(sciezka);
        }

        public void OpenFile(IFileWrapper plik)
        {
            OpenFile(plik.FullPath);
        }

        public void SelectPath(string sciezka)
        {
        }

        public ISelectionWrapper GetSelection()
        {
            throw new System.NotImplementedException();
        }
    }
}