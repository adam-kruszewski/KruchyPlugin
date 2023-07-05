using KruchyParserKodu.ParserKodu.Interfaces;
using KruchyParserKodu.ParserKodu.Models;
using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu
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

        public IList<Pole> Pola { get; private set; }

        public IList<Modyfikator> Modyfikatory { get; set; }

        public List<Atrybut> Atrybuty { get; private set; }

        public PlaceInFile StartingBrace { get; set; }

        public PlaceInFile ClosingBrace { get; set; }

        public Comment Komentarz { get; set; }

        public Documentation Dokumentacja { get; set; }

        public Enumeration()
        {
            Pola = new List<Pole>();
            StartingBrace = new PlaceInFile();
            ClosingBrace = new PlaceInFile();
            Modyfikatory = new List<Modyfikator>();
            Atrybuty = new List<Atrybut>();
        }

    }
}
