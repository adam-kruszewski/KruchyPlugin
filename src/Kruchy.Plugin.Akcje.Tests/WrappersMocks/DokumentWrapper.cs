using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Tests.WrappersMocks
{
    class DokumentWrapper : IDocumentWrapper
    {
        int pozycjaKursoraX = 1;
        int pozycjaKursoraY = 1;

        string zawartosc;

        public DokumentWrapper(string zawartosc)
        {
            this.zawartosc = zawartosc;
        }

        public int GetLineCount()
        {
            return Linie().Count;
        }

        public int GetCursorLineNumber()
        {
            return pozycjaKursoraY + 1;
        }

        public string GetContent()
        {
            return zawartosc;
        }

        public string GetContent(
            int wierszPoczatek,
            int kolumnaPoczatek,
            int wierszKoniec,
            int kolumnaKoniec)
        {
            var linie = Linie();

            var builder = new StringBuilder();

            for (int i = wierszPoczatek - 1; i <= wierszKoniec - 1; i++)
            {
                var poczatek = 0;
                var koniec = linie[i].Length - 1;

                if (i == wierszPoczatek - 1)
                    poczatek = kolumnaPoczatek;

                if (i == wierszKoniec - 1)
                    koniec = kolumnaKoniec;

                if (koniec < poczatek)
                    builder.AppendLine();
                else
                {
                    builder.AppendLine(linie[i].Substring(poczatek, koniec - poczatek + 1));
                }

            }

            return builder.ToString();
        }

        public string GetLineContent(int numerLinii)
        {
            return Linie()[numerLinii - 1];
        }

        public void SetCursor(int wiersz, int kolumna)
        {
            pozycjaKursoraX = wiersz - 1;
            pozycjaKursoraY = kolumna - 1;
        }

        public void SetCursorForAddedMethod(int numerLinii)
        {
            throw new NotImplementedException();
        }

        public void Remove(
            int numerLiniiStart,
            int numerKolumnyStart,
            int numerLiniiKoniec,
            int numerKolumnyKoniec)
        {
            int aktualnyNumerLinii = 1;
            int aktualnyNumerKolumny = 1;

            int indeksPoczatkuUsuwania = -1;
            int indeksKoncaUsuwania = -1;

            for (int i = 0; i < zawartosc.Length; i++)
            {
                if (aktualnyNumerLinii == numerLiniiStart &&
                    aktualnyNumerKolumny == numerKolumnyStart)
                {
                    indeksPoczatkuUsuwania = i;
                }

                if (aktualnyNumerLinii == numerLiniiKoniec &&
                    aktualnyNumerKolumny == numerKolumnyKoniec)
                {
                    indeksKoncaUsuwania = i;
                    if (zawartosc[i] == '\r')
                        indeksKoncaUsuwania--;
                    break;
                }

                if (zawartosc[i] == '\n')
                {
                    aktualnyNumerLinii++;
                    aktualnyNumerKolumny = 1;
                }
                else
                {
                    aktualnyNumerKolumny++;
                }
            }

            var poczatek = "";
            if (indeksPoczatkuUsuwania != -1)
                poczatek += zawartosc.Substring(0, indeksPoczatkuUsuwania);
            zawartosc = poczatek + zawartosc.Substring(indeksKoncaUsuwania + 1);

        }

        private void UstawZawartoscZLinii(IList<string> linie)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < linie.Count; i++)
                if (i != linie.Count - 1)
                    builder.AppendLine(linie[i]);
                else
                    builder.Append(linie[i]);

            zawartosc = builder.ToString();
        }

        public void RemoveLine(int numerLinii)
        {
            var linie = Linie();
            linie.RemoveAt(numerLinii);
            UstawZawartoscZLinii(linie);
        }

        public void RemoveInPlace(int numerLinii, int numerKolumny, int dlugosc)
        {
            throw new NotImplementedException();
        }

        public void InsertInLine(string tekst, int numerLinii)
        {
            var koniecLinii = new StringBuilder().AppendLine().ToString();
            if (!tekst.EndsWith(koniecLinii) && !tekst.EndsWith("\n"))
                tekst += koniecLinii;
            InsertInPlace(tekst, numerLinii, 1);
        }

        public void InsertInPlace(string tekst, int numerLinii, int numerKolumny)
        {
            int aktualnyNumerLinii = 1;
            int aktualnyNumerKolumny = 1;

            for (int i = 0; i < zawartosc.Length; i++)
            {
                if (numerLinii == aktualnyNumerLinii && aktualnyNumerKolumny == numerKolumny)
                {
                    var poczatek = "";

                    if (i > 0)
                        poczatek = zawartosc.Substring(0, i);

                    zawartosc = poczatek + tekst + zawartosc.Substring(i);
                    return;
                }

                if (zawartosc[i] == '\n')
                {
                    aktualnyNumerLinii++;
                    aktualnyNumerKolumny = 1;
                }
                else
                    aktualnyNumerKolumny++;
            }
        }

        private IList<string> Linie()
        {
            return
                zawartosc
                    .Split(new char[] { '\n' })
                        .Select(o => o.Replace("\r", ""))
                            .ToList();
        }
    }
}
