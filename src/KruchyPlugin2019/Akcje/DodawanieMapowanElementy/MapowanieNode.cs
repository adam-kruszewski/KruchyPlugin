using System.Collections.Generic;
using System.Windows.Forms;

namespace KruchyCompany.KruchyPlugin1.Akcje.DodawanieMapowanElementy
{
    class MapowanieNode : TreeNode
    {
        private readonly MapowanyProperty property;

        public MapowanieNode(MapowanyProperty property)
            : base(property.Nazwa)
        {
            this.property = property;
            foreach (var mp in property.Podobiekty)
                Nodes.Add(new MapowanieNode(mp));
        }

        public void ZapiszWybrane(IList<MapowanyProperty> wynik)
        {
            if (Checked)
            {
                wynik.Add(
                    new MapowanyProperty(
                        property.Nazwa,
                        property.NazwaTypu,
                        property.Prefix));
            }
            foreach (MapowanieNode mn in Nodes)
                mn.ZapiszWybrane(wynik);
        }
    }
}