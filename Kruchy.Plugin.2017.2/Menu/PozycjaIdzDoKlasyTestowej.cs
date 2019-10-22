using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaIdzDoKlasyTestowej : PozycjaMenu
    {
        public PozycjaIdzDoKlasyTestowej(ISolutionWrapper solution)
            : base(solution)
        {

        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidIdzDoKlasyTestowej; }
        }

        protected override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.PlikCs;
            }
        }

        protected override void Execute(object sender, EventArgs args)
        {
            new IdzDoKlasyTestowej(solution).Przejdz();
        }
    }
}
