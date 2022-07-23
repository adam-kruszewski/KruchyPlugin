using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruchy.Plugin.Utils.Wrappers
{
    public interface IDocumentWrapper
    {
        int GetCursorLineNumber();

        void SetCursor(int wiersz, int kolumna);

        void SetCursorForAddedMethod(int numerLinii);

        string GetContent();

        string GetLineContent(int numerLinii);

        string GetContent(
            int wierszPoczatek,
            int kolumnaPoczatek,
            int wierszKoniec,
            int kolumnaKoniec);

        void InsertInLine(string tekst, int numerLinii);

        void InsertInPlace(string tekst, int numerLinii, int numerKolumny);

        void RemoveInPlace(int numerLinii, int numerKolumny, int dlugosc);

        void Remove(
            int numerLiniiStart,
            int numerKolumnyStart,
            int numerLiniiKoniec,
            int numerKolumnyKoniec);

        void RemoveLine(int numerLinii);

        int GetLineCount();
    }
}
