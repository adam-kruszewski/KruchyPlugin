namespace KruchyParserKodu.ParserKodu.Models
{
    public class UsingNamespace : ParsedUnit
    {
        public string Name { get; private set; }

        public UsingNamespace(string name)
        {
            Name = name;
            StartPosition = new PlaceInFile();
            EndPosition = new PlaceInFile();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}