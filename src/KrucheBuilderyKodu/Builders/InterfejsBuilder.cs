using System.Text;

namespace KruchyCodeBuilders.Builders
{
    public class InterfejsBuilder : ICodeBuilder
    {
        private string modyfikator { get; set; }
        private string nazwa { get; set; }
        private string nazwaNadklasy { get; set; }

        public InterfejsBuilder ZNazwa(string nazwa)
        {
            this.nazwa = nazwa;
            return this;
        }

        public InterfejsBuilder ZModyfikatorem(string modyfikator)
        {
            this.modyfikator = modyfikator;
            return this;
        }

        public InterfejsBuilder ZNadklasa(string nazwa)
        {
            nazwaNadklasy = nazwa;
            return this;
        }

        public string Build(string wciecie = "")
        {
            var outputBuilder = new StringBuilder();

            outputBuilder.Append(wciecie);
            if (!string.IsNullOrEmpty(modyfikator))
                outputBuilder.Append(modyfikator + " ");
            outputBuilder.Append("interface ");
            outputBuilder.Append(nazwa);
            if (!string.IsNullOrEmpty(nazwaNadklasy))
                outputBuilder.Append(" : " + nazwaNadklasy);
            outputBuilder.AppendLine();
            outputBuilder.AppendLine(wciecie + "{");

            outputBuilder.AppendLine(wciecie + "}");
            return outputBuilder.ToString();

        }
    }
}
