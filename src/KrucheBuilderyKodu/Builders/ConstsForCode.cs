using System.Text;

namespace KruchyCodeBuilders.Builders
{
    public class ConstsForCode
    {
        public const string IndentUnit = "    ";
        public const string DefaultIndentForMethod = IndentUnit + IndentUnit;
        public const string DefaultIndentForMethodContent = DefaultIndentForMethod + IndentUnit;
        public const string DefaultIndentForMethodParameters = DefaultIndentForMethod + IndentUnit;
        public const string DefaultIndentForClass = IndentUnit;
        public const string DefaultIndentForClassField = DefaultIndentForClass + IndentUnit;

        public static string IndentMultiplication(int ile)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < ile; i++)
                builder.Append(IndentUnit);

            return builder.ToString();
        }
    }
}
