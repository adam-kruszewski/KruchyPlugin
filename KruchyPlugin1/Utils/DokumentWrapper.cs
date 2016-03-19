using System.Linq;
using System.Text;
using EnvDTE;
using KrucheBuilderyKodu.Builders;
using KruchyParserKodu.ParserKodu;

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

        public void DodajUsingaJesliTrzeba(string nazwaNamespace)
        {
            var parsowane = Parser.Parsuj(DajZawartosc());

            if (parsowane.Usingi.Select(o => o.Nazwa).Contains(nazwaNamespace))
                return;

            int wierszWstawienia = 1;
            int kolumnaWstawienia = 1;
            var aktualneUsingi = parsowane.Usingi.Select(o => o.Nazwa).ToList();
            aktualneUsingi.Add(nazwaNamespace);

            if (parsowane.Usingi.Any())
            {
                UsunWszystkieUsingi(
                    parsowane,
                    ref wierszWstawienia,
                    ref kolumnaWstawienia);
            }

            var posortowaneDoWstawienia =
                aktualneUsingi
                    .OrderBy(o => DajKluczDoSortowaniaUsingow(o))
                        .ToList();
            var builder = new StringBuilder();
            foreach (var u in posortowaneDoWstawienia)
                builder.AppendLine("using " + u + ";");
            var nowyTekst = builder.ToString().TrimEnd();

            WstawWMiejscu(nowyTekst, wierszWstawienia, 1);
        }

        private void UsunWszystkieUsingi(
            Plik parsowane,
            ref int wierszWstawienia,
            ref int kolumnaWstawienia)
        {
            var dotychczasowePosortowane =
                parsowane.Usingi.OrderBy(o => o.Poczatek.Wiersz);

            var pierwszyUsing = dotychczasowePosortowane.First();
            var ostatniUsing = dotychczasowePosortowane.Last();

            Usun(pierwszyUsing.Poczatek.Wiersz, pierwszyUsing.Poczatek.Kolumna,
                ostatniUsing.Koniec.Wiersz, ostatniUsing.Koniec.Kolumna);
            wierszWstawienia = pierwszyUsing.Poczatek.Wiersz;
            kolumnaWstawienia = pierwszyUsing.Poczatek.Kolumna;
        }

        private string DajKluczDoSortowaniaUsingow(string nazwaUsinga)
        {
            if (nazwaUsinga.StartsWith("System.") || nazwaUsinga == "System")
                return "0" + nazwaUsinga;
            else
                return "1" + nazwaUsinga;
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