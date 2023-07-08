using System;
using System.IO;
using System.Linq;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;

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
                var di = new DirectoryInfo(aktualnyPlik.Directory);
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
                return solution.AktualnyPlik.Name.ToLower().EndsWith("tests.cs");
            }
            if (o == WymaganieDostepnosci.PlikCs)
            {
                return PlikCs(solution);
            }
            if (o == WymaganieDostepnosci.Controller)
            {
                return solution.AktualnyPlik.Name.ToLower().EndsWith("controller.cs");
            }

            if (o == WymaganieDostepnosci.Klasa)
            {
                var p = Parsuj(solution);
                if (p == null || p.DefinedItems.Count < 1)
                    return false;
                return p.DefinedItems.First().KindOfItem == KindOfItem.Class;
            }

            if (o == WymaganieDostepnosci.Interfejs)
            {
                var p = Parsuj(solution);
                if (p == null || p.DefinedItems.Count < 1)
                    return false;
                return p.DefinedItems.First().KindOfItem == KindOfItem.Interface;
            }

            if (o == WymaganieDostepnosci.Builder)
            {
                return solution.AktualnyPlik.Name.ToLower().EndsWith("builder.cs");
            }

            if (o == WymaganieDostepnosci.WidokCshtml)
            {
                return solution.AktualnyPlik.Name.ToLower().EndsWith(".cshtml");
            }

            if (o == WymaganieDostepnosci.PlikDao)
                return solution.AktualnyPlik.NameWithoutExtension.EndsWith("Dao");

            return true;
        }

        private static bool PlikCs(ISolutionWrapper solution)
        {
            return solution.AktualnyPlik.Name.ToLower().EndsWith(".cs");
        }

        private static FileWithCode Parsuj(ISolutionWrapper solution)
        {
            try
            {
                if (!PlikCs(solution))
                    return null;

                return Parser.Parse(solution.AktualnyDokument.GetContent());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd parsowania " + ex);
                return null;
            }
        }

    }
}

