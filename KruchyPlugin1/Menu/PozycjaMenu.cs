﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using KruchyCompany.KruchyPlugin1.Utils;
using KruchyParserKodu.ParserKodu;
using Microsoft.VisualStudio.Shell;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    abstract class PozycjaMenu
    {
        protected readonly SolutionWrapper solution;
        abstract protected uint MenuCommandID { get; }
        protected virtual IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                return new List<WymaganieDostepnosci>();
            }
        }

        OleMenuCommand MenuItem { get; set; }

        public PozycjaMenu(SolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Podlacz(OleMenuCommandService service)
        {
            var menuCommandID =
                new CommandID(
                    GuidList.guidKruchyPlugin1CmdSet,
                    (int)MenuCommandID);
            MenuItem = new OleMenuCommand(Execute, menuCommandID);
            MenuItem.BeforeQueryStatus += BeforeQueryStatus;
            service.AddCommand(MenuItem);
        }

        void BeforeQueryStatus(object sender, EventArgs e)
        {
            if (!Wymagania.All(o => Spelnione(o)))
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
                return solution.AktualnyPlik.Nazwa.ToLower().EndsWith(".cs");
            }
            if (o == WymaganieDostepnosci.Controller)
            {
                return solution.AktualnyPlik.Nazwa.ToLower().EndsWith("controller.cs");
            }

            if (o == WymaganieDostepnosci.Klasa)
            {
                var p = Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());
                if (p.DefiniowaneObiekty.Count < 1)
                    return false;
                return p.DefiniowaneObiekty.First().Rodzaj == RodzajObiektu.Klasa;
            }

            if (o == WymaganieDostepnosci.Interfejs)
            {
                var p = Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());
                if (p.DefiniowaneObiekty.Count < 1)
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

        abstract protected void Execute(object sender, EventArgs args);
    }
}