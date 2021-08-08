using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using System.Collections.Generic;
using System.Linq;

namespace Kruchy.Plugin.Akcje.Akcje
{
    public class UzupelnianieDokumentacji
    {
        private readonly ISolutionWrapper solution;

        public UzupelnianieDokumentacji(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Uzupelnij()
        {
            var sparsowane = solution.ParsujZawartoscAktualnegoDokumetu();

            var listaParsowanychJednostek = new List<ParsowanaJednostka>();

            listaParsowanychJednostek.AddRange(sparsowane.DefiniowaneObiekty);

            listaParsowanychJednostek.AddRange(sparsowane.DefiniowaneObiekty.SelectMany(o => o.Konstruktory));

            listaParsowanychJednostek.AddRange(sparsowane.DefiniowaneObiekty.SelectMany(o => o.Pola));

            listaParsowanychJednostek.AddRange(sparsowane.DefiniowaneObiekty.SelectMany(o => o.Propertiesy));

            listaParsowanychJednostek.AddRange(sparsowane.DefiniowaneObiekty.SelectMany(o => o.Metody));

            listaParsowanychJednostek = listaParsowanychJednostek.OrderByDescending(o => o.Poczatek.Wiersz).ToList();
        }
    }
}
