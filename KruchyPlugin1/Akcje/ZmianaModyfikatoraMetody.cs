using System.Linq;
using KruchyCompany.KruchyPlugin1.ParserKodu;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class ZmianaModyfikatoraMetody
    {
        private readonly DokumentWrapper dokument;
        private readonly string[] modyfikatory = { "public", "private", "internal", "protected" };

        public ZmianaModyfikatoraMetody(DokumentWrapper dokument)
        {
            this.dokument = dokument;
        }

        public void ZmienNa(string modyfikator)
        {
            var parsowane = Parser.Parsuj(dokument.DajZawartosc());
            var metoda =
                parsowane.SzukajMetodyWLinii(dokument.DajNumerLiniiKursora());

            if (metoda != null)
            {
                if (metoda.ZawieraModyfikator(modyfikator))
                    return;

                var dotychczasowyModyfikator =
                    SzukajDotychczasowegoModyfikatora(metoda);

                if (dotychczasowyModyfikator == null)
                    WstawModyfikator(modyfikator, metoda);
                else
                    ZmienModyfikator(modyfikator, dotychczasowyModyfikator);
            }
        }

        private void ZmienModyfikator(
            string modyfikator,
            Modyfikator dotychczasowyModyfikator)
        {
            dokument.Usun(
                dotychczasowyModyfikator.Poczatek.Wiersz,
                dotychczasowyModyfikator.Poczatek.Kolumna,
                dotychczasowyModyfikator.Koniec.Wiersz,
                dotychczasowyModyfikator.Koniec.Kolumna);
            dokument.WstawWMiejscu(
                modyfikator,
                dotychczasowyModyfikator.Poczatek.Wiersz,
                dotychczasowyModyfikator.Poczatek.Kolumna);
        }

        private void WstawModyfikator(string modyfikator, Metoda metoda)
        {
            dokument.WstawWMiejscu(
                modyfikator + " ",
                metoda.Poczatek.Wiersz,
                metoda.Poczatek.Kolumna);
        }

        private Modyfikator SzukajDotychczasowegoModyfikatora(Metoda metoda)
        {
            var dotychczasowyModyfikator =
                metoda.Modyfikatory
                    .Where(o => modyfikatory.Any(m => m == o.Nazwa))
                        .FirstOrDefault();
            return dotychczasowyModyfikator;
        }
    }
}