using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.KonfiguracjaPlugina;
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
                            .Select(o => DajNazweDoWstawienia(o))
                                .ToArray();

            new DodawaniaUsinga(solution).Dodaj(usingi);
        }

        private string DajNazweDoWstawienia(NajczesciejUzywanyUsing o)
        {
            var wynik = o.Nazwa;

            //%NAZWA_MODULU%
            //%NAZWA_MODULU_TESTOWANEGO%
            var zmiany = new Dictionary<string, string>();
            zmiany["%NAZWA_MODULU%"] = DajNazweModulu();
            zmiany["%NAZWA_MODULU_TESTOWANEGO%"] = DajNazweModuluTestowanego();

            foreach (var klucz in zmiany.Keys)
                wynik = wynik.Replace(klucz, zmiany[klucz]);

            return wynik;
        }

        private string DajNazweModulu()
        {
            return solution.AktualnyProjekt.Nazwa;
        }

        private string DajNazweModuluTestowanego()
        {
            var nazwaModulu = DajNazweModulu();
            if (nazwaModulu.ToLower().EndsWith(".tests"))
                return nazwaModulu.Substring(0, nazwaModulu.Length - ".tests".Length);
            else
                return "";
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
