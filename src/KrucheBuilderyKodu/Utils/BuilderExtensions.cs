using System.Text;
using KrucheBuilderyKodu.Builders;

namespace KrucheBuilderyKodu.Utils
{
    public static class BuilderExtensions
    {
        public static void DodajWciecieWgPoziomuMetody(
            this StringBuilder builder,
            int poziomMetody)
        {
            if (poziomMetody > 1)
                for (int i = 0; i < poziomMetody - 1; i++)
                    builder.Append(StaleDlaKodu.JednostkaWciecia);
        }
    }
}