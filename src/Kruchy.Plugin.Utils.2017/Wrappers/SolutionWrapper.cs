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
                return new FileWrapper(dte.ActiveDocument);
            }
        }

        public IProjectWrapper CurrentProject
        {
            get
            {
                var currentFile = CurrentFile;
                if (currentFile == null)
                    return null;

                return currentFile.Project;
            }
        }

        public IDocumentWrapper CurenctDocument
        {
            get
            {
                if (dte.ActiveDocument == null)
                    return null;

                var textDoc = (TextDocument)dte.ActiveDocument.Object("TextDocument");
                return new DocumentWrapper(textDoc);
            }
        }

        public DTE2 DTE { get { return dte; } }

        public IList<IProjectWrapper> Projects
        {
            get
            {
                var result = new List<IProjectWrapper>();
                var solution = dte.Solution;
                for (int i = 1; i <= solution.Count; i++)
                {
                    var p = solution.Projects.Item(i);
                    if (p.FullName.ToLower().EndsWith(".csproj"))
                        result.Add(new ProjectWrapper(p));
                    else
                        FindProject(p, result);
                }
                return result;
            }
        }

        private void FindProject(Project p, List<IProjectWrapper> result)
        {
            for (int i = 1; i <= p.ProjectItems.Count; i++)
            {
                var item = p.ProjectItems.Item(i);
                FindProjectsInProjectItem(item, result);
            }
        }

        private void FindProjectsInProjectItem(
            ProjectItem pi,
            List<IProjectWrapper> result)
        {
            var itemObject = pi.Object as Project;
            if (itemObject == null)
                return;
            if (itemObject.FullName.ToLower().EndsWith(".csproj"))
                result.Add(new ProjectWrapper(itemObject));
            else
            {
                for (int i = 1; i <= itemObject.ProjectItems.Count; i++)
                {
                    var item = itemObject.ProjectItems.Item(i);
                    FindProjectsInProjectItem(item, result);
                }
            }
        }

        public ProjectWrapper ZnajdzProjktDlaPliku(string nazwa)
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