using System.Windows;
using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Extensions;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class DodawanieUprawnienDomyslnych
    {
        private readonly SolutionWrapper solution;

        public DodawanieUprawnienDomyslnych(SolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Dodaj()
        {
            if (!solution.CzyPlikControllera())
            {
                MessageBox.Show("To nie jest plik controllera");
                return;
            }
            var nazwaKlasy = "";
            var dokument = solution.AktualnyDokument;
            var liczbaLinii = dokument.DajLiczbeLinii();
            for (int i = 1; i <= dokument.DajLiczbeLinii(); i++)
            {
                var linia = dokument.DajZawartoscLinii(i);
                if (linia.Contains("class ") && linia.Contains(nazwaKlasy))
                {
                    var trescWstawiana =
                        new AtrybutBuilder()
                            .ZNazwa("UprawnieniaDomyslne")
                                .Build(StaleDlaKodu.WciecieDlaKlasy);
                    dokument.WstawWLinii(trescWstawiana, i);
                    dokument.DodajUsingaJesliTrzeba("Pincasso.MvcApp.Security");
                    break;
                }
            }
        }
    }
}