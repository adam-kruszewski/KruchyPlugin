using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class FileWithCode
    {
        public string Namespace { get; set; }
        public PlaceInFile StartNamespace { get; set; }
        public PlaceInFile EndNamespace { get; set; }
        public IList<UsingNamespace> Usings { get; private set; }

        public IList<DefinedItem> DefinedItems { get; private set; }

        public IList<Enumeration> DefinedEnumerations { get; private set; }

        public FileWithCode()
        {
            DefinedItems = new List<DefinedItem>();
            Usings = new List<UsingNamespace>();
            DefinedEnumerations = new List<Enumeration>();
        }
    }
}