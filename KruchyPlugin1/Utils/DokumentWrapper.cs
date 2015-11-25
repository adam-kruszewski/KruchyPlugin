using EnvDTE;

namespace KruchyCompany.KruchyPlugin1.Utils
{
    public class DokumentWrapper
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
            this.projektWrapper = projekt;
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

        public void WstawWMiejscu(string tekst, int numerLinii, int numerKolumny)
        {
            var editPoint = DajEditPointPoczatkuLinii(numerLinii);
            editPoint.MoveToLineAndOffset(numerLinii, numerKolumny);
            editPoint.Insert(tekst);
        }

        public void UsunWMiejscu(int numerLinii, int numerKolumny, int dlugosc)
        {
            var editPoint = DajEditPointMiejsca(numerLinii, numerKolumny);
            var editPointKoniec = DajEditPointMiejsca(numerLinii, numerKolumny + dlugosc);
            editPoint.Delete(editPointKoniec);
        }

        private EditPoint DajEditPointPoczatkuLinii(
            int numerLinii)
        {
            var poczatekLinii = textDocument.CreateEditPoint();
            poczatekLinii.MoveToLineAndOffset(numerLinii, 1);
            return poczatekLinii;
        }

        private EditPoint DajEditPointMiejsca(
            int numerLinii, int numerKolumny)
        {
            var editPoint = DajEditPointPoczatkuLinii(numerLinii);
            editPoint.MoveToLineAndOffset(numerLinii, numerKolumny);
            return editPoint;
        }

        public int DajLiczbeLinii()
        {
            return textDocument.EndPoint.Line;
        }
    }
}