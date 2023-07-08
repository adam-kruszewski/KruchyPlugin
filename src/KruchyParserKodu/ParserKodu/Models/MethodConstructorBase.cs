using KruchyParserKodu.ParserKodu.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class MethodConstructorBase
        : ParsowanaJednostka
            , IWithParameterBraces
                , IWithOwner
                    , IWithComment
                        , IWithDocumentation
    {
        public DefinedItem Owner { get; set; }

        public IList<Parametr> Parametry { get; private set; }

        public IList<Modifier> Modyfikatory { get; set; }

        public List<Attribute> Atrybuty { get; private set; }

        public PlaceInFile StartingParameterBrace { get; set; }

        public PlaceInFile ClosingParameterBrace { get; set; }

        public Comment Comment { get; set; }

        public Documentation Documentation { get; set; }

        public IList<Instruction> Instructions { get; private set; }

        public bool Prywatna
        {
            get
            {
                return Modyfikatory.Any(o => o.Name == "private");
            }
        }

        public bool Publiczna
        {
            get
            {
                return Modyfikatory.Any(o => o.Name == "public");
            }
        }

        public MethodConstructorBase()
        {
            Parametry = new List<Parametr>();
            Modyfikatory = new List<Modifier>();
            Atrybuty = new List<Attribute>();
            StartingParameterBrace = new PlaceInFile();
            ClosingParameterBrace = new PlaceInFile();
            Instructions = new List<Instruction>();
        }

        public bool ZawieraModyfikator(string nazwa)
        {
            return Modyfikatory.Any(o => o.Name == nazwa);
        }
    }
}
