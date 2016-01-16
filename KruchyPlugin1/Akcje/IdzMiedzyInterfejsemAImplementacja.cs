﻿using System;
using System.IO;
using System.Linq;
using System.Windows;
using KruchyCompany.KruchyPlugin1.ParserKodu;
using KruchyCompany.KruchyPlugin1.Utils;
using System.Collections.Generic;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class IdzMiedzyInterfejsemAImplementacja
    {
        private readonly SolutionWrapper solution;

        public IdzMiedzyInterfejsemAImplementacja(SolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Przejdz()
        {
            var aktualny = solution.AktualnyPlik;
            if (aktualny == null)
                return;

            if (JestInterfejsem(aktualny))
                SprobujPrzejscDoImplementacji(aktualny);
            else
                SprobujPrzejscDoInterfejsu(aktualny);
        }

        private bool JestInterfejsem(PlikWrapper aktualny)
        {
            var zawartosc = aktualny.Dokument.DajZawartosc();
            var parsowane = Parser.Parsuj(zawartosc);
            if (parsowane.DefiniowaneObiekty.Count == 1)
            {
                return parsowane.DefiniowaneObiekty[0].Rodzaj == RodzajObiektu.Interfejs;
            }
            else
                throw new Exception("Brak zdefiniowanego obiektu");
        }

        private void SprobujPrzejscDoImplementacji(PlikWrapper aktualny)
        {
            var parsowane = Parser.Parsuj(aktualny.Dokument.DajZawartosc());
            var metoda =
                parsowane.SzukajMetodyWLinii(
                    aktualny.Dokument.DajNumerLiniiKursora());

            string sciezkaImplementacji = SzukajSciezkiDoImplementacji(aktualny);
            OtworzJesliSciezkaNieNullowa(sciezkaImplementacji);

            if (!string.IsNullOrEmpty(sciezkaImplementacji) && metoda != null)
            {
                UstawSieNaMetodzie(metoda);
            }
        }

        private void UstawSieNaMetodzie(Metoda metoda)
        {
            var parsowane =
                Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());

            if (parsowane.DefiniowaneObiekty.Count != 1)
                return;

            var znalezionaMetoda =
                parsowane.DefiniowaneObiekty[0].Metody
                    .Where(o => TaSamaMetoda(metoda, o))
                        .FirstOrDefault();

            if (znalezionaMetoda != null)
                solution.AktualnyDokument.UstawKursor(
                    znalezionaMetoda.Poczatek.Wiersz,
                    znalezionaMetoda.Poczatek.Kolumna);
        }

        private static bool TaSamaMetoda(Metoda m1, Metoda m2)
        {
            if (m1.Nazwa != m2.Nazwa)
                return false;

            if (m1.Parametry.Count != m2.Parametry.Count)
                return false;

            if (!TakieSameParametry(m1.Parametry, m2.Parametry))
                return false;

            return true;
        }

        private static bool TakieSameParametry(
            IList<Parametr> list1,
            IList<Parametr> list2)
        {

            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i].NazwaTypu != list2[i].NazwaTypu)
                    return false;
                if (list1[i].NazwaParametru != list2[i].NazwaParametru)
                    return false;
            }
            return true;
        }

        private string SzukajSciezkiDoImplementacji(PlikWrapper aktualny)
        {
            var katalog = aktualny.Katalog;
            var katalogImpl = Path.Combine(katalog, "Impl");
            var nazwa = aktualny.Nazwa.Substring(1);
            var sciezka = Path.Combine(katalogImpl, nazwa);
            if (File.Exists(sciezka))
                return sciezka;
            sciezka = Path.Combine(aktualny.Katalog, nazwa);
            if (File.Exists(sciezka))
                return sciezka;
            return null;
        }

        private void SprobujPrzejscDoInterfejsu(PlikWrapper aktualny)
        {
            string sciezkaDoInterfejsu = SzukajSciezkiDoInterfejsu(aktualny);
            OtworzJesliSciezkaNieNullowa(sciezkaDoInterfejsu);
        }

        private string SzukajSciezkiDoInterfejsu(PlikWrapper aktualny)
        {
            var katalog = aktualny.Katalog;
            var katalogInterfejsu = Directory.GetParent(katalog).FullName;
            var nazwa = "I" + aktualny.Nazwa;
            var sciezka = Path.Combine(katalogInterfejsu, nazwa);
            if (File.Exists(sciezka))
                return sciezka;
            sciezka = Path.Combine(aktualny.Katalog, nazwa);
            if (File.Exists(sciezka))
                return sciezka;
            MessageBox.Show("Nie znalazłem " + sciezka);
            return null;
        }

        private void OtworzJesliSciezkaNieNullowa(string sciezka)
        {
            if (sciezka == null)
                return;
            var solutionExplorer = new SolutionExplorerWrapper(solution);
            solutionExplorer.OtworzPlik(sciezka);
        }
    }
}