using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Tests.WrappersMocks
{
    class SolutionExlorerWrapper : ISolutionExplorerWrapper
    {
        public string OtwartyPlik { get; private set; }

        private readonly SolutionWrapper solution;

        public SolutionExlorerWrapper(
            ISolutionWrapper solution)
        {
            this.solution = solution as SolutionWrapper;
        }

        public void OtworzPlik(string sciezka)
        {
            OtwartyPlik = sciezka;
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