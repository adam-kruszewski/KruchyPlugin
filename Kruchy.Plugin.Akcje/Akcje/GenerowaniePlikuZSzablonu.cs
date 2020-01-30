using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Akcje
{
    public class GenerowaniePlikuZSzablonu
    {
        private readonly ISolutionExplorerWrapper solutionExplorer;
        private readonly ISolutionWrapper soluton;

        public GenerowaniePlikuZSzablonu(
            ISolutionExplorerWrapper solutionExplorer,
            ISolutionWrapper soluton)
        {
            this.solutionExplorer = solutionExplorer;
            this.soluton = soluton;
        }

        public void Generuj(string nazwaSzablonu)
        {
            var konf = Konfiguracja.GetInstance(soluton);

            var szablon =
                konf
                    .SchematyGenerowania()
                        .Single(o => o.TytulSchematu == nazwaSzablonu);

            foreach (var schematKlasy in szablon.SchematyKlas)
            {
                GenerujWgSchmatu(schematKlasy);
            }
        }

        private void GenerujWgSchmatu(SchematKlasy schematKlasy)
        {
            var sciezkaDoPliku =
                Path.Combine(
                    soluton.AktualnyProjekt.SciezkaDoKatalogu,
                    schematKlasy.NazwaPliku);

            var tresc = schematKlasy.Tresc;

            File.WriteAllText(sciezkaDoPliku, tresc, Encoding.UTF8);
        }
    }
}