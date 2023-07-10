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
    public class SolutionExplorerWrapper : ISolutionExplorerWrapper
    {
        private readonly ISolutionWrapper solution;
        private readonly DTE2 dte;

        private UIHierarchy SolutionExplorer
        {
            get
            {
                return dte.ToolWindows.SolutionExplorer;
            }
        }

        private UIHierarchyItem MainNode
        {
            get { return SolutionExplorer.UIHierarchyItems.Item(1); }
        }

        private SolutionExplorerWrapper(ISolutionWrapper solution, DTE2 dte)
        {
            this.solution = solution;
            this.dte = dte;
        }

        public static ISolutionExplorerWrapper GetForSolution(
            ISolutionWrapper solution,
            DTE2 dte)
        {
            return new SolutionExplorerWrapper(solution, dte);
        }

        private IList<UIHierarchyItem> AllNodes()
        {
            var wynik = new List<UIHierarchyItem>();
            FindUiElements();

            VisitNodeAndChildren(MainNode, wynik);
            return wynik;
        }

        private void VisitNodeAndChildren(
            UIHierarchyItem node,
            List<UIHierarchyItem> result)
        {
            result.Add(node);
            for (int i = 1; i <= node.UIHierarchyItems.Count; i++)
            {
                var child = node.UIHierarchyItems.Item(i);
                result.Add(child);
                VisitNodeAndChildren(child, result);
            }
        }

        private void FindUiElements()
        {
            UIHierarchyItem UIHItem = SolutionExplorer.UIHierarchyItems.Item(1);

            var expanding = new List<UIHierarchyItems>();
            if (!UIHItem.UIHierarchyItems.Expanded)
            {
                UIHItem.UIHierarchyItems.Expanded = true;
                expanding.Add(UIHItem.UIHierarchyItems);
            }
            Visit(UIHItem.UIHierarchyItems, expanding);
            foreach (var items in expanding)
                items.Expanded = false;
        }

        private void Visit(
            UIHierarchyItems uIHierarchyItems,
            List<UIHierarchyItems> expanding)
        {
            if (!uIHierarchyItems.Expanded)
            {
                uIHierarchyItems.Expanded = true;
                expanding.Add(uIHierarchyItems);
            }

            for (int i = 1; i <= uIHierarchyItems.Count; i++)
            {
                var item = uIHierarchyItems.Item(i);
                var projectItem = item.Object as ProjectItem;
                Visit(item.UIHierarchyItems, expanding);
            }
        }

        public void OpenFile(string path)
        {
            //ZaladujElementyUI();
            dte.ItemOperations.OpenFile(path);
        }

        public void OpenFile(IFileWrapper fileWrapper)
        {
            //ZaladujElementyUI();
            dte.ItemOperations.OpenFile(fileWrapper.FullPath);
        }

        public void SelectPath(string path)
        {
            var projectNodes = FindProjectNodes(3);
            if (Directory.Exists(path))
            {
                var info = new FileInfo(path);
                var fullName = info.FullName;
                var project =
                    projectNodes
                        .Where(o => ProjectContainsPath(fullName, o))
                            .FirstOrDefault();
                if (project != null)
                {
                    var nodeDirectoryNode = getNodeDirectoryNode(project);
                    var rest = fullName.Substring(nodeDirectoryNode.Length);

                    var parts =
                        rest.Split(
                            Path.DirectorySeparatorChar)
                                .Where(o => o != "")
                                    .ToArray();
                    RevertSelection();

                    UIHierarchyItem foundNode = GetNodeForRest(project, parts);
                    if (foundNode != null)
                    {
                        foundNode.Select(vsUISelectionType.vsUISelectionTypeSetCaret);
                        foundNode.Select(vsUISelectionType.vsUISelectionTypeToggle);
                        foundNode.Select(vsUISelectionType.vsUISelectionTypeSelect);
                        foundNode.UIHierarchyItems.Expanded = true;
                    }
                }
            }
        }

        private void RevertSelection()
        {
            foreach (EnvDTE.UIHierarchyItem item in SolutionExplorer.SelectedItems as object[])
            {
                item.Select(vsUISelectionType.vsUISelectionTypeToggle);
            }
        }

        private bool ProjectContainsPath(string fullPath, UIHierarchyItem hierarchyItem)
        {
            return
                fullPath
                    .ToLower()
                        .StartsWith(getNodeDirectoryNode(hierarchyItem).ToLower());
        }

        private UIHierarchyItem GetNodeForRest(
            UIHierarchyItem projectHierarchyItem,
            string[] parts)
        {
            if (parts.Length == 0)
                return null;
            var nazwa = parts.First().ToLower();
            if (!projectHierarchyItem.UIHierarchyItems.Expanded)
                projectHierarchyItem.UIHierarchyItems.Expanded = true;

            for (int i = 1; i <= projectHierarchyItem.UIHierarchyItems.Count; i++)
            {
                var item = projectHierarchyItem.UIHierarchyItems.Item(i);
                if (item.Name.ToLower() == nazwa)
                {
                    if (parts.Length == 1)
                        return item;
                    else
                    {
                        var newParts = new string[parts.Length - 1];
                        for (int j = 0; j < parts.Length - 1; j++)
                            newParts[j] = parts[j + 1];
                        return GetNodeForRest(item, newParts);
                    }
                }
            }
            return null;
        }

        private string getNodeDirectoryNode(UIHierarchyItem wezelProjektu)
        {
            var project = wezelProjektu.Object as Project;

            if (project == null)
                throw new ApplicationException("Coś poszło nie tak z projektem");

            var info = new FileInfo(project.FullName);
            return info.DirectoryName;
        }

        private string BuildPath(UIHierarchyItem hierarchyItem)
        {
            if (hierarchyItem.Name == "KontaktyWatku")
            {
                var h = hierarchyItem.Collection.Parent as UIHierarchyItem;
            }

            var indexOnPath = new List<UIHierarchyItem>();
            var currentHierarchyItem = hierarchyItem;
            while (currentHierarchyItem != null)
            {
                var p = currentHierarchyItem.Object as Project;
                if (p != null)
                {
                    if (p.FullName.ToLower().EndsWith(".csproj"))
                    {
                        var fi = new FileInfo(p.FullName);
                        var pathElements = new List<string>();
                        pathElements.Add(fi.DirectoryName);
                        indexOnPath.Reverse();
                        foreach (UIHierarchyItem item in indexOnPath)
                            pathElements.Add(item.Name);
                        var result = string.Join("" + Path.PathSeparator, pathElements.ToArray());
                        return result;
                    }
                    else
                        return string.Empty;
                }
                indexOnPath.Add(currentHierarchyItem);
                currentHierarchyItem = currentHierarchyItem.Collection.Parent as UIHierarchyItem;
            }
            return string.Empty;
        }

        private List<UIHierarchyItem> FindProjectNodes(int maxDepth)
        {
            var result = new List<UIHierarchyItem>();
            UIHierarchyItem solutionItem = SolutionExplorer.UIHierarchyItems.Item(1);

            result.AddRange(FindProjectNodes(solutionItem, maxDepth));
            return result;
        }

        private IEnumerable<UIHierarchyItem> FindProjectNodes(
            UIHierarchyItem item, int maxDepth)
        {
            var initialExpanded = item.UIHierarchyItems.Expanded;
            if (!item.UIHierarchyItems.Expanded)
            {
                item.UIHierarchyItems.Expanded = true;
                item.UIHierarchyItems.Expanded = initialExpanded;
            }

            for (int i = 1; i <= item.UIHierarchyItems.Count; i++)
            {
                UIHierarchyItem currentHierarchyItem = item.UIHierarchyItems.Item(i);
                if (IsProject(currentHierarchyItem))
                    yield return currentHierarchyItem;
                else
                {
                    if (maxDepth > 0)
                        foreach (var p in FindProjectNodes(currentHierarchyItem, maxDepth - 1))
                            yield return p;
                }
            }
        }

        private bool IsProject(UIHierarchyItem item)
        {
            var project = item.Object as Project;
            if (project != null && project.FullName.EndsWith(".csproj"))
                return true;
            return false;
        }

        public ISelectionWrapper GetSelection()
        {
            return new SelectionWrapper(this.dte);
        }
    }
}