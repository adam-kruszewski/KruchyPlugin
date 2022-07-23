﻿using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class PodzielParametryNaLinie
    {
        private readonly ISolutionWrapper solution;

        public PodzielParametryNaLinie(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Podziel()
        {
            var dokument = solution.AktualnyDokument;
            var parsowane =
                Parser.Parsuj(dokument.GetContent());

            var metoda = parsowane
                    .SzukajMetodyWLinii(dokument.GetCursorLineNumber());

            if (metoda == null)
            {
                var konstruktor =
                    parsowane
                        .SzukajKonstruktoraWLinii(dokument.GetCursorLineNumber());
                if (konstruktor != null)
                    PodzielNaLinieKonstruktor(konstruktor);
                else
                    MessageBox.Show("Kursor nie jest w metodzie");
                return;
            }

            dokument.Remove(
                metoda.NawiasOtwierajacyParametry.Wiersz,
                metoda.NawiasOtwierajacyParametry.Kolumna,
                metoda.NawiasZamykajacyParametry.Wiersz,
                metoda.NawiasZamykajacyParametry.Kolumna + 1);

            dokument.InsertInPlace(
                GenerujNoweParametry(metoda.Parametry, metoda, metoda),
                metoda.NawiasOtwierajacyParametry.Wiersz,
                metoda.NawiasOtwierajacyParametry.Kolumna);
        }

        private void PodzielNaLinieKonstruktor(Konstruktor konstruktor)
        {
            var dokument = solution.AktualnyDokument;

            dokument.Remove(
                konstruktor.NawiasOtwierajacyParametry.Wiersz,
                konstruktor.NawiasOtwierajacyParametry.Kolumna,
                konstruktor.NawiasZamykajacyParametry.Wiersz,
                konstruktor.NawiasZamykajacyParametry.Kolumna + 1);

            dokument.InsertInPlace(
                GenerujNoweParametry(konstruktor.Parametry, konstruktor),
                konstruktor.NawiasOtwierajacyParametry.Wiersz,
                konstruktor.NawiasOtwierajacyParametry.Kolumna);
        }

        private string GenerujNoweParametry(
            IEnumerable<Parametr> parametryMetody,
            IZWlascicielem obiekt,
            Metoda metoda = null)
        {
            var builder = new StringBuilder();
            builder.Append("(");
            var parametry =
                parametryMetody
                        .Select(o => DajDefinicjeParametru(o))
                            .ToArray();
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
            }
            builder.Append(string.Join(lacznik, parametry));
            builder.Append(")");
            return builder.ToString();
        }

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

            foreach (var atrybut in parametr.Atrybuty)
            {
                var atrybutBuilder = new AtrybutBuilder().ZNazwa(atrybut.Nazwa);
                foreach (var parametrAtrybutu in atrybut.Parametry)
                {
                    atrybutBuilder.DodajWartoscParametruNieStringowa(parametrAtrybutu.Wartosc);
                }

                builder.Append(atrybutBuilder.Build(true));
                builder.Append(" ");
            }

            if (parametr.ZThisem)
                builder.Append("this ");

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
    }
}
