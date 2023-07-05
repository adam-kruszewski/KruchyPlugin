using KruchyParserKodu.ParserKodu.Interfaces;
using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class Constructor
        : MethodConstructorBase
                , IWithBraces
    {
        public string Modifier { get; set; }

        public PlaceInFile StartingBrace { get; set; }
        public PlaceInFile ClosingBrace { get; set; }

        //jeśli nie ma wołania kontruktora z nadklasy, to pole jest nullem
        public IList<string> ParentClassContructorParameters;
        public string InitializationKeyWord { get; set; }

        public Constructor() : base()
        {
            StartingBrace = new PlaceInFile();
            ClosingBrace = new PlaceInFile();
        }
    }
}