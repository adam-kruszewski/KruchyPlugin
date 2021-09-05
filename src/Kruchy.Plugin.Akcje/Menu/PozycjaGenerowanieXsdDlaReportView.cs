using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.Akcje.Generowanie.Xsd.Komponenty;
using Kruchy.Plugin.Akcje.Interfejs;
using Kruchy.Plugin.Pincasso.Akcje.Atrybuty;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kruchy.Plugin.Akcje.Menu
{
    [SpecyficzneDlaPincasso]
    public class PozycjaGenerowanieXsdDlaReportView : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public PozycjaGenerowanieXsdDlaReportView(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidGenerujXsdDlaReportView; }
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get { yield return WymaganieDostepnosci.PlikCs; }
        }

        public void Execute(object sender, EventArgs args)
        {
            new GenerowanieXsdDlaReportView(solution, solutionExplorer)
                .Generuj(new ParametryGenerowaniaXsd(solution));
        }

        private class ParametryGenerowaniaXsd : IParametryGenerowaniaXsd
        {
            private readonly ISolutionWrapper solution;

            public ParametryGenerowaniaXsd(ISolutionWrapper solution)
            {
                this.solution = solution;
            }

            public string SciezkaDoXsd
            {
                get
                {
                    var aktualnaZawartosc = solution.AktualnyDokument.DajZawartosc();

                    var regex = new Regex(@"//sciezka_do_xsd=([A-Za-z0-9_\\/.]+)");
                    var match = regex.Match(aktualnaZawartosc);

                    if (match.Success)
                    {
                        var wyniki = new List<string>();
                        for (int i = 0; i < match.Groups.Count; i++)
                            if (!match.Groups[i].Value.Contains("//sciezka_do_xsd="))
                                wyniki.Add(match.Groups[i].Value);

                        if (wyniki.Count == 1)
                            return
                                Path.Combine(
                                    solution.AktualnyProjekt.SciezkaDoKatalogu,
                                    wyniki.Single());
                    }

                    var okno = new NazwaKlasyWindow();
                    okno.EtykietaNazwyPliku = "Ścieżka do xsd";

                    okno.ShowDialog();
                    if (!string.IsNullOrEmpty(okno.NazwaPliku))
                    {
                        return Path.Combine(
                            solution.AktualnyProjekt.SciezkaDoKatalogu,
                            okno.NazwaPliku);
                    }
                    return okno.NazwaPliku;
                }
            }
        }
    }
}
