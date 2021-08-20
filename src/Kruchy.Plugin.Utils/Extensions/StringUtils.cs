using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kruchy.Plugin.Utils.Extensions
{
    public static class StringUtils
    {
        public static string Spacji(this int ile)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < ile; i++)
                builder.Append(" ");

            return builder.ToString();
        }

        public static IEnumerable<string> PodzielNaSlowaOdWielkichLiter(this string text)
        {
            var wielkieLitery = new List<int>();

            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) || i == 0)
                    wielkieLitery.Add(i);
            }

            for (int i = 0; i < wielkieLitery.Count - 1; i++)
            {
                yield return text.Substring(wielkieLitery[i], wielkieLitery[i + 1] - wielkieLitery[i]);
            }

            yield return text.Substring(wielkieLitery.Last());
        }

        public static string ZacznijDuzaLitera(this string text)
        {
            return char.ToUpper(text[0]) + text.Substring(1);
        }
    }
}