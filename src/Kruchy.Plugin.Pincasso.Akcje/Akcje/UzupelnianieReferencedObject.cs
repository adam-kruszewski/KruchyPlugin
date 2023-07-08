using System.Linq;
using System.Text;
using System.Windows.Forms;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace Kruchy.Plugin.Pincasso.Akcje.Akcje
{
    class UzupelnianieReferencedObject
    {
        private readonly IDocumentWrapper dokument;
        private const string NamespaceDlaAtrybutuReferencedObject = "KomponentyStandardowe.Data";

        public UzupelnianieReferencedObject(IDocumentWrapper dokument)
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

            var nazwaAtrybutu = property.Name;
            var nazwaTypu = property.TypeName;

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

            dokument.InsertInLine(builder.ToString(), numerLiniiDlaAtrybutuKluczaObcego);
        }

        private void DodajUsingaJesliTrzeba()
        {
            dokument.DodajUsingaJesliTrzeba(NamespaceDlaAtrybutuReferencedObject);
        }

        private bool DodajJesliTrzebaAtrybutReferencedObject(
            int numerLiniiKursora,
            string nazwaAtrybutu,
            KruchyParserKodu.ParserKodu.Models.Property property)
        {
            if (property.Attributes.Any(o => o.Name == "ReferencedObject"))
                return false;

            var nowaLinia =
                new StringBuilder()
                    .AppendLine(string.Format("        [ReferencedObject(\"{0}\")]", nazwaAtrybutu + "ID"))
                        .ToString();
            dokument.InsertInLine(nowaLinia, numerLiniiKursora);
            return true;
        }

        private string DajZawartosc()
        {
            return dokument.GetContent();
        }

        private int DajNumerLiniiKursora()
        {
            return dokument.GetCursorLineNumber();
        }
    }
}
