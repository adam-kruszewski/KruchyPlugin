using System;
using System.Linq;
using System.Text;
using KrucheBuilderyKodu.Builders;
using KruchyCompany.KruchyPlugin1.Extensions;
using KruchyCompany.KruchyPlugin1.Utils;
using KruchyParserKodu.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class InicjowaniePolaWKonstruktorze
    {
        private readonly SolutionWrapper solution;

        public InicjowaniePolaWKonstruktorze(
            SolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Inicjuj()
        {
            string nazwaDoZainicjowania = null;
            string typDoZainicjowania = null;

            var parsowane =
                solution.ParsujZawartoscAktualnegoDokumetu();
            int numerLinii = solution.AktualnyDokument.DajNumerLiniiKursora();
            var properties =
                parsowane
                    .SzukajPropertiesaWLinii(numerLinii);

            if (properties != null)
            {
                nazwaDoZainicjowania = properties.Nazwa;
                typDoZainicjowania = properties.NazwaTypu;
            }else
            {
                var pole = parsowane.SzukajPolaWLinii(numerLinii);
                if (pole == null)
                    return;

                nazwaDoZainicjowania = pole.Nazwa;
                typDoZainicjowania = pole.NazwaTypu;
            }

            Inicjuj(nazwaDoZainicjowania, typDoZainicjowania, parsowane, numerLinii);
        }

        private void Inicjuj(string nazwa, string typ, Plik parsowane, int numerLinii)
        {
            var klasa = parsowane.SzukajKlasyWLinii(numerLinii);

            if (klasa.Konstruktory.Any())
            {
                var konstruktor = klasa.Konstruktory.First();
                solution.AktualnyDokument.WstawWLinii(
                    DajZawartoscDoDodania(nazwa, typ, true),
                    konstruktor.KoncowaKlamerka.Wiersz);
            }else
            {
                var zawartoscKontruktora =
                    GenerujZawartoscKontruktora(
                        klasa,
                        DajZawartoscDoDodania(nazwa, typ, false));
                solution.AktualnyDokument.WstawWLinii(
                    new StringBuilder().AppendLine() + zawartoscKontruktora,
                    parsowane.SzukajPierwszejLiniiDlaKonstruktora(numerLinii));
            }
        }

        private string DajZawartoscDoDodania(string nazwa, string typ, bool koncowyEnter)
        {
            var builder = new StringBuilder();
            builder.Append(StaleDlaKodu.WciecieDlaZawartosciMetody);
            builder.Append(nazwa);
            builder.Append(" = new ");
            builder.Append(typ);
            builder.Append("();");
            if (koncowyEnter)
                builder.AppendLine();
            return builder.ToString();
        }

        private string GenerujZawartoscKontruktora(
            Obiekt klasa,
            string zawartoscDoDodania)
        {
            var builder = new MetodaBuilder();
            builder.JedenParametrWLinii(false);
            builder.ZNazwa(klasa.Nazwa);
            builder.ZTypemZwracanym("");
            builder.DodajModyfikator("public");
            builder.DodajLinie(zawartoscDoDodania.TrimStart());
            var zawartoscKontruktora = builder.Build(StaleDlaKodu.WciecieDlaMetody);
            return zawartoscKontruktora;
        }
    }
}