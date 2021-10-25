using Kruchy.Plugin.Akcje.Interfejs;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;
using Kruchy.Plugin.Utils.UI;
using Kruchy.Plugin.Utils.Wrappers;
using System.Collections.Generic;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class DodajKonfiguracjeCzasownika
    {
        private readonly ISolutionWrapper solution;

        public DodajKonfiguracjeCzasownika(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Dodaj()
        {
            IAddVerbConfigurationWindow dialogAdd = UIObjects.FactoryInstance.Get<IAddVerbConfigurationWindow>();

            UIObjects.ShowWindowModal(dialogAdd);

            if (dialogAdd.Confirmed)
            {
                Konfiguracja.Modify(solution, conf =>
                {
                    if (conf.Dokumentacja == null)
                        conf.Dokumentacja = new KonfiguracjaPlugina.Xml.Dokumentacja();

                    if (conf.Dokumentacja.Czasowniki == null)
                        conf.Dokumentacja.Czasowniki = new List<KonfiguracjaPlugina.Xml.Czasownik>();

                    conf.Dokumentacja.Czasowniki.Add(
                        new Czasownik
                        {
                            Wartosc = dialogAdd.Value,
                            RegexNazwyKlasy = dialogAdd.ClassNameRegex,
                            WyjsciowaWartosc = dialogAdd.OutputValue
                        });
                });
            }
        }
    }
}