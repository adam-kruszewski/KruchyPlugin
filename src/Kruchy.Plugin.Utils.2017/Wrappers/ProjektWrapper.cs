using System.Collections.Generic;
using System.IO;
using EnvDTE;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Utils._2017.Wrappers
{
#pragma warning disable VSTHRD010
    public class ProjektWrapper : IProjectWrapper
    {
        private readonly Project project;

        public ProjektWrapper(Project project)
        {
            this.project = project;
        }

        public string Name { get { return project.Name; } }

        public string Path { get { return project.FullName; } }

        public string DirectoryPath
        {
            get
            {
                FileInfo f = new FileInfo(Path);
                return f.DirectoryName;
            }
        }

        public IFileWrapper[] Files
        {
            get
            {
                var listaPlikow = new List<IFileWrapper>();
                foreach (ProjectItem pi in project.ProjectItems)
                {
                    SzukajPlikow(listaPlikow, pi);
                }
                return listaPlikow.ToArray();
            }
        }

        private void SzukajPlikow(
            List<IFileWrapper> listaPlikow,
            ProjectItem pi)
        {
            if (JestPlikiemWProjekcie(pi))
                listaPlikow.Add(new PlikWrapper(pi));
            else
            {
                foreach (ProjectItem piDzieci in pi.ProjectItems)
                    SzukajPlikow(listaPlikow, piDzieci);
            }
        }

        private bool JestPlikiemWProjekcie(ProjectItem pi)
        {
            if (pi.FileCount != 1)
                return false;
            var fi = new FileInfo(pi.FileNames[0]);
            if (Directory.Exists(fi.FullName))
                return false;
            return true;
        }

        public IFileWrapper AddFile(string sciezka)
        {
            return new PlikWrapper(
                project.ProjectItems.AddFromFile(sciezka));
        }

        public bool ContainsNamespace(string nazwaNamespace)
        {
            return nazwaNamespace.ToLower().StartsWith(Name.ToLower());
        }

        public IEnumerable<string> GetFilesFromNamespace(string nazwaNamespace)
        {
            var wzglednyNamespace =
                nazwaNamespace.Substring(Name.Length + 1);
            string sciezkaPolozeniaPlikow = BudujSciezke(wzglednyNamespace);
            foreach (var plik in Files)
            {
                if (plik.FullPath.ToLower()
                    .StartsWith(sciezkaPolozeniaPlikow.ToLower()))
                    yield return plik.FullPath;
            }
        }

        private string BudujSciezke(string wzglednyNamespace)
        {
            var wynik = DirectoryPath;
            var czesci = wzglednyNamespace.Split('.');
            foreach (var c in czesci)
            {
                if (Directory.Exists(wynik))
                    wynik = System.IO.Path.Combine(wynik, c);
                else
                    wynik += "." + c;
            }

            return wynik;
        }
    }
}