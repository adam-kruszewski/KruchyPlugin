using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Utils._2017.Wrappers
{
#pragma warning disable VSTHRD010
    public class SolutionWrapper : ISolutionWrapper
    {
        private readonly DTE2 dte;

        public SolutionWrapper(DTE2 dte)
        {
            this.dte = dte;
        }

        public string FullName
        {
            get { return dte.Solution.FullName; }
        }

        public string Name
        {
            get
            {
                var fi = new FileInfo(FullName);
                return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
            }
        }

        public string Directory
        {
            get
            {
                var fi = new FileInfo(FullName);
                return fi.DirectoryName;
            }
        }

        public IFileWrapper CurrentFile
        {
            get
            {
                try
                {
                    if (dte.ActiveDocument == null)
                        return null;
                }catch (ArgumentException)
                {
                    return null;
                }
                return new PlikWrapper(dte.ActiveDocument);
            }
        }

        public IProjectWrapper CurrentProject
        {
            get
            {
                var plik = CurrentFile;
                if (plik == null)
                    return null;

                return plik.Project;
            }
        }

        public IDocumentWrapper CurenctDocument
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

        public IList<IProjectWrapper> Projects
        {
            get
            {
                var wynik = new List<IProjectWrapper>();
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

        private void SzukajProjektow(Project p, List<IProjectWrapper> wynik)
        {
            for (int i = 1; i <= p.ProjectItems.Count; i++)
            {
                var item = p.ProjectItems.Item(i);
                SzukajProjektowWProjectItem(item, wynik);
            }
        }

        private void SzukajProjektowWProjectItem(
            ProjectItem pi,
            List<IProjectWrapper> wynik)
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

        public IProjectWrapper FindProject(string nazwa)
        {
            var l = Projects.ToList();
            return l.Where(o => o.Name == nazwa).FirstOrDefault();
        }
    }
}