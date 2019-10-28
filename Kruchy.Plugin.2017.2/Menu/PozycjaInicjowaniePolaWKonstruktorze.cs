using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaInicjowaniePolaWKonstruktorze : PozycjaMenu, IPozycjaMenu
    {
        public override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidInicjujWKontruktorze; }
        }

        public override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Projekt;
                yield return WymaganieDostepnosci.PlikCs;
            }
        }

        public PozycjaInicjowaniePolaWKonstruktorze(
            ISolutionWrapper solution) : base(solution) { }

        public override void Execute(object sender, EventArgs args)
        {
            new InicjowaniePolaWKonstruktorze(solution).Inicjuj();
        }
    }
}