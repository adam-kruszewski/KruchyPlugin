using Kruchy.Plugin.Akcje.Interfejs;
using Kruchy.Plugin.UI.Controls.Models;
using Kruchy.Plugin.Utils.UI;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        public IEnumerable<VariableToFill> VariablesToFill { set => AddVariableControls(value); }

        private void AddVariableControls(IEnumerable<VariableToFill> value)
        {
            var variablesGrid = FindName("VariablesGrid") as Grid;

            var rowIndex = 0;

            foreach (var variableToFill in value)
            {
                variablesGrid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(30) });

                var label =
                    new TextBlock
                    {
                        Text = variableToFill.Name,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                Grid.SetRow(label, rowIndex);
                Grid.SetColumn(label, 0);

                variablesGrid.Children.Add(label);

                var valueTextBox =
                    new TextBox
                    {
                        Height = 25,
                        Text = variableToFill.InitialValue,
                        VerticalAlignment = VerticalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center
                    };

                Grid.SetRow(valueTextBox, rowIndex);
                Grid.SetColumn(valueTextBox, 1);

                variablesGrid.Children.Add(valueTextBox);

                controlsDictionary[variableToFill.Name] = valueTextBox;

                rowIndex++;
            }
        }

        public IDictionary<string, object> VariablesValues
        {
            get => controlsDictionary
                .Select(o => new KeyValuePair<string, object>(o.Key, o.Value.Text))
                    .ToDictionary(o => o.Key, o => o.Value);
        }


        private IDictionary<string, TextBox> controlsDictionary = new Dictionary<string, TextBox>();

        private void cancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (var d in controlsDictionary)

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

            VariablesValues.Clear();

            Close();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newHeight = TreeViewSelectDirectory.Height + (e.NewSize.Height - e.PreviousSize.Height);

            if (newHeight > TreeViewSelectDirectory.MaxHeight)
                newHeight = TreeViewSelectDirectory.MaxHeight;

            TreeViewSelectDirectory.Height = newHeight;
        }
    }
}
