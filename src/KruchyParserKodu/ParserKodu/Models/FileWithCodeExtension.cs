using System;
using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu.Models
{
    public static class FileWithCodeExtension
    {
        public static Method FindMethodByLineNumber(
            this FileWithCode parsed,
            int lineNumber)
        {
            var methods =
                parsed
                    .DefinedItems
                        .SelectMany(o => GetAllMethodsForObject(o));

            return
                methods
                    .Where(o =>
                        o.StartPosition.Row <= lineNumber
                            && o.EndPosition.Row >= lineNumber)
                            .FirstOrDefault();
        }

        private static IEnumerable<Method> GetAllMethodsForObject(DefinedItem defindedItem)
        {
            var internalObjectsMethods =
                defindedItem.InternalDefinedItems.SelectMany(o => GetAllMethodsForObject(o));

            return defindedItem.Methods.Union(internalObjectsMethods);
        }

        public static Constructor FindConstructorByLineNumber(
            this FileWithCode parsed,
            int lineNumber)
        {
            var constructors = 
                parsed
                    .DefinedItems
                        .SelectMany(o => GetAllConstructors(o));

            return constructors
                    .Where(o => ContainsLineByNumber(o, lineNumber))
                        .FirstOrDefault();
        }

        private static IEnumerable<Constructor> GetAllConstructors(DefinedItem definedItem)
        {
            var internalObjectConstructors =
                definedItem.InternalDefinedItems
                    .SelectMany(o => GetAllConstructors(o));

            return definedItem.Constructors.Union(internalObjectConstructors);
        }

        public static Property FindPropertyByLineNumber(
            this FileWithCode parsed,
            int lineNumber)
        {
            var properties =
                parsed
                    .DefinedItems
                        .SelectMany(o => GetAllProperties(o));
            return
                properties
                    .Where(o =>
                        o.StartPosition.Row <= lineNumber
                            && o.EndPosition.Row >= lineNumber)
                            .FirstOrDefault();
        }

        private static IEnumerable<Property> GetAllProperties(DefinedItem obiekt)
        {
            var internalObjectsProperties =
                obiekt.InternalDefinedItems.SelectMany(o => GetAllProperties(o));

            return obiekt.Properties.Union(internalObjectsProperties);
        }

        public static Field FindFieldByLineNumber(
            this FileWithCode parsed,
            int lineNumber)
        {
            var pola = parsed.DefinedItems.SelectMany(o => o.Fields);
            return
                pola
                    .Where(o =>
                        o.StartPosition.Row <= lineNumber
                            && o.EndPosition.Row >= lineNumber)
                            .FirstOrDefault();
        }

        public static int FindFirstLineForMethod(this FileWithCode parsowane)
        {
            if (parsowane.DefinedItems.Count != 1)
                throw new Exception("Liczba definiowanych obiektów rózna od 1");
            var definedItem = parsowane.DefinedItems.First();

            var definitionsLastLines = definedItem.Fields.Select(o => o.EndPosition)
                .Union(definedItem.Properties.Select(o => o.EndPosition))
                .Union(definedItem.Constructors.Select(o => o.EndPosition))
                .Union(definedItem.Methods.Select(o => o.EndPosition))
                    .Select(o => o.Row);
            if (definitionsLastLines.Count() == 0)
            {
                return definedItem.StartingBrace.Row + 1;
            }

            return definitionsLastLines.Max() + 1;
        }

        public static int FindFirstLineForConstructor(
            this FileWithCode parsed,
            int lineNumberInDefinedItem)
        {
            var definedItem = parsed.FindClassByLineNumber(lineNumberInDefinedItem);

            var definitionLastLines =
                definedItem.Fields.Select(o => o.EndPosition)
                    .Union(definedItem.Properties.Select(o => o.EndPosition))
                        .Select(o => o.Row);

            if (definitionLastLines.Count() == 0)
            {
                return definedItem.StartingBrace.Row + 1;
            }

            return definitionLastLines.Max() + 1;
        }

        public static DefinedItem FindClassByLineNumber(
            this FileWithCode parsed,
            int lineNumber)
        {
            return
                parsed
                    .DefinedItems
                    .SelectMany(o => GetDefinedItemsOfDefinedItem(o))
                    .Where(o => o.KindOfItem == KindOfItem.Class)
                    .Where(o => o.ContainsLineByNumber(lineNumber))
                    .OrderBy(o => GetDistance(o, lineNumber))
                            .FirstOrDefault();
        }

        private static object GetDistance(DefinedItem definedItem, int lineNumber)
        {
            return Math.Abs(definedItem.StartPosition.Row - lineNumber)
                + Math.Abs(definedItem.EndPosition.Row - lineNumber);
        }

        public static IEnumerable<DefinedItem> GetDefinedItemsOfDefinedItem(DefinedItem definedItem)
        {
            var result =
                definedItem.InternalDefinedItems
                    .SelectMany(o => GetDefinedItemsOfDefinedItem(o))
                        .ToList();

            result.Add(definedItem);

            return result;
        }

        public static DefinedItem FindDefinedItemByLineNumber(
            this FileWithCode parsed,
            int lineNumber)
        {
            return
                parsed
                    .DefinedItems
                        .Where(o => o.ContainsLineByNumber(lineNumber))
                            .FirstOrDefault();
        }

        private static bool ContainsLineByNumber(
            this ParsedUnit parsedUnit,
            int lineNumber)
        {
            if (parsedUnit.StartPosition.Row <= lineNumber
                    && parsedUnit.EndPosition.Row >= lineNumber)
                return true;
            else
                return false;
        }
    }
}