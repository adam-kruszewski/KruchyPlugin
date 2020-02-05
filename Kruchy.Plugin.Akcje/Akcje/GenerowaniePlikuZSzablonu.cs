using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

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

            tresc = ZamienZmienneNaWartosci(tresc, schematKlasy);

            File.WriteAllText(sciezkaDoPliku, tresc, Encoding.UTF8);
        }

        private string ZamienZmienneNaWartosci(string tresc, SchematKlasy schematKlasy)
        {
            var zmienne = PrzygotujWartosciZmiennych(schematKlasy);

            foreach (var zmienna in zmienne)
                tresc = tresc.Replace("%" + zmienna.Key + "%", zmienna.Value);

            return tresc;
        }

        private Dictionary<string, string> PrzygotujWartosciZmiennych(SchematKlasy schematKlasy)
        {
            var sparsowane = Parser.Parsuj(soluton.AktualnyDokument.DajZawartosc());

            var wynik = new Dictionary<string, string>();

            wynik["NAZWA_KLASY"] = DajNazweKlasy(sparsowane);
            wynik["NAMESPACE_KLASY"] = sparsowane.Namespace;
            wynik["NAZWA_PLIKU"] = soluton.AktualnyPlik.Nazwa;
            wynik["NAZWA_PLIKU_BEZ_ROZSZERZENIA"] =
                soluton.AktualnyPlik.NazwaBezRozszerzenia;

            return wynik;
        }

        private string DajNazweKlasy(Plik sparsowane)
        {
            var obiekt =
            sparsowane.SzukajKlasyWLinii(soluton.AktualnyDokument.DajNumerLiniiKursora());

            if (obiekt == null)
                return sparsowane.DefiniowaneObiekty.Single().Nazwa;
            else
                return obiekt.Nazwa;
        }
    }
}