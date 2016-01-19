using System.Linq;
using System.Text;
using System.Windows;
using KrucheBuilderyKodu.Builders;
using KruchyCompany.KruchyPlugin1.ParserKodu;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class PodzielParametryNaLinie
    {
        private readonly SolutionWrapper solution;

        public PodzielParametryNaLinie(SolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Podziel()
        {
            var dokument = solution.AktualnyDokument;
            var parsowane =
                Parser.Parsuj(dokument.DajZawartosc());

            var metoda = parsowane
                    .SzukajMetodyWLinii(dokument.DajNumerLiniiKursora());

            if (metoda == null)
            {
                MessageBox.Show("Kursor nie jest w metodzie");
                return;
            }

            dokument.Usun(
                metoda.NawiasOtwierajacyParametry.Wiersz,
                metoda.NawiasOtwierajacyParametry.Kolumna,
                metoda.NawiasZamykajacyParametry.Wiersz,
                metoda.NawiasZamykajacyParametry.Kolumna + 1);

            dokument.WstawWMiejscu(
                GenerujNoweParametry(metoda),
                metoda.NawiasOtwierajacyParametry.Wiersz,
                metoda.NawiasOtwierajacyParametry.Kolumna);
        }

        private string GenerujNoweParametry(Metoda metoda)
        {
            var builder = new StringBuilder();
            builder.Append("(");
            var parametry =
                metoda
                    .Parametry
                        .Select(o => o.NazwaTypu + " " + o.NazwaParametru)
                            .ToArray();
            var lacznik = ",\n" + StaleDlaKodu.WcieciaDlaParametruMetody;
            if (parametry.Count() > 0)
                builder.Append("\n" + StaleDlaKodu.WcieciaDlaParametruMetody);
            builder.Append(string.Join(lacznik, parametry));
            builder.Append(")");
            return builder.ToString();
        }
    }
}
