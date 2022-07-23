using EnvDTE;
using EnvDTE80;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Linq;

namespace Kruchy.Plugin.Utils._2017.Wrappers
{
    class SelectionWrapper : ISelectionWrapper
    {
        private readonly DTE2 dte;

        public SelectionWrapper(
            DTE2 dte)
        {
            this.dte = dte;
        }

        public ISelectedFolder GetSingleSelectedFolder()
        {
            var selectedItems = dte.SelectedItems;
            if (null != selectedItems)
            {
                if (selectedItems.Count > 1)
                    return null;

                var selectedItem = selectedItems.Item(1);

                var folderName = selectedItem.ProjectItem.Properties.Item("FolderName")?.Value?.ToString();

                if (folderName == null)
                    return null;

                return new SelectedFolder(selectedItem);

            }
            return null;
        }
    }
}
