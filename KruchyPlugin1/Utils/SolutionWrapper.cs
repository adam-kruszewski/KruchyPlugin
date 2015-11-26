using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace KruchyCompany.KruchyPlugin1.Utils
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

        public ProjektWrapper ZnajdzProjekt(string nazwa)
        {
            var l = Projekty.ToList();
            return l.Where(o => o.Nazwa == nazwa).FirstOrDefault();
        }

        public PlikWrapper OtworzPlik(PlikWrapper plik)
        {
            return null;
        }

        public PlikWrapper OtworzPlik(string sciezka)
        {
            UIHierarchy UIH = dte.ToolWindows.SolutionExplorer;
            UIHierarchyItem UIHItem = UIH.UIHierarchyItems.Item(1);

            if (!UIHItem.UIHierarchyItems.Expanded)
                UIHItem.UIHierarchyItems.Expanded = true;
            Podrozuj(UIHItem.UIHierarchyItems);
            return null;
        }

        private void Podrozuj(UIHierarchyItems uIHierarchyItems)
        {
            if (!uIHierarchyItems.Expanded)
                uIHierarchyItems.Expanded = true;

            for (int i = 1; i <= uIHierarchyItems.Count; i++)
            {
                var item = uIHierarchyItems.Item(i);
                Podrozuj(item.UIHierarchyItems);
            }
        }

    }
}
