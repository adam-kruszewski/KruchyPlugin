using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KruchyCompany.KruchyPlugin1.Utils
{
    public class SolutionWrapper
    {
        private readonly DTE dte;

        public SolutionWrapper(DTE dte)
        {
            this.dte = dte;
        }

        public string Nazwa { get; set; }

        public IList<ProjektWrapper> Projekty
        {
            get
            {
                var wynik = new List<ProjektWrapper>();
                var solution = dte.Solution;
                for (int i = 1; i <= solution.Count; i++)
                {
                    var p = solution.Projects.Item(i);
                    wynik.Add(new ProjektWrapper(p));
                }
                return wynik;
            }
        }

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

        public ProjektWrapper ZnajdzProjekt(string nazwa)
        {
            return Projekty.Where(o => o.Nazwa == nazwa).FirstOrDefault();
        }
    }
}
