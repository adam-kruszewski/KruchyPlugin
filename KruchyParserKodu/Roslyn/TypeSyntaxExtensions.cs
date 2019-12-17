using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KruchyParserKodu.Roslyn
{
    public static class TypeSyntaxExtensions
    {
        public static string DajNazweTypu(this TypeSyntax syntax)
        {
            var identyfikatorTypu = syntax as IdentifierNameSyntax;
            var alternatywnyIdentyfikatorTypu = syntax as GenericNameSyntax;
            var predefiniowanyTyp = syntax as PredefinedTypeSyntax;
            var nullableTyp = syntax as NullableTypeSyntax;

            if (predefiniowanyTyp != null)
                return predefiniowanyTyp.ToString();

            if (identyfikatorTypu != null)
                return identyfikatorTypu.Identifier.ValueText;
            if (alternatywnyIdentyfikatorTypu != null)
                return alternatywnyIdentyfikatorTypu.ToFullString().Trim();

            if (nullableTyp != null)
                return nullableTyp.ToFullString().Trim();

            return syntax.ToFullString().Trim();
        }
    }
}