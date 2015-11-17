using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KruchyCompany.KruchyPlugin1.Interfejs
{
    public partial class NazwaKlasyWindow : Form
    {
        public string NazwaPliku { get; private set; }

        public NazwaKlasyWindow()
        {
            InitializeComponent();
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
