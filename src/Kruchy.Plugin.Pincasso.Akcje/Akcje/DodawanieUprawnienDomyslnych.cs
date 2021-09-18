using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using System.Windows.Forms;

namespace Kruchy.Plugin.Pincasso.Akcje.Akcje
{
    public class DodawanieUprawnienDomyslnych
    {
        private readonly ISolutionWrapper solution;

        public DodawanieUprawnienDomyslnych(ISolutionWrapper solution)
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
