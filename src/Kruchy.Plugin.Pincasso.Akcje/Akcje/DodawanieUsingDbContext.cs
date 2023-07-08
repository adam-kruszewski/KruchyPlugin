using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;

namespace Kruchy.Plugin.Pincasso.Akcje.Akcje
{
    public class DodawanieUsingDbContext
    {
        private readonly ISolutionWrapper solution;

        public DodawanieUsingDbContext(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Dodaj()
        {
            var projekt = solution.AktualnyProjekt;

            if (projekt == null)
            {
                MessageBox.Show("Brak aktualnego projektu");
                return;
            }

            var nazwaPlikuContextu = SzukajPlikuContextu(projekt);
            var parsowane = Parser.ParsujPlik(nazwaPlikuContextu);

            var nazwaKlasyContextu = parsowane.DefinedItems.First().Name;
            var namespaceKlasy = parsowane.Namespace;

            var aktualnyDokument = solution.AktualnyDokument;
            aktualnyDokument.DodajUsingaJesliTrzeba(namespaceKlasy);
            aktualnyDokument.DodajUsingaJesliTrzeba("Pincasso.Core.Base");

            var parsowaneAktualny =
                Parser.Parsuj(solution.AktualnyDokument.GetContent());
            DodajAtrybutContext(nazwaKlasyContextu, parsowaneAktualny);
            DodajInterfejsUsingContext(nazwaKlasyContextu, parsowaneAktualny);
        }

        private void DodajAtrybutContext(
            string nazwaKlasyContextu,
            FileWithCode parsowaneAktualny)
        {
            var klasa = parsowaneAktualny.DefinedItems.First();

            int numerLiniiWstawiania = -1;
            if (klasa.Properties.Any())
                numerLiniiWstawiania =
                    klasa.Properties.Select(o => o.StartPosition.Row).Min();
            else
            {
                numerLiniiWstawiania = klasa.StartingBrace.Row + 1;
            }

            var propBuilder =
                new PropertyBuilder()
                    .ZNazwa("Context")
                    .ZNazwaTypu(nazwaKlasyContextu)
                    .ZModyfikatorem("public");

            solution.AktualnyDokument.InsertInLine(
                propBuilder.Build(StaleDlaKodu.WciecieDlaMetody),
                numerLiniiWstawiania);
        }

        private void DodajInterfejsUsingContext(
            string nazwaKlasyContextu,
            FileWithCode parsowaneAktualny)
        {
            var klasa = parsowaneAktualny.DefinedItems.First();
            if (klasa.SuperClassAndInterfaces.Any())
            {
                var ostatni = klasa.SuperClassAndInterfaces.Last();
                var wstawianyTekst = ", IUseDbDaoContext<" + nazwaKlasyContextu + ">";
                if (ostatni.StartPosition.Row == klasa.StartPosition.Row)
                {
                    solution.AktualnyDokument.
                        InsertInPlace(
                            wstawianyTekst,
                            ostatni.StartPosition.Row,
                            ostatni.EndPosition.Column);
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.Append(StaleDlaKodu.WciecieDlaKlasy);
                    for (int i = 0; i <= klasa.SuperClassAndInterfaces.Count(); i++)
                        sb.Append(StaleDlaKodu.JednostkaWciecia);
                    sb.Append(wstawianyTekst);
                    sb.AppendLine();
                    solution.AktualnyDokument
                        .InsertInPlace(
                            sb.ToString(),
                            ostatni.StartPosition.Row + 1,
                            1);
                }
            }
            else
            {
                MessageBox.Show("Bez dziedziczeń jeszcze nie obsługiwane");
                return;
            }
        }

        private string SzukajPlikuContextu(IProjectWrapper projekt)
        {
            var katalogBase = Path.Combine(projekt.DirectoryPath, "Base");
            var pliki = Directory.GetFiles(katalogBase);
            var plikContextu =
                pliki
                    .Where(o => o.ToLower().EndsWith("context.cs"))
                    .OrderByDescending(o => o.Length)
                    .FirstOrDefault();
            if (plikContextu == null)
                return null;

            var fileInfo = new FileInfo(plikContextu);
            return fileInfo.FullName;
        }
    }
}
