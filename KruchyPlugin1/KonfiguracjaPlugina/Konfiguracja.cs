using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.KonfiguracjaPlugina
{
    class Konfiguracja
    {
        #region[STATIC]
        private static Konfiguracja instance;
        public static Konfiguracja GetInstance()
        {
            if (instance == null)
                instance = new Konfiguracja();
            return instance;
        }
        #endregion

        private KonfiguracjaUsingow Usingi { get; set; }

        private Konfiguracja()
        {
        }

        public KonfiguracjaUsingow DajKonfiguracjeUsingow(SolutionWrapper solution)
        {
            if (Usingi == null || Usingi.NazwaSolution != solution.PelnaNazwa)
                Usingi = new KonfiguracjaUsingow(solution);
            return Usingi;
        }
    }
}