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

        public void OtworzPlik(string sciezka)
        {
            OtwartyPlik = sciezka;
            if (solution.AktualnyDokument != null)
                PoprzednieZawartosciDokumentow.Add(
                    solution.AktualnyDokument.DajZawartosc());
            solution.OtworzPlik(sciezka);
        }

        public void OtworzPlik(IPlikWrapper plik)
        {
            OtworzPlik(plik.SciezkaPelna);
        }

        public void UstawSieNaMiejscu(string sciezka)
        {
        }
    }
}