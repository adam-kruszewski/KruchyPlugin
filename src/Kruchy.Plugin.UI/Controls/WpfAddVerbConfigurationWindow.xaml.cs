using Kruchy.Plugin.Akcje.Interfejs;

namespace Kruchy.Plugin.UI.Controls
{
    /// <summary>
    /// Interaction logic for WpfAddVerbConfigurationWindow.xaml
    /// </summary>
    public partial class WpfAddVerbConfigurationWindow : IAddVerbConfigurationWindow
    {
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

        public string OutputValue
        {
            get => OutputValueControl.Text;
            set => OutputValueControl.Text = value;
        }

        public bool Confirmed { get; private set; }

        public WpfAddVerbConfigurationWindow()
        {
            InitializeComponent();
        }

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
