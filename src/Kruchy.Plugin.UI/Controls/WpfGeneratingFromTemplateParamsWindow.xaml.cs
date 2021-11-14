using Kruchy.Plugin.Akcje.Interfejs;
using Kruchy.Plugin.UI.Controls.Models;
using Kruchy.Plugin.Utils.UI;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kruchy.Plugin.UI.Controls
{
    /// <summary>
    /// Interaction logic for WpfGeneratingFromTemplateParamsWindow.xaml
    /// </summary>
    public partial class WpfGeneratingFromTemplateParamsWindow : IGeneratingFromTemplateParamsWindow
    {
        private static readonly string[] DirectoriesToOmit = { "bin", "obj" };

        public WpfGeneratingFromTemplateParamsWindow()
        {
            InitializeComponent();

            //Models.PlaceInSolutionItem root = new Models.PlaceInSolutionItem() { Path = "Menu" };
            //Models.PlaceInSolutionItem childItem1 = new Models.PlaceInSolutionItem() { Path = "Child item #1" };
            //childItem1.Items.Add(new Models.PlaceInSolutionItem() { Path = "Child item #1.1" });
            //childItem1.Items.Add(new Models.PlaceInSolutionItem() { Path = "Child item #1.2" });
            //root.Items.Add(childItem1);
            //root.Items.Add(new Models.PlaceInSolutionItem() { Path = "Child item #2" });
            //TreeViewSelectDirectory.Items.Add(root);
        }

        public IEnumerable<IProjektWrapper> Projects
        {
            set
            {
                foreach (var project in value.OrderBy(o => o.Nazwa))
                    AddProjectTreeItem(project);
            }
        }

        private void AddProjectTreeItem(IProjektWrapper project)
        {
            var projectItem = new PlaceInSolutionItem
            {
                Title = project.Nazwa,
                Project = project,
                Path = project.SciezkaDoKatalogu,
            };

            TreeViewSelectDirectory.Items.Add(projectItem);

            AddDirectories(project, project.SciezkaDoKatalogu, projectItem);
        }

        private void AddDirectories(
            IProjektWrapper project,
            string startingDirectory,
            PlaceInSolutionItem parentProjectItem)
        {
            foreach (var directory in System.IO.Directory.GetDirectories(startingDirectory))
            {
                var directoryInfo = new DirectoryInfo(directory);

                if (DirectoriesToOmit.Contains(directoryInfo.Name))
                    continue;

                var projectItem = new PlaceInSolutionItem
                {
                    Project = project,
                    Path = directory
                };

                projectItem.Title = projectItem.RelativePath;

                parentProjectItem.Items.Add(projectItem);

                AddDirectories(project, directory, projectItem);
            }
        }

        public IProjektWrapper Project
        {
            set
            {
                Projects = new[] { value };
            }
        }

        public string Directory { get; set; }
        public IProjektWrapper SelectedProject { get; private set; }

        private void cancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void addButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (TreeViewSelectDirectory.SelectedItem == null)
            {
                UIObjects.ShowMessageBox(null, "Nie wybrano katalogu");
                return;
            }

            Directory = (TreeViewSelectDirectory.SelectedItem as PlaceInSolutionItem)?.Path;
            SelectedProject = (TreeViewSelectDirectory.SelectedItem as PlaceInSolutionItem)?.Project;

            Close();
        }
    }
}
