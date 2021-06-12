using System.IO;
using EnvDTE;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Utils._2017.Wrappers
{
    public class PlikWrapper : IPlikWrapper
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
                FileInfo fi;
                if (document != null)
                    fi = new FileInfo(document.FullName);
                else
                    fi = new FileInfo(projectItem.FileNames[0]);
                return fi.Name;
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

        public IProjektWrapper Projekt
        {
            get
            {
                if (projectItem != null)
                    return new ProjektWrapper(projectItem.ContainingProject);
                else
                    if (document.ProjectItem != null)
                        return new ProjektWrapper(document.ProjectItem.ContainingProject);

                return null;
            }
        }

        public IDokumentWrapper Dokument
        {
            get
            {
                var textDocument = (TextDocument)document.Object("TextDocument");
                return new DokumentWrapper(textDocument);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Nazwa, Katalog);
        }
    }
}
