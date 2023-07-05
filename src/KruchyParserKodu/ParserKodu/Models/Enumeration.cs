using KruchyParserKodu.ParserKodu.Interfaces;
using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class Enumeration
        : ParsowanaJednostka
            ,IWithName
                , IWithBraces
                    , IWithOwner
                        , IWithComment
                            , IWithDocumentation
    {
        public Obiekt Owner { get; set; }

        public string Name { get; set; }

        public IList<Pole> Fields { get; private set; }

        public IList<Modyfikator> Modifiers { get; set; }

        public List<Attribute> Attributes { get; private set; }

        public PlaceInFile StartingBrace { get; set; }

        public PlaceInFile ClosingBrace { get; set; }

        public Comment Comment { get; set; }

        public Documentation Documentation { get; set; }

        public Enumeration()
        {
            Fields = new List<Pole>();
            StartingBrace = new PlaceInFile();
            ClosingBrace = new PlaceInFile();
            Modifiers = new List<Modyfikator>();
            Attributes = new List<Attribute>();
        }

    }
}
