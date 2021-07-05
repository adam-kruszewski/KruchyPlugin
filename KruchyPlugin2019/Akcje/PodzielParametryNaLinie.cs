using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;  namespace KruchyCompany.KruchyPlugin1.Akcje {     class PodzielParametryNaLinie     {         private readonly ISolutionWrapper solution;          public PodzielParametryNaLinie(ISolutionWrapper solution)         {             this.solution = solution;         }          public void Podziel()         {             var dokument = solution.AktualnyDokument;             var parsowane =                 Parser.Parsuj(dokument.DajZawartosc());              var metoda = parsowane                     .SzukajMetodyWLinii(dokument.DajNumerLiniiKursora());              if (metoda == null)             {
                var konstruktor =
                    parsowane
                        .SzukajKonstruktoraWLinii(dokument.DajNumerLiniiKursora());
                if (konstruktor != null)
                    PodzielNaLinieKonstruktor(konstruktor);                 else                     System.Windows.MessageBox.Show("Kursor nie jest w metodzie");                 return;             }              dokument.Usun(                 metoda.NawiasOtwierajacyParametry.Wiersz,                 metoda.NawiasOtwierajacyParametry.Kolumna,                 metoda.NawiasZamykajacyParametry.Wiersz,                 metoda.NawiasZamykajacyParametry.Kolumna + 1);              dokument.WstawWMiejscu(                 GenerujNoweParametry(metoda.Parametry, metoda, metoda),                 metoda.NawiasOtwierajacyParametry.Wiersz,                 metoda.NawiasOtwierajacyParametry.Kolumna);         }

        private void PodzielNaLinieKonstruktor(Konstruktor konstruktor)
        {
            var dokument = solution.AktualnyDokument;

            dokument.Usun(
                konstruktor.NawiasOtwierajacyParametry.Wiersz,
                konstruktor.NawiasOtwierajacyParametry.Kolumna,
                konstruktor.NawiasZamykajacyParametry.Wiersz,
                konstruktor.NawiasZamykajacyParametry.Kolumna + 1);

            dokument.WstawWMiejscu(
                GenerujNoweParametry(konstruktor.Parametry, konstruktor),
                konstruktor.NawiasOtwierajacyParametry.Wiersz,
                konstruktor.NawiasOtwierajacyParametry.Kolumna);
        }          private string GenerujNoweParametry(             IEnumerable<Parametr> parametryMetody,             IZWlascicielem obiekt,             Metoda metoda = null)         {             var builder = new StringBuilder();             builder.Append("(");
            var parametry =                 parametryMetody
                        .Select(o => DajDefinicjeParametru(o))                             .ToArray();
            var lacznikBuilder =
                new StringBuilder()
                    .Append(",")
                    .AppendLine()
                    .Append(StaleDlaKodu.WcieciaDlaParametruMetody);

            var poziomMetody = WyliczPoziomMetody(obiekt.Wlasciciel);

            DodajWciecieWgPoziomuMetody(lacznikBuilder, poziomMetody);

            var lacznik = lacznikBuilder.ToString();
            if (parametry.Any())
            {
                builder.AppendLine();
                builder.Append(StaleDlaKodu.WcieciaDlaParametruMetody);
                DodajWciecieWgPoziomuMetody(builder, poziomMetody);
                DodajThisJesliTrzeba(builder, metoda);
            }             builder.Append(string.Join(lacznik, parametry));             builder.Append(")");             return builder.ToString();         }

        private void DodajWciecieWgPoziomuMetody(
            StringBuilder lacznikBuilder,
            int poziomMetody)
        {
            if (poziomMetody > 1)
                for (int i = 0; i < poziomMetody - 1; i++)
                    lacznikBuilder.Append(StaleDlaKodu.JednostkaWciecia);
        }

        private int WyliczPoziomMetody(IZWlascicielem obiekt)
        {
            if (obiekt == null)
                return 0;

            if (obiekt.Wlasciciel == null)
                return 1;

            return WyliczPoziomMetody(obiekt.Wlasciciel) + 1;
        }

        private string DajDefinicjeParametru(Parametr parametr)
        {
            var builder = new StringBuilder();
            if (parametr.ZParams)
                builder.Append("params ");
            if (parametr.ZOut)
                builder.Append("out ");
            if (parametr.ZRef)
                builder.Append("ref ");

            builder.Append(parametr.NazwaTypu + " ");
            builder.Append(parametr.NazwaParametru);
            builder.Append(DajOpisWartosciDomyslnej(parametr));

            return builder.ToString();
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
            if (metoda == null)
                return;
            if (metoda.Parametry.Any() && metoda.Parametry.First().ZThisem)
                builder.Append("this ");
        }     } } 