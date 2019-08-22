﻿using System.Windows;
using System.Windows.Forms;
using Kruchy.Plugin.Utils.Wrappers;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class DodawaniaUsinga
    {
        private readonly SolutionWrapper solution;

        public DodawaniaUsinga(SolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Dodaj(params string[] usingi)
        {
            if (solution.AktualnyPlik == null)
            {
                MessageBox.Show("Brak otwartego pliku");
                return;
            }
            foreach (var nazwaUsinga in usingi)
                solution
                    .AktualnyPlik
                        .Dokument
                            .DodajUsingaJesliTrzeba(nazwaUsinga);
        }
    }
}