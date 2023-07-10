using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
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
            var sparsowane = Parser.Parse(solution.CurentDocument.GetContent());

            var obiekt =
                sparsowane.FindDefinedItemByLineNumber(solution.CurentDocument.GetCursorLineNumber());

            if (obiekt == null && sparsowane.DefinedItems.Count() == 1)
                obiekt = sparsowane.DefinedItems.Single();

            if (obiekt == null)
            {
                MessageBox.Show("Nie znaleziono obiektu");
                return;
            }

            string nazwaKlasyDao = null;
            string nazwaInterfejsuDao = null;
            if (obiekt.KindOfItem == KindOfItem.Class)
            {
                nazwaKlasyDao = obiekt.Name;
                nazwaInterfejsuDao = "I" + nazwaKlasyDao;
            }

            if (obiekt.KindOfItem == KindOfItem.Interface)
            {
                nazwaInterfejsuDao = obiekt.Name;
                nazwaKlasyDao = nazwaInterfejsuDao.Substring(1);
            }

            var plikIDao =
                solution.CurrentProject.
                    Files.SingleOrDefault(o => o.Name == nazwaInterfejsuDao + ".cs");

            var plikDao =
                solution.CurrentProject
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

            var dokument = solution.CurentDocument;

            var sparsowaneDao = Parser.ParseFile(plikDao.FullPath);
            var sparsowaneIDao = Parser.ParseFile(plikIDao.FullPath);

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

            var dokument = solution.CurentDocument;

            var sparsowaneIDao = Parser.ParseFile(plikIDao.FullPath);

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
            var sparsowane = Parser.Parse(dokument.GetContent());

            var metodaPo =
            sparsowane
                .DefinedItems
                    .Single()
                        .Methods
                            .Where(o => o.ReturnType.Name.CompareTo(nazwaInterfejsuDao) > 0)
                                .FirstOrDefault();

            if (metodaPo == null)
                return sparsowane.DefinedItems.Single().ClosingBrace.Row;
            else
                return metodaPo.StartPosition.Row;
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
                    .CurrentProject
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
