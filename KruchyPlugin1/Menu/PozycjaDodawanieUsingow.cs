using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.KonfiguracjaPlugina;
using KruchyCompany.KruchyPlugin1.Utils;
using KruchyParserKodu.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaDodawanieUsingow : PozycjaMenu
    {
        public PozycjaDodawanieUsingow(SolutionWrapper solution)
            : base(solution) { }

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
            var konf = Konfiguracja.GetInstance(solution);
            var aktualnaZawartosc = solution.AktualnyDokument.DajZawartosc();
            var aktualnyNamespace = Parser.Parsuj(aktualnaZawartosc).Namespace;

            var usingi =
                konf.DajKonfiguracjeUsingow(solution)
                    .NajczesciejUzywane
                        .Where(o => PasujeDoNamespaca(o, aktualnyNamespace))
                            .Select(o => o.Nazwa)
                                .ToArray();

            new DodawaniaUsinga(solution).Dodaj(usingi);
        }

        private bool PasujeDoNamespaca(
            NajczesciejUzywanyUsing uzywanyUsing,
            string aktualnyNamespace)
        {
            if (string.IsNullOrEmpty(uzywanyUsing.NamespaceUzycia))
                return true;
            var regex = new Regex(uzywanyUsing.NamespaceUzycia);
            return regex.IsMatch(aktualnyNamespace);
        }
    }
}
