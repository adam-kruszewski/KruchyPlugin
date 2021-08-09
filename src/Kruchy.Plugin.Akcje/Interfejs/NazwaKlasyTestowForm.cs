using System;
using System.Windows.Forms;
using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Interfejs
{
    public partial class NazwaKlasyTestowForm : Form
    {
        public string NazwaKlasy { get; private set; }
        public string Rodzaj { get; private set; }
        public string InterfejsTestowany { get; private set; }
        public string Katalog { get; set; }

        private readonly ISolutionWrapper solution;

        public NazwaKlasyTestowForm(ISolutionWrapper solution)
        {
            this.solution = solution;

            InitializeComponent();
            if (solution.AktualnyPlik == null)
                throw new ApplicationException("Brak otwartego pliku");

            WypelnijRodzajKlasyTestowej();

            comboRodzajMigracji.SelectedIndex = 0;

            var nazwaObiektu = solution.NazwaObiektuAktualnegoPliku();

            tbInterfejsTestowany.Text = nazwaObiektu;
            if (!tbInterfejsTestowany.Text.StartsWith("I"))
                tbInterfejsTestowany.Text = "I" + tbInterfejsTestowany.Text;
            tbNazwaKlasyTestowej.Text = nazwaObiektu + "Tests";
            if (nazwaObiektu.StartsWith("I") && char.IsUpper(nazwaObiektu[1]))
                tbNazwaKlasyTestowej.Text = nazwaObiektu.Substring(1) + "Tests";
        }

        private void WypelnijRodzajKlasyTestowej()
        {
            foreach (KlasaTestowa klasaTestowa in
                    Konfiguracja.GetInstance(solution).KlasyTestowe())
                comboRodzajMigracji.Items.Add(klasaTestowa.Nazwa);
        }

        private void buttonGeneruj_Click(object sender, EventArgs e)
        {
            if (!Waliduj())
                return;

            Generuj();
            Close();
        }

        private bool Waliduj()
        {
            if (string.IsNullOrEmpty(tbNazwaKlasyTestowej.Text.Trim()))
            {
                MessageBox.Show("Brak nazwy klasy testowej");
                return false;
            }
            if (comboRodzajMigracji.SelectedIndex == 0)
            {
                if (string.IsNullOrEmpty(tbInterfejsTestowany.Text.Trim()))
                {
                    MessageBox.Show("Brak interfejsu testowanego");
                    return false;
                }
            }
            return true;
        }

        private void Generuj()
        {
            NazwaKlasy = tbNazwaKlasyTestowej.Text;
            Rodzaj = comboRodzajMigracji.SelectedItem.ToString();
            InterfejsTestowany = tbInterfejsTestowany.Text;
            Katalog = textBoxKatalog.Text;
            Close();
        }

        private void buttonAnuluj_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void NazwaKlasyTestowForm_KeyPress(
            object sender,
            KeyPressEventArgs e)
        {
            if (e.KeyChar == '\n')
                buttonGeneruj.PerformClick();
        }

        private void NazwaKlasyTestowForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                buttonGeneruj.PerformClick();
        }
    }
}