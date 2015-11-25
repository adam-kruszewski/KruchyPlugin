using System.IO;
using EnvDTE;

namespace KruchyCompany.KruchyPlugin1.Utils
{
    public class PlikWrapper
    {
        private Document document;
        private ProjectItem projectItem;

        public PlikWrapper(EnvDTE.Document document)
        {
            this.document = document;
        }

        public PlikWrapper(ProjectItem projectItem)
        {
            this.projectItem = projectItem;
        }

        public string Nazwa
        {
            get
            {
                if (document != null)
                    return document.Name;
                else
                {
                    return projectItem.FileNames[0];
                }
            }
        }

        public string NazwaBezRozszerzenia
        {
            get
            {
                var index = Nazwa.LastIndexOf('.');
                if (index < 0)
                    return Nazwa;
                else
                    return Nazwa.Substring(0, index);
            }
        }

        public string SciezkaPelna
        {
            get
            {
                if (document != null)
                    return document.FullName;
                else
                {
                    var fi = new FileInfo(projectItem.FileNames[0]);
                    return fi.FullName;
                }
            }
        }

        public string Katalog
        {
            get
            {
                var fi = new FileInfo(SciezkaPelna);
                return fi.DirectoryName;
            }
        }

        public string SciezkaWzgledna
        {
            get
            {
                var p = SciezkaPelna;

                var katalogProjektu = Projekt.SciezkaDoKatalogu;
                p = p.Replace(katalogProjektu, "");
                return p;
            }
        }

        public ProjektWrapper Projekt
        {
            get
            {
                if (projectItem != null)
                    return new ProjektWrapper(projectItem.ContainingProject);
                else
                    return new ProjektWrapper(document.ProjectItem.ContainingProject);
            }
        }

        public DokumentWrapper Dokument
        {
            get
            {
                var textDocument = (TextDocument)document.Object("TextDocument");
                return new DokumentWrapper(textDocument);
            }
        }
    }
}
