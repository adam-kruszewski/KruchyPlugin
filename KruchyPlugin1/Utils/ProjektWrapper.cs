using System.Collections.Generic;
using System.IO;
using EnvDTE;

namespace KruchyCompany.KruchyPlugin1.Utils
{
    public class ProjektWrapper
    {
        private readonly Project project;

        public ProjektWrapper(Project project)
        {
            this.project = project;
        }

        //private readonly 

        public string Nazwa
        {
            get
            {
                return project.Name;
            }
        }

        public string Sciezka
        {
            get { return project.FullName; }
        }

        public string SciezkaDoKatalogu
        {
            get
            {
                FileInfo f = new FileInfo(Sciezka);
                return f.DirectoryName;
            }
        }

        public PlikWrapper[] Pliki
        {
            get
            {
                var listaPlikow = new List<PlikWrapper>();
                foreach (ProjectItem pi in project.ProjectItems)
                {
                    SzukajPlikow(listaPlikow, pi);
                }
                return listaPlikow.ToArray();
            }
        }

        private void SzukajPlikow(
            List<PlikWrapper> listaPlikow,
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

        public PlikWrapper DodajPlik(string sciezka)
        {
            return new PlikWrapper(
                project.ProjectItems.AddFromFile(sciezka));
        }
    }
}