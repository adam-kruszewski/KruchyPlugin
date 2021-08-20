namespace Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml
{
    public static class JezykExtensions
    {
        public static string Konstruktor(this int jezyk)
        {
            if (jezyk == 2)
            {
                return "Constructor";
            }

            return "Konstruktor";
        }

        public static string PrzygotujCzasownik(this int jezyk, string text)
        {
            if (jezyk == 2)
            {
                return text.PrzygotujAngielskiCzasownik();
            }

            return text;
        }

        public static string PrzygotujAngielskiCzasownik(this string text)
        {
            if (text.EndsWith("s"))
            {
                return text + "es";
            }else if (text.EndsWith("y"))
            {
                return text.Substring(0, text.Length - 1) + "ies";
            }else
            {
                return text + "s";
            }
        }
    }
}