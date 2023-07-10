using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace Kruchy.Plugin.Akcje.Menu
{
    class PozycjaDodawanieUsingow : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaDodawanieUsingow(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidDodajNaczesciejUzywaneUsingi; }
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.PlikCs;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            var konf = Konfiguracja.GetInstance(solution);
            var aktualnaZawartosc = solution.CurenctDocument.GetContent();
            var aktualnyNamespace = Parser.Parse(aktualnaZawartosc).Namespace;

            var usingi =
                konf.DajKonfiguracjeUsingow(solution)
                    .NajczesciejUzywane
                        .Where(o => PasujeDoNamespaca(o, aktualnyNamespace))
                            .Select(o => DajNazweDoWstawienia(o))
                                .ToArray();

            new DodawanieUsinga(solution).Dodaj(usingi);
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
            return solution.CurrentProject.Name;
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
