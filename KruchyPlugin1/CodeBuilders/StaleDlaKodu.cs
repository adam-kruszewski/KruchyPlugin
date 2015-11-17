using System.Text;

namespace KruchyCompany.KruchyPlugin1.CodeBuilders
{
    class StaleDlaKodu
    {
        public const string JednostkaWciecia = "    ";

        public static string WielokrotnoscWciecia(int ile)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < ile; i++)
                builder.Append(JednostkaWciecia);

            return builder.ToString();
        }
    }
}
