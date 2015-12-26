﻿using System;
using System.IO;
using System.Windows;
using KruchyCompany.KruchyPlugin1.ParserKodu;
using KruchyCompany.KruchyPlugin1.Utils;

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
            string sciezkaImplementacji = SzukajSciezkiDoImplementacji(aktualny);
            OtworzJesliSciezkaNieNullowa(sciezkaImplementacji);
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