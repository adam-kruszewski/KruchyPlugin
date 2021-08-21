namespace Kruchy.Plugin.Akcje.Tests.Samples
{
    /// <summary>
    /// Klasa do dokumentacji
    /// </summary>
    class KlasaDoDokumentacji
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="parametr">Parametr</param>
        public KlasaDoDokumentacji(string parametr)
        {

        }

        /// <summary>
        /// Wykonaj metode1
        /// </summary>
        public void WykonajMetode1()
        {

        }

        /// <summary>
        /// Wykonaj metode z wynikiem
        /// </summary>
        /// <param name="stringowyParametr">Stringowy parametr</param>
        /// <returns></returns>
        public int WykonajMetodeZWynikiem(string stringowyParametr)
        {
            return 1;
        }

        /// <inheritdoc/>
        public override void MetodaDziedziczona()
        {

        }

        /// <summary>
        /// Pole testowe
        /// </summary>
        private string poleTestowe;

        /// <summary>
        /// Wlasciwosc testowa
        /// </summary>
        public int WlasciwoscTestowa { get; set; }
    }
}
