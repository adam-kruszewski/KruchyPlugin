using System;
using System.Windows.Forms;

namespace KruchyCompany.KruchyPlugin1.Interfejs
{
    public partial class NazwaKlasyWindow : Form
    {
        public string NazwaPliku { get; private set; }
        public bool StanCheckBoxa { get; set; }
        public string EtykietaNazwyPliku
        {
            set { label1.Text = value; }
        }
        public string EtykietaCheckBoxa
        {
            set { checkBox1.Text = value; }
        }
        public string InicjalnaWartosc
        {
            set { tbNazwaKlasy.Text = value; }
        }

        public NazwaKlasyWindow(bool zCheckBoxem = false)
        {
            InitializeComponent();
            EtykietaNazwyPliku = "Nazwa pliku";
            if (!zCheckBoxem)
            {
                checkBox1.Visible = false;
                checkBox1.Enabled = false;
                var roznica = -20;
                PrzesunWPionie(buttonAnuluj, roznica);
                PrzesunWPionie(buttonZatwierdz, roznica);
                Height += roznica;
            }
            else
            {
            }
        }

        private void PrzesunWPionie(Button button, int roznica)
        {
            var aktualne = button.Bounds;
            button.SetBounds(
                aktualne.X,
                aktualne.Y + roznica,
                aktualne.Width,
                aktualne.Height);
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
            StanCheckBoxa = checkBox1.Checked;
            Close();
        }
    }
}
