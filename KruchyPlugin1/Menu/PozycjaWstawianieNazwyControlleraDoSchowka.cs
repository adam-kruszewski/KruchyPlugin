using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaWstawianieNazwyControlleraDoSchowka : PozycjaMenu
    {
        public PozycjaWstawianieNazwyControlleraDoSchowka(
            SolutionWrapper solution)
            : base(solution)
        {

        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidWstawDoSchowkaNazweControllera; }
        }

        protected override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Controller;
            }
        }

        protected override void Execute(object sender, EventArgs args)
        {
            new WstawianieNazwyControlleraDoSchowka(solution).Wstaw();
        }
    }
}
