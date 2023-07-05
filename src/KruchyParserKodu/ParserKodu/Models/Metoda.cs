using KruchyParserKodu.ParserKodu.Interfaces;
using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class Metoda
        : MethodConstructorBase
            , IWithParameterBraces
                , IWithOwner
                    , IWithComment
                        , IWithDocumentation
    {
        public IList<ParametrGeneryczny> GenericParameters { get; set; }

        public TypZwracany ReturnType { get; set; }

        public string Name { get; set; }

        public Metoda()
        {
            GenericParameters = new List<ParametrGeneryczny>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}