using System.Linq;
using System.Text;
using KrucheBuilderyKodu.Builders;
using KrucheBuilderyKodu.Utils;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu;
using KruchyParserKodu.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class InicjowaniePolaWKonstruktorze
    {
        private readonly ISolutionWrapper solution;

        public InicjowaniePolaWKonstruktorze(
            ISolutionWrapper solution)
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
            }
            else
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

            var poziomKlasy = klasa.WyliczPoziomMetody();

            if (klasa.Konstruktory.Any())
            {
                var konstruktor = klasa.Konstruktory.First();
                solution.AktualnyDokument.WstawWLinii(
                    DajZawartoscDoDodania(nazwa, typ, true, poziomKlasy),
                    konstruktor.KoncowaKlamerka.Wiersz);
            }
            else
            {
                var zawartoscKontruktora =
                    GenerujZawartoscKontruktora(
                        klasa,
                        DajZawartoscDoDodania(nazwa, typ, false, poziomKlasy));
                solution.AktualnyDokument.WstawWLinii(
                    new StringBuilder().AppendLine() + zawartoscKontruktora,
                    parsowane.SzukajPierwszejLiniiDlaKonstruktora(numerLinii));
            }
        }

        private string DajZawartoscDoDodania(
            string nazwa,
            string typ,
            bool koncowyEnter,
            int poziomKlasy)
        {
            var builder = new StringBuilder();
            builder.Append(StaleDlaKodu.WciecieDlaZawartosciMetody);

            builder.DodajWciecieWgPoziomuMetody(poziomKlasy);

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