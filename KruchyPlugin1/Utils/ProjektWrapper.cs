using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public PlikWrapper[] Pliki { get; set; }

        public void DodajPlik(string katalog, string nazwaPliku, bool implementacja)
        {
            throw new System.NotImplementedException();
        }

        public void DodajPlik(string sciezka)
        {
            project.ProjectItems.AddFromFile(sciezka);
        }
    }
}