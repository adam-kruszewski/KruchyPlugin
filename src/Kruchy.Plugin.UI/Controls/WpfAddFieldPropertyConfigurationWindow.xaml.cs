using Kruchy.Plugin.Akcje.Interfejs;
using System;
using System.Windows.Controls;

namespace Kruchy.Plugin.UI.Controls
{
    /// <summary>
    /// Interaction logic for WpfAddFieldPropertyConfigurationWindow.xaml
    /// </summary>
    public partial class WpfAddFieldPropertyConfigurationWindow : IAddFieldPropertyConfigurationWindow
    {
        public WpfAddFieldPropertyConfigurationWindow()
        {
            InitializeComponent();
        }

        public string Value
        {
            get => ValueControl.Text;
            set => ValueControl.Text = value;
        }

        public string ClassNameRegex
        {
            get => ClassNameRegexControl.Text;
            set => ClassNameRegexControl.Text = value;
        }

        public string FieldPropertyTypeRegex
        {
            get => FieldPropertyTypeRegexControl.Text;
            set => FieldPropertyTypeRegexControl.Text = value;
        }

        public string OutputValue
        {
            get => OutputValueControl.Text;
            set => OutputValueControl.Text = value;
        }

        public bool Confirmed { get; private set; }

        private void cancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void addButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Confirmed = true;

            Close();
        }
    }
}
