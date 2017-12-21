using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Kruchy.Plugin.Utils.Wrappers
{
    public class SolutionWrapper
    {
        private readonly DTE2 dte;

        public SolutionWrapper(DTE2 dte)
        {
            this.dte = dte;
        }

        public string PelnaNazwa
        {
            get { return dte.Solution.FullName; }
        }

        public string Nazwa
        {
            get
            {
                var fi = new FileInfo(PelnaNazwa);
                return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
            }
        }

        public string Katalog
        {
            get
            {
                var fi = new FileInfo(PelnaNazwa);
                return fi.DirectoryName;
            }
        }

        public PlikWrapper AktualnyPlik
        {
            get { return new PlikWrapper(dte.ActiveDocument); }
        }

        public ProjektWrapper AktualnyProjekt
        {
            get
            {
                var plik = AktualnyPlik;
                if (plik == null)
                    return null;

                return plik.Projekt;
            }
        }

        public DokumentWrapper AktualnyDokument
        {
            get
            {
                if (dte.ActiveDocument == null)
                    return null;

                var textDoc = (TextDocument)dte.ActiveDocument.Object("TextDocument");
                return new DokumentWrapper(textDoc);
            }
        }

        public DTE2 DTE { get { return dte; } }

        public IList<ProjektWrapper> Projekty
        {
            get
            {
                var wynik = new List<ProjektWrapper>();
                var solution = dte.Solution;
                for (int i = 1; i <= solution.Count; i++)
                {
                    var p = solution.Projects.Item(i);
                    if (p.FullName.ToLower().EndsWith(".csproj"))
                        wynik.Add(new ProjektWrapper(p));
                    else
                        SzukajProjektow(p, wynik);
                }
                return wynik;
            }
        }

        private void SzukajProjektow(Project p, List<ProjektWrapper> wynik)
        {
            for (int i = 1; i <= p.ProjectItems.Count; i++)
            {
                var item = p.ProjectItems.Item(i);
                SzukajProjektowWProjectItem(item, wynik);
            }
        }

        private void SzukajProjektowWProjectItem(
            ProjectItem pi,
            List<ProjektWrapper> wynik)
        {
            var itemObject = pi.Object as Project;
            if (itemObject == null)
                return;
            if (itemObject.FullName.ToLower().EndsWith(".csproj"))
                wynik.Add(new ProjektWrapper(itemObject));
            else
            {
                for (int i = 1; i <= itemObject.ProjectItems.Count; i++)
                {
                    var item = itemObject.ProjectItems.Item(i);
                    SzukajProjektowWProjectItem(item, wynik);
                }
            }
        }

        public ProjektWrapper ZnajdzProjktDlaPliku(string nazwa)
        {
            throw new System.NotImplementedException();
        }

        public ProjektWrapper ZnajdzProjekt(string nazwa)
        {
            var l = Projekty.ToList();
            return l.Where(o => o.Nazwa == nazwa).FirstOrDefault();
        }

        public void OtworzPlik(string sciezka)
        {
            new SolutionExplorerWrapper(this).OtworzPlik(sciezka);
        }
    }
}
