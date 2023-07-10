using System.IO;
using EnvDTE;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Utils._2017.Wrappers
{
#pragma warning disable VSTHRD010
    public class FileWrapper : IFileWrapper
    {
        private Document document;
        private ProjectItem projectItem;

        public FileWrapper(EnvDTE.Document document)
        {
            this.document = document;
        }

        public FileWrapper(ProjectItem projectItem)
        {
            this.projectItem = projectItem;
        }

        public string Name
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

        public string NameWithoutExtension
        {
            get
            {
                var index = Name.LastIndexOf('.');
                if (index < 0)
                    return Name;
                else
                    return Name.Substring(0, index);
            }
        }

        public string FullPath
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

        public string Directory
        {
            get
            {
                var fi = new FileInfo(FullPath);
                return fi.DirectoryName;
            }
        }

        public string RelativePath
        {
            get
            {
                var p = FullPath;

                var projectDirectory = Project.DirectoryPath;
                p = p.Replace(projectDirectory, "");
                return p;
            }
        }

        public IProjectWrapper Project
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

        public IDocumentWrapper Document
        {
            get
            {
                var textDocument = (TextDocument)document.Object("TextDocument");
                return new DocumentWrapper(textDocument);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Name, Directory);
        }
    }
}
