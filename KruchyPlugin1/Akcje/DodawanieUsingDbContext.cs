using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using KrucheBuilderyKodu.Builders;
using KruchyCompany.KruchyPlugin1.Utils;
using KruchyParserKodu.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class DodawanieUsingDbContext
    {
        private readonly SolutionWrapper solution;

        public DodawanieUsingDbContext(SolutionWrapper solution)
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

            var nazwaKlasyContextu = parsowane.DefiniowaneObiekty.First().Nazwa;
            var namespaceKlasy = parsowane.Namespace;

            var aktualnyDokument = solution.AktualnyDokument;
            aktualnyDokument.DodajUsingaJesliTrzeba(namespaceKlasy);
            aktualnyDokument.DodajUsingaJesliTrzeba("Pincasso.Core.Base");

            var parsowaneAktualny =
                Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());
            DodajAtrybutContext(nazwaKlasyContextu, parsowaneAktualny);
            DodajInterfejsUsingContext(nazwaKlasyContextu, parsowaneAktualny);
        }

        private void DodajAtrybutContext(
            string nazwaKlasyContextu,
            Plik parsowaneAktualny)
        {
            var klasa = parsowaneAktualny.DefiniowaneObiekty.First();

            int numerLiniiWstawiania = -1;
            if (klasa.Propertiesy.Any())
                numerLiniiWstawiania =
                    klasa.Propertiesy.Select(o => o.Poczatek.Wiersz).Min();
            else
            {
                numerLiniiWstawiania = klasa.PoczatkowaKlamerka.Wiersz + 1;
            }

            var propBuilder =
                new PropertyBuilder()
                    .ZNazwa("Context")
                    .ZNazwaTypu(nazwaKlasyContextu)
                    .ZModyfikatorem("public");

            solution.AktualnyDokument.WstawWLinii(
                propBuilder.Build(StaleDlaKodu.WciecieDlaMetody),
                numerLiniiWstawiania);
        }

        private void DodajInterfejsUsingContext(
            string nazwaKlasyContextu,
            Plik parsowaneAktualny)
        {
            var klasa = parsowaneAktualny.DefiniowaneObiekty.First();
            if (klasa.NadklasaIInterfejsy.Any())
            {
                var ostatni = klasa.NadklasaIInterfejsy.Last();
                var wstawianyTekst = ", IUseDbDaoContext<" + nazwaKlasyContextu + ">";
                if (ostatni.Poczatek.Wiersz == klasa.Poczatek.Wiersz)
                {
                    solution.AktualnyDokument.
                        WstawWMiejscu(
                            wstawianyTekst,
                            ostatni.Poczatek.Wiersz,
                            ostatni.Koniec.Kolumna);
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.Append(StaleDlaKodu.WciecieDlaKlasy);
                    for (int i = 0; i <= klasa.NadklasaIInterfejsy.Count(); i++)
                        sb.Append(StaleDlaKodu.JednostkaWciecia);
                    sb.Append(wstawianyTekst);
                    sb.AppendLine();
                    solution.AktualnyDokument
                        .WstawWMiejscu(
                            sb.ToString(),
                            ostatni.Poczatek.Wiersz + 1,
                            1);
                }
            }else
            {
                MessageBox.Show("Bez dziedziczeń jeszcze nie obsługiwane");
                return;
            }
        }

        private string SzukajPlikuContextu(ProjektWrapper projekt)
        {
            var katalogBase = Path.Combine(projekt.SciezkaDoKatalogu, "Base");
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