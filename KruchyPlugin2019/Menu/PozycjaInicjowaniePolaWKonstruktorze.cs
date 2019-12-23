using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaInicjowaniePolaWKonstruktorze : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaInicjowaniePolaWKonstruktorze(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidInicjujWKontruktorze; }
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Projekt;
                yield return WymaganieDostepnosci.PlikCs;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            new InicjowaniePolaWKonstruktorze(solution).Inicjuj();
        }
    }
}