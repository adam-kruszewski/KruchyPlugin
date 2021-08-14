using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KruchyCompany.KruchyPlugin1.Akcje.DodawanieMapowanElementy
{
    public partial class WyborMapowanForm : Form
    {
        public List<MapowanyProperty> Wybrane { get; set; }

        public WyborMapowanForm(IEnumerable<MapowanyProperty> opisMapowan)
        {
            Wybrane = new List<MapowanyProperty>();

            InitializeComponent();

            foreach (var mp in opisMapowan)
            {
                var node = new MapowanieNode(mp);
                treeView1.Nodes.Add(node);
                node.Checked = true;
            }
        }

        private void buttonAnuluj_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonGenerujMapowania_Click(object sender, EventArgs e)
        {
            Wybrane.Clear();
            foreach (MapowanieNode mn in treeView1.Nodes)
                mn.ZapiszWybrane(Wybrane);
            this.Close();
        }

        private void buttonZaznaczWszystkie_Click(object sender, EventArgs e)
        {
            foreach (TreeNode nd in treeView1.Nodes)
                nd.Checked = true;
        }

        private void buttonOdznaczWszystkie_Click(object sender, EventArgs e)
        {
            OdznaczWszystkie(treeView1.Nodes);
        }

        private void OdznaczWszystkie(TreeNodeCollection nodes)
        {
            foreach (TreeNode nd in nodes)
            {
                nd.Checked = false;
                OdznaczWszystkie(nd.Nodes);
            }
        }
    }
}
