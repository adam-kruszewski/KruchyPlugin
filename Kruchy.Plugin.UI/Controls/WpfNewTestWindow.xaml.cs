using Kruchy.Plugin.Akcje.Interfejs;
using System.Windows;

namespace Kruchy.Plugin.UI.Controls
{
    /// <summary>
    /// Interaction logic for WpfNewTestWindow.xaml
    /// </summary>
    public partial class WpfNewTestWindow : INewTestWindow
    {
        public WpfNewTestWindow()
        {
            InitializeComponent();

            addButton.Click += AddButton_Click;
            cancelButton.Click += CancelButton_Click;
        }

        private string selectedClassName;
        private bool selectedAsync;

        private void AddButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            selectedClassName = className.Text;
            selectedAsync = asyncCheckBox.IsChecked.Value;

            if (string.IsNullOrEmpty(selectedClassName))
            {
                MessageBox.Show("Podaj nazwę testu");
            }
            else
                this.Close();
        }

        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        public bool Async => selectedAsync;

        string INewTestWindow.Name => selectedClassName;
    }
}
