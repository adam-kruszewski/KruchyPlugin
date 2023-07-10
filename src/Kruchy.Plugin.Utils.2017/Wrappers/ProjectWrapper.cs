using System.Collections.Generic;
using System.IO;
using EnvDTE;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Utils._2017.Wrappers
{
#pragma warning disable VSTHRD010
    public class ProjectWrapper : IProjectWrapper
    {
        private readonly Project project;

        public ProjectWrapper(Project project)
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
                var fileList = new List<IFileWrapper>();
                foreach (ProjectItem pi in project.ProjectItems)
                {
                    FindFiles(fileList, pi);
                }
                return fileList.ToArray();
            }
        }

        private void FindFiles(
            List<IFileWrapper> fileList,
            ProjectItem pi)
        {
            if (IsFileInProject(pi))
                fileList.Add(new FileWrapper(pi));
            else
            {
                foreach (ProjectItem projectItemChild in pi.ProjectItems)
                    FindFiles(fileList, projectItemChild);
            }
        }

        private bool IsFileInProject(ProjectItem pi)
        {
            if (pi.FileCount != 1)
                return false;
            var fi = new FileInfo(pi.FileNames[0]);
            if (Directory.Exists(fi.FullName))
                return false;
            return true;
        }

        public IFileWrapper AddFile(string path)
        {
            return new FileWrapper(
                project.ProjectItems.AddFromFile(path));
        }

        public bool ContainsNamespace(string namespaceName)
        {
            return namespaceName.ToLower().StartsWith(Name.ToLower());
        }

        public IEnumerable<string> GetFilesFromNamespace(string namespaceName)
        {
            var relativeNamespace =
                namespaceName.Substring(Name.Length + 1);
            string fileLocationPath = BuildPath(relativeNamespace);
            foreach (var file in Files)
            {
                if (file.FullPath.ToLower()
                    .StartsWith(fileLocationPath.ToLower()))
                    yield return file.FullPath;
            }
        }

        private string BuildPath(string realativeNamespace)
        {
            var result = DirectoryPath;
            var parts = realativeNamespace.Split('.');
            foreach (var c in parts)
            {
                if (Directory.Exists(result))
                    result = System.IO.Path.Combine(result, c);
                else
                    result += "." + c;
            }

            return result;
        }
    }
}