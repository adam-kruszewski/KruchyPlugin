using System;
using System.Windows.Forms;

namespace KruchyCompany.KruchyPlugin1.Interfejs
{
    public partial class NazwaKlasyWindow : Form
    {
        public string NazwaPliku { get; private set; }
        public string EtykietaNazwyPliku
        {
            set { label1.Text = value; }
        }
        public string InicjalnaWartosc
        {
            set { tbNazwaKlasy.Text = value; }
        }

        public NazwaKlasyWindow()
        {
            InitializeComponent();
            EtykietaNazwyPliku = "Nazwa pliku";
        }

        private void buttonAnuluj_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonZatwierdz_Click(object sender, EventArgs e)
        {
            var wartosc = tbNazwaKlasy.Text.Trim();
            if (string.IsNullOrEmpty(wartosc))
            {
                MessageBox.Show("Wypełnij nazwę klsay");
                tbNazwaKlasy.Select();
                return;
            }
            NazwaPliku = wartosc;
            Close();
        }
    }
}
