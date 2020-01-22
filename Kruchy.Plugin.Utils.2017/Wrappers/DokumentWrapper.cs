using System.Linq;
using System.Text;
using EnvDTE;
using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace Kruchy.Plugin.Utils._2017.Wrappers
{
    public class DokumentWrapper : IDokumentWrapper
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

        public int DajNumerLiniiKursora()
        {
            return textDocument.Selection.TopPoint.Line;
        }

        public void UstawKursor(int wiersz, int kolumna)
        {
            textDocument.Selection.MoveToLineAndOffset(wiersz, kolumna);
        }

        public void UstawKursosDlaMetodyDodanejWLinii(int numerLinii)
        {
            UstawKursor(
                numerLinii + 2,
                1 + StaleDlaKodu.WciecieDlaMetody.Length
                + StaleDlaKodu.JednostkaWciecia.Length);
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

        public string DajZawartosc(int wierszPoczatek, int kolumnaPoczatek,
            int wierszKoniec, int kolumnaKoniec)
        {
            var poczatekLinii = textDocument.CreateEditPoint();
            poczatekLinii.MoveToLineAndOffset(wierszPoczatek, kolumnaPoczatek);

            var koniecLinii = textDocument.CreateEditPoint();
            koniecLinii.MoveToLineAndOffset(wierszKoniec, kolumnaKoniec);

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

        public void Usun(int numerLiniiStart, int numerKolumnyStart,
            int numerLiniiKoniec, int numerKolumnyKoniec)
        {
            var editPoint = DajEditPointMiejsca(numerLiniiStart, numerKolumnyStart);
            var editPointKoniec = DajEditPointMiejsca(numerLiniiKoniec, numerKolumnyKoniec);
            editPoint.Delete(editPointKoniec);
        }

        public void UsunLinie(int numerLinii)
        {
            var editPoint = DajEditPointPoczatkuLinii(numerLinii);
            var editPointKonca = DajEditPointPoczatkuLinii(numerLinii);
            editPointKonca.LineDown();
            editPoint.Delete(editPointKonca);
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