using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using Microsoft.VisualStudio.Shell;

namespace Kruchy.Plugin.Utils._2017
{
    public class PozycjaMenuAdapter
    {
        public static Guid guidKruchyPluginCmdSetStatic;

        private readonly IPozycjaMenu pozycjaMenu;
        private readonly ISolutionWrapper solution;

        OleMenuCommand MenuItem { get; set; }

        public PozycjaMenuAdapter(IPozycjaMenu pozycjaMenu, ISolutionWrapper solution)
        {
            this.pozycjaMenu = pozycjaMenu;
            this.solution = solution;
        }

        public void Podlacz(IMenuCommandService service)
        {
            Podlacz(service, guidKruchyPluginCmdSetStatic);
        }

        public void Podlacz(IMenuCommandService service, Guid guidKruchyPluginCmdSet)
        {
            var menuCommandID =
                new CommandID(guidKruchyPluginCmdSet, (int)pozycjaMenu.MenuCommandID);
            if (!(pozycjaMenu is IPozycjaMenuDynamicznieRozwijane))
                MenuItem = new OleMenuCommand(Execute, menuCommandID);
            else
                MenuItem = new DynamicItemMenuCommand(
                    menuCommandID, (IPozycjaMenuDynamicznieRozwijane)pozycjaMenu);
            MenuItem.BeforeQueryStatus += BeforeQueryStatus;
            service.AddCommand(MenuItem);
        }

        void BeforeQueryStatus(object sender, EventArgs e)
        {
            if (!pozycjaMenu.Wymagania.All(o => Spelnione(o)))
            {
                this.MenuItem.Enabled = false;
            }
            else
            {
                this.MenuItem.Enabled = true;
            }
        }

        private bool Spelnione(WymaganieDostepnosci o)
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
                return !solution.AktualnyProjekt.Nazwa.ToLower().EndsWith(".tests");
            }
            if (o == WymaganieDostepnosci.KlasaTestowa)
            {
                return solution.AktualnyPlik.Nazwa.ToLower().EndsWith("tests.cs");
            }
            if (o == WymaganieDostepnosci.PlikCs)
            {
                return PlikCs();
            }
            if (o == WymaganieDostepnosci.Controller)
            {
                return solution.AktualnyPlik.Nazwa.ToLower().EndsWith("controller.cs");
            }

            if (o == WymaganieDostepnosci.Klasa)
            {
                var p = Parsuj();
                if (p == null || p.DefiniowaneObiekty.Count < 1)
                    return false;
                return p.DefiniowaneObiekty.First().Rodzaj == RodzajObiektu.Klasa;
            }

            if (o == WymaganieDostepnosci.Interfejs)
            {
                var p = Parsuj();
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
            return true;
        }

        private bool PlikCs()
        {
            return solution.AktualnyPlik.Nazwa.ToLower().EndsWith(".cs");
        }

        private Plik Parsuj()
        {
            try
            {
                if (!PlikCs())
                    return null;

                return Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());
            }catch (Exception ex)
            {
                Console.WriteLine("Błąd parsowania " + ex);
                return null;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            pozycjaMenu.Execute(sender, args);
        }
    }
}