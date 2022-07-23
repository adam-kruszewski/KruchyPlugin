using System;
using System.IO;
using System.Linq;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace Kruchy.Plugin.Utils.Menu
{
    public static class DostepnoscPozycjiMenuExtensions
    {
        public static bool SpelnioneWymaganie(
            this IPozycjaMenu pozycjaMenu,
            ISolutionWrapper solution)
        { 
            return pozycjaMenu.Wymagania.All(o => Spelnione(o, solution));
        }

        public static bool Spelnione(WymaganieDostepnosci o, ISolutionWrapper solution)
        {
            if (solution.AktualnyProjekt == null)
                return false;

            if (solution.AktualnyPlik == null)
                return false;

            if (o == WymaganieDostepnosci.DomainObject)
            {
                var aktualnyPlik = solution.AktualnyPlik;
                var di = new DirectoryInfo(aktualnyPlik.Katalog);
                if (di.Name.ToLower() != "domain")
                    return false;
            }
            if (o == WymaganieDostepnosci.Projekt)
            {
                return solution.AktualnyProjekt != null;
            }
            if (o == WymaganieDostepnosci.Modul)
            {
                if (solution.AktualnyProjekt == null)
                    return false;
                return !solution.AktualnyProjekt.Name.ToLower().EndsWith(".tests");
            }
            if (o == WymaganieDostepnosci.KlasaTestowa)
            {
                return solution.AktualnyPlik.Nazwa.ToLower().EndsWith("tests.cs");
            }
            if (o == WymaganieDostepnosci.PlikCs)
            {
                return PlikCs(solution);
            }
            if (o == WymaganieDostepnosci.Controller)
            {
                return solution.AktualnyPlik.Nazwa.ToLower().EndsWith("controller.cs");
            }

            if (o == WymaganieDostepnosci.Klasa)
            {
                var p = Parsuj(solution);
                if (p == null || p.DefiniowaneObiekty.Count < 1)
                    return false;
                return p.DefiniowaneObiekty.First().Rodzaj == RodzajObiektu.Klasa;
            }

            if (o == WymaganieDostepnosci.Interfejs)
            {
                var p = Parsuj(solution);
                if (p == null || p.DefiniowaneObiekty.Count < 1)
                    return false;
                return p.DefiniowaneObiekty.First().Rodzaj == RodzajObiektu.Interfejs;
            }

            if (o == WymaganieDostepnosci.Builder)
            {
                return solution.AktualnyPlik.Nazwa.ToLower().EndsWith("builder.cs");
            }

            if (o == WymaganieDostepnosci.WidokCshtml)
            {
                return solution.AktualnyPlik.Nazwa.ToLower().EndsWith(".cshtml");
            }

            if (o == WymaganieDostepnosci.PlikDao)
                return solution.AktualnyPlik.NazwaBezRozszerzenia.EndsWith("Dao");

            return true;
        }

        private static bool PlikCs(ISolutionWrapper solution)
        {
            return solution.AktualnyPlik.Nazwa.ToLower().EndsWith(".cs");
        }

        private static Plik Parsuj(ISolutionWrapper solution)
        {
            try
            {
                if (!PlikCs(solution))
                    return null;

                return Parser.Parsuj(solution.AktualnyDokument.GetContent());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd parsowania " + ex);
                return null;
            }
        }

    }
}

