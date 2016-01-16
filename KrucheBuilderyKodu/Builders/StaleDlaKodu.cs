using System.Text;

namespace KrucheBuilderyKodu.Builders
{
    public class StaleDlaKodu
    {
        public const string JednostkaWciecia = "    ";
        public const string WciecieDlaMetody = JednostkaWciecia + JednostkaWciecia;
        public const string WciecieDlaKlasy = JednostkaWciecia;

        public static string WielokrotnoscWciecia(int ile)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < ile; i++)
                builder.Append(JednostkaWciecia);

            return builder.ToString();
        }
    }
}
