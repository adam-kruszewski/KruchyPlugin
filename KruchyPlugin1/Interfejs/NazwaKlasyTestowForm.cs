using System;
using System.Windows.Forms;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Extensions;

namespace KruchyCompany.KruchyPlugin1.Interfejs
{
    public partial class NazwaKlasyTestowForm : Form
    {
        public string NazwaKlasy { get; private set; }
        public RodzajKlasyTestowej Rodzaj { get; private set; }
        public string InterfejsTestowany { get; private set; }
        public bool Integracyjny { get; set; }

        RodzajKlasyTestowej[] rodzaje =
        {
            RodzajKlasyTestowej.ServiceTests,
            RodzajKlasyTestowej.TestsWithDatabase,
            RodzajKlasyTestowej.Zwykla
        };

        public NazwaKlasyTestowForm(SolutionWrapper solution)
        {
            InitializeComponent();
            foreach (var r in rodzaje)
                comboRodzajMigracji.Items.Add(r);

            if (solution.AktualnyPlik == null)
                throw new ApplicationException("Brak otwartego pliku");
            comboRodzajMigracji.SelectedIndex = 0;

            var nazwaObiektu = solution.NazwaObiektuAktualnegoPliku();

            tbInterfejsTestowany.Text = nazwaObiektu;
            if (!tbInterfejsTestowany.Text.StartsWith("I"))
                tbInterfejsTestowany.Text = "I" + tbInterfejsTestowany.Text;
            tbNazwaKlasyTestowej.Text = nazwaObiektu + "Tests";
            if (nazwaObiektu.StartsWith("I") && char.IsUpper(nazwaObiektu[1]))
                tbNazwaKlasyTestowej.Text = nazwaObiektu.Substring(1) + "Tests";
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
            Rodzaj = (RodzajKlasyTestowej)comboRodzajMigracji.SelectedItem;
            InterfejsTestowany = tbInterfejsTestowany.Text;
            Integracyjny = checkBoxIntegracyjny.Checked;
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
