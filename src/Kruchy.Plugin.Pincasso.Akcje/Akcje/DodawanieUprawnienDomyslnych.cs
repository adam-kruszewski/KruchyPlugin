using KruchyCodeBuilders.Builders;
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
            var dokument = solution.CurentDocument;
            var liczbaLinii = dokument.GetLineCount();
            for (int i = 1; i <= dokument.GetLineCount(); i++)
            {
                var linia = dokument.GetLineContent(i);
                if (linia.Contains("class ") && linia.Contains(nazwaKlasy))
                {
                    var trescWstawiana =
                        new AttributeBuilder()
                            .WithName("UprawnieniaDomyslne")
                                .Build(ConstsForCode.DefaultIndentForClass);
                    dokument.InsertInLine(trescWstawiana, i);
                    dokument.DodajUsingaJesliTrzeba("Pincasso.MvcApp.Security");
                    break;
                }
            }
        }
    }
}
