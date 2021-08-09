using System.IO;
using System.Text;

namespace KruchyParserKoduTests.Utils
{
    class WczytywaczZawartosciPrzykladow
    {
        public string DajZawartoscPrzykladu(
            string nazwaPrzykladu,
            string namespace1 = "KruchyParserKoduTests.Samples.")
        {
            using (
                var stream =
            GetType().Assembly.GetManifestResourceStream(namespace1 + nazwaPrzykladu))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }

        }
    }
}
