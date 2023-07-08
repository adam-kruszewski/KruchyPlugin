using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KruchyParserKodu.Roslyn
{
    public static class TypeSyntaxExtensions
    {
        public static string GetTypeName(this TypeSyntax syntax)
        {
            var typeIdentifier = syntax as IdentifierNameSyntax;
            var alternateTypeIdentifier = syntax as GenericNameSyntax;
            var predefindType = syntax as PredefinedTypeSyntax;
            var nullableType = syntax as NullableTypeSyntax;

            if (predefindType != null)
                return predefindType.ToString();

            if (typeIdentifier != null)
                return typeIdentifier.Identifier.ValueText;
            if (alternateTypeIdentifier != null)
                return alternateTypeIdentifier.ToFullString().Trim();

            if (nullableType != null)
                return nullableType.ToFullString().Trim();

            return syntax.ToFullString().Trim();
        }

        public static bool IsGeneric(this TypeSyntax syntax)
        {
            return (syntax as GenericNameSyntax) != null;
        }

        public static Tuple<string, List<string>> GetGenericTypesDetails(
            this TypeSyntax syntax)
        {
            var generic = syntax as GenericNameSyntax;

            var parametry =
                generic
                    .TypeArgumentList
                        .Arguments
                            .Select(o => o.GetTypeName());

            return Tuple.Create(generic.Identifier.ValueText, parametry.ToList());
        }
    }
}