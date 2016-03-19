using System.Linq; using System.Text; using System.Windows; using KrucheBuilderyKodu.Builders; using KruchyCompany.KruchyPlugin1.ParserKodu; using KruchyCompany.KruchyPlugin1.Utils;  namespace KruchyCompany.KruchyPlugin1.Akcje {     class PodzielParametryNaLinie     {         private readonly SolutionWrapper solution;          public PodzielParametryNaLinie(SolutionWrapper solution)         {             this.solution = solution;         }          public void Podziel()         {             var dokument = solution.AktualnyDokument;             var parsowane =                 Parser.Parsuj(dokument.DajZawartosc());              var metoda = parsowane                     .SzukajMetodyWLinii(dokument.DajNumerLiniiKursora());              if (metoda == null)             {                 MessageBox.Show("Kursor nie jest w metodzie");                 return;             }              dokument.Usun(                 metoda.NawiasOtwierajacyParametry.Wiersz,                 metoda.NawiasOtwierajacyParametry.Kolumna,                 metoda.NawiasZamykajacyParametry.Wiersz,                 metoda.NawiasZamykajacyParametry.Kolumna + 1);              dokument.WstawWMiejscu(                 GenerujNoweParametry(metoda),                 metoda.NawiasOtwierajacyParametry.Wiersz,                 metoda.NawiasOtwierajacyParametry.Kolumna);         }          private string GenerujNoweParametry(Metoda metoda)         {             var builder = new StringBuilder();             builder.Append("(");
            var parametry =                 metoda                     .Parametry
                        .Select(o => DajDefinicjeParametru(o))                             .ToArray();
            var lacznikBuilder =
                new StringBuilder()
                    .Append(",")
                    .AppendLine()
                    .Append(StaleDlaKodu.WcieciaDlaParametruMetody);
            var lacznik = lacznikBuilder.ToString();
            if (parametry.Any())
            {
                builder.AppendLine();
                builder.Append(StaleDlaKodu.WcieciaDlaParametruMetody);
                DodajThisJesliTrzeba(builder, metoda);
            }             builder.Append(string.Join(lacznik, parametry));             builder.Append(")");             return builder.ToString();         }

        private string DajDefinicjeParametru(Parametr parametr)
        {
            return
                parametr.NazwaTypu + " " +
                parametr.NazwaParametru
                + DajOpisWartosciDomyslnej(parametr);
        }

        private string DajOpisWartosciDomyslnej(Parametr parametr)
        {
            if (!string.IsNullOrEmpty(parametr.WartoscDomyslna))
                return " = " + parametr.WartoscDomyslna;
            else
                return string.Empty;
        }

        private void DodajThisJesliTrzeba(StringBuilder builder, Metoda metoda)
        {
            if (metoda.Parametry.Any() && metoda.Parametry.First().ZThisem)
                builder.Append("this ");
        }     } } 