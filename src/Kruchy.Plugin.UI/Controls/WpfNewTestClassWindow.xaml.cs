using Kruchy.Plugin.Akcje.Interfejs;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;
using System;
using System.Windows;

namespace Kruchy.Plugin.UI.Controls
{
    /// <summary>
    /// Interaction logic for WpfNewTestClassWindow.xaml
    /// </summary>
    public partial class WpfNewTestClassWindow : INewTestClassWindow
    {
        public WpfNewTestClassWindow()
        {
            InitializeComponent();
        }

        private void FillTestType(Konfiguracja konfiguracja)
        {
            foreach (KlasaTestowa klasaTestowa in konfiguracja.KlasyTestowe())
                ComboTestType.Items.Add(klasaTestowa.Nazwa);

            if (ComboTestType.Items.Count > 0)
                ComboTestType.SelectedIndex = 0;
        }

        public string ClassName
        {
            get { return TestClassName.Text; }
            set { TestClassName.Text = value; }
        }

        public string TestType => ComboTestType.SelectedItem?.ToString();

        string INewTestClassWindow.TestedInterface
        {
            get { return TestedInterface.Text; }
            set { TestedInterface.Text = value; }
        }

        string INewTestClassWindow.Directory => Directory.Text;

        public Konfiguracja Konfiguracja
        {
            set
            {
                FillTestType(value);
            }
        }

        public bool Cancelled { get; private set; } = true;

        public Func<string> GetDirectoryFromModuleFunc { private get; set; }

        private void addButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!Validate())
                return;

            Cancelled = false;

            Close();
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(TestType))
            {
                MessageBox.Show("Brak rodzaju testów");
                return false;
            }
            if (string.IsNullOrEmpty(ClassName))
            {
                MessageBox.Show("Brak nazwy klasy testów");
                return false;
            }

            return true;
        }

        private void cancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Cancelled = true;
            this.Close();
        }

        private void CopyDirectoryFromModule_Click(object sender, RoutedEventArgs e)
        {
            if (GetDirectoryFromModuleFunc != null)
            {
                Directory.Text = GetDirectoryFromModuleFunc();
            }
        }
    }
}
