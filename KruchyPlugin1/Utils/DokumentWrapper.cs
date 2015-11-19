using EnvDTE;

namespace KruchyCompany.KruchyPlugin1.Utils
{
    class DokumentWrapper
    {
        private readonly TextDocument textDocument;
        private readonly ProjektWrapper projektWrapper;

        public DokumentWrapper(TextDocument textDocument)
        {
            this.textDocument = textDocument;
        }

        public DokumentWrapper(TextDocument textDocument, ProjektWrapper projekt)
        {
            this.textDocument = textDocument;
            this.projektWrapper = projektWrapper;
        }

        public void DodajUsingaJesliTrzeba(string nazwaUsinga)
        {
            int indeks = 1;
            string linia = null;
            do
            {
                linia = DajZawartoscLinii(indeks);
                if (linia.Contains("using") && linia.Contains(nazwaUsinga))
                    return;
                indeks++;
            } while (linia.Contains("using"));

            var endPoint = DajEditPointPoczatkuLinii(indeks - 1);
            endPoint.Insert(
                string.Format("using {0};\n", nazwaUsinga));
        }

        public int DajNumerLiniiKursora()
        {
            return textDocument.Selection.TopPoint.Line;
        }

        public string DajZawartosc()
        {
            EditPoint objEditPoint =
                (EditPoint)textDocument.StartPoint.CreateEditPoint();
            EditPoint endEditPoint =
                (EditPoint)textDocument.EndPoint.CreateEditPoint();

            return objEditPoint.GetText(endEditPoint.AbsoluteCharOffset);
        }

        public string DajZawartoscLinii(int numerLinii)
        {
            var poczatekLinii = textDocument.CreateEditPoint();
            poczatekLinii.MoveToLineAndOffset(numerLinii, 1);

            var koniecLinii = textDocument.CreateEditPoint();
            koniecLinii.MoveToLineAndOffset(numerLinii, 1);
            koniecLinii.MoveToLineAndOffset(numerLinii, koniecLinii.LineLength + 1);

            return poczatekLinii.GetText(koniecLinii);
        }

        public void WstawWLinii(string tekst, int numerLinii)
        {
            var poczatekLinii =
            DajEditPointPoczatkuLinii(numerLinii);
            poczatekLinii.Insert(tekst);
        }

        private EditPoint DajEditPointPoczatkuLinii(
            int numerLinii)
        {
            var poczatekLinii = textDocument.CreateEditPoint();
            poczatekLinii.MoveToLineAndOffset(numerLinii, 1);
            return poczatekLinii;
        }

        public int DajLiczbeLinii()
        {
            return textDocument.EndPoint.Line;
        }
    }
}