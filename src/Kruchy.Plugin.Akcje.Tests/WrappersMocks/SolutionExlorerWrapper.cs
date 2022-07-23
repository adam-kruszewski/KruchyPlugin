﻿using System.Collections.Generic;
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
            if (solution.AktualnyDokument != null)
                PoprzednieZawartosciDokumentow.Add(
                    solution.AktualnyDokument.GetContent());
            solution.OtworzPlik(sciezka);
        }

        public void OpenFile(IPlikWrapper plik)
        {
            OpenFile(plik.SciezkaPelna);
        }

        public void SelectPath(string sciezka)
        {
        }
    }
}