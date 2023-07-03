using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Kruchy.Plugin.Pincasso.Akcje.Akcje
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
            var sparsowane = Parser.Parsuj(solution.AktualnyDokument.GetContent());

            var obiekt =
                sparsowane.SzukajObiektuWLinii(solution.AktualnyDokument.GetCursorLineNumber());

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
                nazwaKlasyDao = obiekt.Name;
                nazwaInterfejsuDao = "I" + nazwaKlasyDao;
            }

            if (obiekt.Rodzaj == RodzajObiektu.Interfejs)
            {
                nazwaInterfejsuDao = obiekt.Name;
                nazwaKlasyDao = nazwaInterfejsuDao.Substring(1);
            }

            var plikIDao =
                solution.AktualnyProjekt.
                    Files.SingleOrDefault(o => o.Name == nazwaInterfejsuDao + ".cs");

            var plikDao =
                solution.AktualnyProjekt
                    .Files.SingleOrDefault(o => o.Name == nazwaKlasyDao + ".cs");

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
            IFileWrapper plikIDao,
            IFileWrapper plikDao)
        {
            solutionExplorer.OpenFile(sciezkaDoContext);

            var dokument = solution.AktualnyDokument;

            var sparsowaneDao = Parser.ParsujPlik(plikDao.FullPath);
            var sparsowaneIDao = Parser.ParsujPlik(plikIDao.FullPath);

            dokument.DodajUsingaJesliTrzeba(sparsowaneDao.Namespace);
            dokument.DodajUsingaJesliTrzeba(sparsowaneIDao.Namespace);
            dokument.DodajUsingaJesliTrzeba("Pincasso.Core.Base");

            dokument.InsertInLine(
                    string.Format("{0}public {1} {2} ", StaleDlaKodu.WciecieDlaMetody, nazwaInterfejsuDao, nazwaKlasyDao)
                     + "{ get { " +
                     string.Format("return GetDao<{0}>();", nazwaKlasyDao)
                     + " } }",
                    SzukajLiniiDoDodanieMetody(dokument, nazwaInterfejsuDao));
        }

        private void UzupelniejIContext(
            string sciezkaDoIContext,
            string nazwaInterfejsuDao,
            string nazwaKlasyDao,
            IFileWrapper plikIDao)
        {
            solutionExplorer.OpenFile(sciezkaDoIContext);

            var dokument = solution.AktualnyDokument;

            var sparsowaneIDao = Parser.ParsujPlik(plikIDao.FullPath);

            dokument.DodajUsingaJesliTrzeba(sparsowaneIDao.Namespace);

            dokument.InsertInLine(
                string.Format("{0}{1} {2} ",
                    StaleDlaKodu.WciecieDlaMetody,
                    nazwaInterfejsuDao,
                    nazwaKlasyDao) + "{ get; }",
                SzukajLiniiDoDodanieMetody(dokument, nazwaInterfejsuDao));
        }

        private int SzukajLiniiDoDodanieMetody(
            IDocumentWrapper dokument,
            string nazwaInterfejsuDao)
        {
            var sparsowane = Parser.Parsuj(dokument.GetContent());

            var metodaPo =
            sparsowane
                .DefiniowaneObiekty
                    .Single()
                        .Metody
                            .Where(o => o.TypZwracany.Nazwa.CompareTo(nazwaInterfejsuDao) > 0)
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
                    .OrderBy(o => o.Name.Length)
                    .FirstOrDefault()
                        ?.FullPath;
        }

        private string SzukajSciezkiDoIContext()
        {
            return
                SzukajWgWyrazeniaRegularnego("I(.)*Context.cs")
                .OrderByDescending(o => o.Name.Length)
                    .FirstOrDefault()
                        ?.FullPath;
        }

        private IEnumerable<IFileWrapper> SzukajWgWyrazeniaRegularnego(string wyrazenie)
        {
            return
                solution
                    .AktualnyProjekt
                        .Files
                            .Where(o => PasujeDoWyrazenia(o, wyrazenie));
        }

        private bool PasujeDoWyrazenia(IFileWrapper o, string wyrazenie)
        {
            var regex = new Regex(wyrazenie);
            return regex.Match(o.Name).Success;
        }
    }
}
