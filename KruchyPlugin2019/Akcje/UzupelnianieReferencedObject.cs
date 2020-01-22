using System.Linq;
using System.Text;
using System.Windows.Forms;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class UzupelnianieReferencedObject
    {
        private readonly IDokumentWrapper dokument;
        private const string NamespaceDlaAtrybutuReferencedObject = "KomponentyStandardowe.Data";

        public UzupelnianieReferencedObject(IDokumentWrapper dokument)
        {
            this.dokument = dokument;
        }

        public void Uzupelnij()
        {
            var numerLinii = DajNumerLiniiKursora();
            var zawartosc = DajZawartosc();
            var parsowane = Parser.Parsuj(zawartosc);
            var property = parsowane.SzukajPropertiesaWLinii(numerLinii);

            if (property == null)
            {
                MessageBox.Show("Nie wybrane pole");
                return;
            }

            var nazwaAtrybutu = property.Nazwa;
            var nazwaTypu = property.NazwaTypu;

            var numerLiniiDlaAtrybutuKluczaObcego = numerLinii;

            if (DodajJesliTrzebaAtrybutReferencedObject(numerLinii, nazwaAtrybutu, property))
            {
                DodajPoleKluczaObcego(nazwaAtrybutu, nazwaTypu, numerLiniiDlaAtrybutuKluczaObcego);
                DodajUsingaJesliTrzeba();
            }
        }

        private void DodajPoleKluczaObcego(
            string nazwaAtrybutu,
            string nazwaTypu,
            int numerLiniiDlaAtrybutuKluczaObcego)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("        [ForeignKey(typeof({0}))]", nazwaTypu);
            builder.AppendLine();
            builder.AppendLine("        public int " + nazwaAtrybutu + "ID { get; set; }");
            builder.AppendLine();

            dokument.WstawWLinii(builder.ToString(), numerLiniiDlaAtrybutuKluczaObcego);
        }

        private void DodajUsingaJesliTrzeba()
        {
            dokument.DodajUsingaJesliTrzeba(NamespaceDlaAtrybutuReferencedObject);
        }

        private bool DodajJesliTrzebaAtrybutReferencedObject(
            int numerLiniiKursora,
            string nazwaAtrybutu,
            KruchyParserKodu.ParserKodu.Property property)
        {
            if (property.Atrybuty.Any(o => o.Nazwa == "ReferencedObject"))
                return false;

            var nowaLinia =
                new StringBuilder()
                    .AppendLine(string.Format("        [ReferencedObject(\"{0}\")]", nazwaAtrybutu + "ID"))
                        .ToString();
            dokument.WstawWLinii(nowaLinia, numerLiniiKursora);
            return true;
        }

        private string DajZawartosc()
        {
            return dokument.DajZawartosc();
        }

        private int DajNumerLiniiKursora()
        {
            return dokument.DajNumerLiniiKursora();
        }
    }
}