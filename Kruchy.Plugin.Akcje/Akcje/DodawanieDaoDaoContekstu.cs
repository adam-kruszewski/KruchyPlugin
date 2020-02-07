using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace Kruchy.Plugin.Akcje.Akcje
{
    public class DodawanieDaoDaoContekstu
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public DodawanieDaoDaoContekstu(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public void Dodaj()
        {
            var sparsowane = Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());

            var obiekt =
                sparsowane.SzukajObiektuWLinii(solution.AktualnyDokument.DajNumerLiniiKursora());

            if (obiekt == null && sparsowane.DefiniowaneObiekty.Count() == 1)
                obiekt = sparsowane.DefiniowaneObiekty.Single();

            if (obiekt == null)
            {
                MessageBox.Show("Nie znaleziono obiektu");
                return;
            }

            string nazwaKlasyDao = null;
            string nazwaInterfejsuDao = null;
            if (obiekt.Rodzaj == RodzajObiektu.Klasa)
            {
                nazwaKlasyDao = obiekt.Nazwa;
                nazwaInterfejsuDao = "I" + nazwaKlasyDao;
            }

            if (obiekt.Rodzaj == RodzajObiektu.Interfejs)
            {
                nazwaInterfejsuDao = obiekt.Nazwa;
                nazwaKlasyDao = nazwaInterfejsuDao.Substring(1);
            }

            var plikIDao =
                solution.AktualnyProjekt.
                    Pliki.SingleOrDefault(o => o.Nazwa == nazwaInterfejsuDao + ".cs");

            var plikDao =
                solution.AktualnyProjekt
                    .Pliki.SingleOrDefault(o => o.Nazwa == nazwaKlasyDao + ".cs");

            var sciezkaDoIContext = SzukajSciezkiDoIContext();
            var sciezkaDoContext = SzukajSciezkiDoContext();

            if (string.IsNullOrEmpty(sciezkaDoIContext) ||
                string.IsNullOrEmpty(sciezkaDoContext))
            {
                MessageBox.Show("Nie znaleziono plików IContext lub Context");
                return;
            }

            //najpierw IContext
            UzupelniejIContext(sciezkaDoIContext, nazwaInterfejsuDao, nazwaKlasyDao, plikIDao);

            UzupelnijContext(
                sciezkaDoContext,
                nazwaInterfejsuDao,
                nazwaKlasyDao,
                plikIDao,
                plikDao);
        }

        private void UzupelnijContext(
            string sciezkaDoContext,
            string nazwaInterfejsuDao,
            string nazwaKlasyDao,
            IPlikWrapper plikIDao,
            IPlikWrapper plikDao)
        {
            solutionExplorer.OtworzPlik(sciezkaDoContext);

            var dokument = solution.AktualnyDokument;

            var sparsowaneDao = Parser.ParsujPlik(plikDao.SciezkaPelna);
            var sparsowaneIDao = Parser.ParsujPlik(plikIDao.SciezkaPelna);

            dokument.DodajUsingaJesliTrzeba(sparsowaneDao.Namespace);
            dokument.DodajUsingaJesliTrzeba(sparsowaneIDao.Namespace);
            dokument.DodajUsingaJesliTrzeba("Pincasso.Core.Base");

            dokument.WstawWLinii(
                    string.Format("{0}public {1} {2} ",StaleDlaKodu.WciecieDlaMetody, nazwaInterfejsuDao, nazwaKlasyDao)
                     + "{ get { " +
                     string.Format("return GetDao<{0}>();", nazwaKlasyDao)
                     + " } }",
                    SzukajLiniiDoDodanieMetody(dokument, nazwaInterfejsuDao));
        }

        private void UzupelniejIContext(
            string sciezkaDoIContext,
            string nazwaInterfejsuDao,
            string nazwaKlasyDao,
            IPlikWrapper plikIDao)
        {
            solutionExplorer.OtworzPlik(sciezkaDoIContext);

            var dokument = solution.AktualnyDokument;

            var sparsowaneIDao = Parser.ParsujPlik(plikIDao.SciezkaPelna);

            dokument.DodajUsingaJesliTrzeba(sparsowaneIDao.Namespace);

            dokument.WstawWLinii(
                string.Format("{0}{1} {2} ",
                    StaleDlaKodu.WciecieDlaMetody,
                    nazwaInterfejsuDao,
                    nazwaKlasyDao) + "{ get; }",
                SzukajLiniiDoDodanieMetody(dokument, nazwaInterfejsuDao));
        }

        private int SzukajLiniiDoDodanieMetody(
            IDokumentWrapper dokument,
            string nazwaInterfejsuDao)
        {
            var sparsowane = Parser.Parsuj(dokument.DajZawartosc());

            var metodaPo =
            sparsowane
                .DefiniowaneObiekty
                    .Single()
                        .Metody
                            .Where(o => o.TypZwracany.CompareTo(nazwaInterfejsuDao) > 0)
                                .FirstOrDefault();

            if (metodaPo == null)
                return sparsowane.DefiniowaneObiekty.Single().KoncowaKlamerka.Wiersz;
            else
                return metodaPo.Poczatek.Wiersz;
        }

        private string SzukajSciezkiDoContext()
        {
            return
                SzukajWgWyrazeniaRegularnego("(.)*Context.cs")
                    .OrderBy(o => o.Nazwa.Length)
                    .FirstOrDefault()
                        ?.SciezkaPelna;
        }

        private string SzukajSciezkiDoIContext()
        {
            return
                SzukajWgWyrazeniaRegularnego("I(.)*Context.cs")
                .OrderByDescending(o => o.Nazwa.Length)
                    .FirstOrDefault()
                        ?.SciezkaPelna;
        }

        private IEnumerable<IPlikWrapper> SzukajWgWyrazeniaRegularnego(string wyrazenie)
        {
            return
                solution
                    .AktualnyProjekt
                        .Pliki
                            .Where(o => PasujeDoWyrazenia(o, wyrazenie));
        }

        private bool PasujeDoWyrazenia(IPlikWrapper o, string wyrazenie)
        {
            var regex = new Regex(wyrazenie);
            return regex.Match(o.Nazwa).Success;
        }
    }
}
