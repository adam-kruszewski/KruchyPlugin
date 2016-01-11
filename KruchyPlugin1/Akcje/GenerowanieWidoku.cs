﻿using System.IO;
using System.Windows;
using KruchyCompany.KruchyPlugin1.Extensions;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class GenerowanieWidoku
    {
        private readonly SolutionWrapper solution;

        public GenerowanieWidoku(SolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Generuj(string nazwa)
        {
            if (!solution.CzyPlikControllera())
            {
                MessageBox.Show("To nie jest plik controllera");
                return;
            }
            nazwa = Normalizuj(nazwa);

            var katalogControllera =
                solution.AktualnyPlik.SciezkaKataloguControllera();
            if (Directory.Exists(katalogControllera))
                Directory.CreateDirectory(katalogControllera);

            var pelnaSciezka = Path.Combine(katalogControllera, nazwa);
            File.WriteAllText(pelnaSciezka, "");
            solution.AktualnyProjekt.DodajPlik(pelnaSciezka);
            solution.OtworzPlik(pelnaSciezka);
        }

        private string Normalizuj(string nazwa)
        {
            if (!nazwa.ToLower().EndsWith(".cshtml"))
                return nazwa + ".cshtml";
            else
                return nazwa;
        }
    }
}