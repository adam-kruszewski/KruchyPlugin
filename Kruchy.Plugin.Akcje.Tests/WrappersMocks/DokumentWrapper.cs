using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Tests.WrappersMocks
{
    class DokumentWrapper : IDokumentWrapper
    {
        int pozycjaKursoraX = 1;
        int pozycjaKursoraY = 1;

        string zawartosc;

        public DokumentWrapper(string zawartosc)
        {
            this.zawartosc = zawartosc;
        }

        public int DajLiczbeLinii()
        {
            return Linie().Count;
        }

        public int DajNumerLiniiKursora()
        {
            return pozycjaKursoraY + 1;
        }

        public string DajZawartosc()
        {
            return zawartosc;
        }

        public string DajZawartosc(
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

        public string DajZawartoscLinii(int numerLinii)
        {
            return Linie()[numerLinii - 1];
        }

        public void DodajUsingaJesliTrzeba(string nazwaNamespace)
        {
            throw new NotImplementedException();
        }

        public void UstawKursor(int wiersz, int kolumna)
        {
            pozycjaKursoraX = wiersz - 1;
            pozycjaKursoraY = kolumna - 1;
        }

        public void UstawKursosDlaMetodyDodanejWLinii(int numerLinii)
        {
            throw new NotImplementedException();
        }

        public void Usun(
            int numerLiniiStart,
            int numerKolumnyStart,
            int numerLiniiKoniec,
            int numerKolumnyKoniec)
        {
            var linie = Linie();
            for (int i = numerLiniiKoniec - 1; i >= numerLiniiStart - 1; i--)
            {
                if (i != numerLiniiStart - 1 && i != numerLiniiKoniec - 1)
                {
                    linie.RemoveAt(i);
                }
                else
                {
                    var poczatek = 0;
                    var koniec = linie[i].Length - 1;

                    if (i == numerLiniiStart - 1)
                        poczatek = numerKolumnyStart - 1;
                    if (i == numerLiniiKoniec - 1)
                        koniec = numerKolumnyKoniec - 1;

                    if (poczatek == 0 && koniec >= linie[i].Length - 1)
                        linie.RemoveAt(i);
                    else
                        linie[i] = linie[i].Substring(poczatek, koniec - poczatek);
                }
            }
            UstawZawartoscZLinii(linie);
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

        public void UsunLinie(int numerLinii)
        {
            var linie = Linie();
            linie.RemoveAt(numerLinii);
            UstawZawartoscZLinii(linie);
        }

        public void UsunWMiejscu(int numerLinii, int numerKolumny, int dlugosc)
        {
            throw new NotImplementedException();
        }

        public void WstawWLinii(string tekst, int numerLinii)
        {
            WstawWMiejscu(tekst, numerLinii, 0);
        }

        public void WstawWMiejscu(string tekst, int numerLinii, int numerKolumny)
        {
            var linie = Linie();

            var linieDoWstawienia =
                tekst
                    .Split(new char[] { '\n' })
                        .ToList();

            if (linieDoWstawienia.Last().EndsWith("\r"))
                linieDoWstawienia.Add("");
            linieDoWstawienia = linieDoWstawienia.Select(o => o.Replace("\r", "")).ToList();

            var aktualnyIndeksWstawianejLinii = numerLinii - 1;
            var linia = linie[aktualnyIndeksWstawianejLinii];

            for (int i = 0; i < linieDoWstawienia.Count; i++)
            {
                var wstawianaLinia = linieDoWstawienia[i];

                if (i == linieDoWstawienia.Count - 1)
                {
                    if (numerKolumny - 1 > linia.Length)
                    {
                        linia =
                            linia.Substring(0, numerKolumny)
                            + tekst +
                            linia.Substring(numerKolumny);
                    }
                    else if (numerKolumny == 0)
                    {
                        linia = wstawianaLinia + linia;
                    }
                    else

                        linia = linia + wstawianaLinia;
                    linie[aktualnyIndeksWstawianejLinii] = linia;
                    aktualnyIndeksWstawianejLinii++;
                }
                else
                {
                    linie.Insert(aktualnyIndeksWstawianejLinii, wstawianaLinia);
                    aktualnyIndeksWstawianejLinii++;
                }
            }

            UstawZawartoscZLinii(linie);
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
