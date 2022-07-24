using Kruchy.Plugin.Utils.Wrappers;
using System.Collections.ObjectModel;

namespace Kruchy.Plugin.UI.Controls.Models
{
    public class PlaceInSolutionItem
    {
        public string Path { get; set; }

        public string RelativePath
        {
            get
            {
                var result = Path.Replace(Project.DirectoryPath, "");

                return result.TrimStart(new[] { '\\', '/' });
            }
        }

        public IProjectWrapper Project { get; set; }

        public string Title { get; set; }

        public ObservableCollection<PlaceInSolutionItem> Items { get; set; }

        public bool Expanded { get; set; } = true;

        public PlaceInSolutionItem()
        {
            Items = new ObservableCollection<PlaceInSolutionItem>();
        }
    }
}
