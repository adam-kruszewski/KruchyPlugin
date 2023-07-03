using Kruchy.Plugin.Akcje.Interfejs;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.Utils;
using Kruchy.Plugin.Utils.UI;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using System.Collections.Generic;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class DodajKonfiguracjeWlasciwosciPola
    {
        private readonly ISolutionWrapper solution;

        public DodajKonfiguracjeWlasciwosciPola(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Dodaj()
        {
            IAddFieldPropertyConfigurationWindow dialogAdd = UIObjects.FactoryInstance.Get<IAddFieldPropertyConfigurationWindow>();

            var aktualnyNumerLinii = solution.AktualnyDokument.GetCursorLineNumber();

            var parsowane = Parser.Parsuj(solution.AktualnyDokument.GetContent());

            var aktualnaWlasciwosc = parsowane.SzukajPropertiesaWLinii(aktualnyNumerLinii);

            if (aktualnaWlasciwosc != null)
            {
                dialogAdd.ClassNameRegex = aktualnaWlasciwosc.Owner.Name;
                dialogAdd.FieldPropertyTypeRegex = aktualnaWlasciwosc.NazwaTypu;
                dialogAdd.Value = aktualnaWlasciwosc.Nazwa;
            }

            var aktualnePole = parsowane.SzukajPolaWLinii(aktualnyNumerLinii);

            if (aktualnePole != null)
            {
                dialogAdd.ClassNameRegex = aktualnePole.Owner.Name;
                dialogAdd.FieldPropertyTypeRegex = aktualnePole.NazwaTypu;
                dialogAdd.Value = aktualnePole.Nazwa;
            }

            UIObjects.ShowWindowModal(dialogAdd);

            if (dialogAdd.Confirmed)
            {
                Konfiguracja.Modify(solution, conf =>
                {
                    if (conf.Dokumentacja == null)
                        conf.Dokumentacja = new KonfiguracjaPlugina.Xml.Dokumentacja();

                    if (conf.Dokumentacja.WlasciwosciPola == null)
                        conf.Dokumentacja.WlasciwosciPola = new List<KonfiguracjaPlugina.Xml.WlasciwoscPole>();

                    conf.Dokumentacja.WlasciwosciPola.Add(
                        new KonfiguracjaPlugina.Xml.WlasciwoscPole
                        {
                            RegexNazwyKlasy = dialogAdd.ClassNameRegex.NullIfEmptyStringElseValue(),
                            RegexTypWlasciwosciPola = dialogAdd.FieldPropertyTypeRegex.NullIfEmptyStringElseValue(),
                            Wartosc = dialogAdd.Value.NullIfEmptyStringElseValue(),
                            WyjsciowaWartosc = dialogAdd.OutputValue.NullIfEmptyStringElseValue()
                        });
                });
            }
        }
    }
}
