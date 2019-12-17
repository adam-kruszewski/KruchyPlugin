using System;
using System.Collections.Generic;
using System.Linq;
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

        public static bool JestGeneryczny(this TypeSyntax syntax)
        {
            return (syntax as GenericNameSyntax) != null;
        }

        public static Tuple<string, List<string>> DajDaneTypuGenerycznego(
            this TypeSyntax syntax)
        {
            var generyczny = syntax as GenericNameSyntax;

            var parametry =
                generyczny
                    .TypeArgumentList
                        .Arguments
                            .Select(o => o.DajNazweTypu());

            return Tuple.Create(generyczny.Identifier.ValueText, parametry.ToList());
        }
    }
}