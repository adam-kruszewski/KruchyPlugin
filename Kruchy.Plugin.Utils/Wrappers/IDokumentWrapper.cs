using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruchy.Plugin.Utils.Wrappers
{
    public interface IDokumentWrapper
    {
        void DodajUsingaJesliTrzeba(string nazwaNamespace);

        int DajNumerLiniiKursora();

        void UstawKursor(int wiersz, int kolumna);

        void UstawKursosDlaMetodyDodanejWLinii(int numerLinii);

        string DajZawartosc();

        string DajZawartoscLinii(int numerLinii);

        string DajZawartosc(
            int wierszPoczatek,
            int kolumnaPoczatek,
            int wierszKoniec,
            int kolumnaKoniec);

        void WstawWLinii(string tekst, int numerLinii);

        void WstawWMiejscu(string tekst, int numerLinii, int numerKolumny);

        void UsunWMiejscu(int numerLinii, int numerKolumny, int dlugosc);

        void Usun(
            int numerLiniiStart,
            int numerKolumnyStart,
            int numerLiniiKoniec,
            int numerKolumnyKoniec);

        void UsunLinie(int numerLinii);

        int DajLiczbeLinii();
    }
}
