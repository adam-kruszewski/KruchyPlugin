using EnvDTE;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Utils._2017.Wrappers
{
    class SelectedFolder : ISelectedFolder
    {
        public SelectedFolder(
            string name,
            string fullPath)
        {
            Name = name;
            FullPath = fullPath;
        }

        public SelectedFolder(SelectedItem selectedItem) :
            this(
                selectedItem.Name,
                selectedItem.ProjectItem.Properties.Item("FullPath").Value.ToString())
        {
        }

        public string Name { get; private set; }

        public string FullPath { get; private set; }
    }
}
