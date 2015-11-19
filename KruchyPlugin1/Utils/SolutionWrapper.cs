using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KruchyCompany.KruchyPlugin1.Utils
{
    class SolutionWrapper
    {
        private readonly DTE dte;

        public SolutionWrapper(DTE dte)
        {
            this.dte = dte;
        }

        public string Nazwa { get; set; }

        public IList<ProjektWrapper> Projekty { get; set; }

        public void UstawSieNaPliku(string nazwa)
        {

        }

        public void UstawSieNaKatalogu (string nazwa)
        {

        }

        public ProjektWrapper ZnajdzProjktDlaPliku(string nazwa)
        {
            throw new System.NotImplementedException();
        }

        public PlikWrapper AktualnyPlik
        {
            get { return new PlikWrapper(dte.ActiveDocument); }
        }
    }
}
