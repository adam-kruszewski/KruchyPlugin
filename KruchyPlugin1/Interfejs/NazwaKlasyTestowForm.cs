using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Interfejs
{
    public partial class NazwaKlasyTestowForm : Form
    {
        public string NazwaKlasy { get; private set; }
        public RodzajKlasyTestowej Rodzaj { get; private set; }
        public string InterfejsTestowany { get; private set; }

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
            tbInterfejsTestowany.Text =
                "I" + solution.AktualnyPlik.NazwaBezRozszerzenia;
            tbNazwaKlasyTestowej.Text =
                solution.AktualnyPlik.NazwaBezRozszerzenia + "Tests";
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
