using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaInicjowaniePolaWKonstruktorze : PozycjaMenu
    {
        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidInicjujWKontruktorze; }
        }

        protected override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Projekt;
                yield return WymaganieDostepnosci.PlikCs;
            }
        }

        public PozycjaInicjowaniePolaWKonstruktorze(
            SolutionWrapper solution) : base(solution) { }

        protected override void Execute(object sender, EventArgs args)
        {
            new InicjowaniePolaWKonstruktorze(solution).Inicjuj();
        }
    }
}