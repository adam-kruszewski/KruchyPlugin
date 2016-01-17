using System;
using System.Collections.Generic;
using System.Linq;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.KonfiguracjaPlugina;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaDodawanieUsingow : PozycjaMenu
    {
        public PozycjaDodawanieUsingow(SolutionWrapper solution)
            : base(solution)
        {

        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidDodajNaczesciejUzywaneUsingi; }
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
            var konf = Konfiguracja.GetInstance();
            var usingi =
                konf.DajKonfiguracjeUsingow(solution)
                    .NajczesciejUzywane.ToArray();
            new DodawaniaUsinga(solution)
                .Dodaj(usingi);
        }
    }
}
