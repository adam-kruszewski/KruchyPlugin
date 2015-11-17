using EnvDTE;
using KruchyCompany.KruchyPlugin1.Utils;
using System;
using System.Linq;
using System.Text;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class UzupelnianieReferencedObject
    {
        private readonly _DTE dte;
        private readonly DokumentWrapper dokument;
        private string[] modyifkatory =
        {
            "public",
            "private",
            "internal",
            "protected",
            "const"
        };
        private const string NamespaceDlaAtrybutuReferencedObject = "KomponentyStandardowe.Data";

        public UzupelnianieReferencedObject(_DTE dte)
        {
            this.dte = dte;
            var textDoc = (TextDocument)dte.ActiveDocument.Object("TextDocument");
            dokument = new DokumentWrapper(textDoc);
        }

        public void Uzupelnij()
        {
            var textDoc = (TextDocument)dte.ActiveDocument.Object("TextDocument");

            var numerLinii = DajNumerLiniiKursora();
            var zawartosc = DajZawartosc();
            var zawartoscLinii = DajZawartoscLinii(numerLinii);
            var nazwaAtrybutu = DajNazweAtrybutu(zawartoscLinii);
            var nazwaTypu = DajNazweTypu(zawartoscLinii);

            var numerLiniiDlaAtrybutuKluczaObcego = numerLinii;

            DodajJesliTrzebaAtrybutReferencedObject(numerLinii, nazwaAtrybutu);
            DodajPoleKluczaObcego(nazwaAtrybutu, nazwaTypu, numerLiniiDlaAtrybutuKluczaObcego);
            DodajUsingaJesliTrzeba();
        }

        private void DodajPoleKluczaObcego(
            string nazwaAtrybutu,
            string nazwaTypu,
            int numerLiniiDlaAtrybutuKluczaObcego)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("        [ForeignKey(typeof({0}))]\n", nazwaTypu);
            builder.AppendLine("        public int " + nazwaAtrybutu + "ID { get; set; }");
            builder.AppendLine();

            dokument.WstawWLinii(builder.ToString(), numerLiniiDlaAtrybutuKluczaObcego);
        }

        private void DodajUsingaJesliTrzeba()
        {
            dokument.DodajUsingaJesliTrzeba(NamespaceDlaAtrybutuReferencedObject);
        }

        private string DajNazweTypu(string zawartoscLinii)
        {
            var slowa = PodzielNaSlowa(zawartoscLinii);
            return slowa.Where(o => !modyifkatory.Contains(o)).First();
        }

        private string DajNazweAtrybutu(string zawartoscLinii)
        {
            var slowa = PodzielNaSlowa(zawartoscLinii);
            return slowa.Where(o => !modyifkatory.Contains(o)).ToList()[1];
        }

        private bool DodajJesliTrzebaAtrybutReferencedObject(
            int numerLiniiKursora, string nazwaAtrybutu)
        {
            var poprzedniaLinia = DajZawartoscLinii(numerLiniiKursora - 1);
            if (poprzedniaLinia.Contains("ReferencedObject"))
                return false;
            var nowaLinia =
                string.Format("\n        [ReferencedObject(\"{0}\")]", nazwaAtrybutu + "ID");
            dokument.WstawWLinii(nowaLinia, numerLiniiKursora - 1);
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

        private string DajZawartoscLinii(int numerLinii)
        {
            return dokument.DajZawartoscLinii(numerLinii);
        }

        private EditPoint DajEditPointPoczatkuLinii(
            TextDocument textDocument,
            int numerLinii)
        {
            var poczatekLinii = textDocument.CreateEditPoint();
            poczatekLinii.MoveToLineAndOffset(numerLinii, 1);
            return poczatekLinii;
        }

        private string[] PodzielNaSlowa(string linia)
        {
            var separatory = new char[] { ' ', '\t' };
            return linia.Split(separatory, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
