using KruchyParserKodu.ParserKodu.Interfaces;
using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class Method
        : MethodConstructorBase
            , IWithParameterBraces
                , IWithOwner
                    , IWithComment
                        , IWithDocumentation
    {
        public IList<GenericParameter> GenericParameters { get; set; }

        public ReturnedType ReturnType { get; set; }

        public string Name { get; set; }

        public Method()
        {
            GenericParameters = new List<GenericParameter>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}