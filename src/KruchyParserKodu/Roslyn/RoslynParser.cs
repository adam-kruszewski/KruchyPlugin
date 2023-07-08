using System;
using System.Collections.Generic;
using System.Linq;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Interfaces;
using KruchyParserKodu.ParserKodu.Models;
using KruchyParserKodu.ParserKodu.Models.Instructions;
using KruchyParserKodu.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace KruchyParserKodu.Roslyn
{
    class RoslynParser : IParser
    {
        public FileWithCode Parse(string zawartosc)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(zawartosc);
            var root = syntaxTree.GetRoot() as CompilationUnitSyntax;

            DisplayHierachy(root);

            var result = new FileWithCode();

            BaseNamespaceDeclarationSyntax namespaceDeclaration =
                root.Members.OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

            var fileScopedNamespaceDeclaration =
                root.Members.OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault();

            if (fileScopedNamespaceDeclaration != null)
                namespaceDeclaration = fileScopedNamespaceDeclaration;

            if (namespaceDeclaration != null)
                result.Namespace = namespaceDeclaration.Name.ToString();


            foreach (var u in root.Usings)
                result.Usings.Add(GetUsing(u));

            var classes =
                namespaceDeclaration.Members.OfType<ClassDeclarationSyntax>();

            foreach (var definedClass in classes)
            {
                var definedItem = ParseClass(syntaxTree, definedClass);

                result.DefinedItems.Add(definedItem);
            }

            var interfaces = namespaceDeclaration.Members.OfType<InterfaceDeclarationSyntax>();
            foreach (var interfaceSyntax in interfaces)
            {
                result.DefinedItems.Add(ParseInterface(interfaceSyntax));
            }

            var enums = namespaceDeclaration.Members.OfType<EnumDeclarationSyntax>();
            foreach (var enumSyntax in enums)
            {
                result.DefinedEnumerations.Add(ParseEnum(enumSyntax));
            }

            return result;
        }

        private Enumeration ParseEnum(EnumDeclarationSyntax enumSyntax)
        {
            var definedEnum = new Enumeration();
            definedEnum.Name = enumSyntax.Identifier.ValueText;
            SetPosition(enumSyntax.SyntaxTree, definedEnum, enumSyntax);

            FillField(definedEnum.Fields, enumSyntax, null);

            SetOpeningBracePosition(definedEnum, enumSyntax.OpenBraceToken);
            SetClosingBracePosition(definedEnum, enumSyntax.CloseBraceToken);

            FillAttributes(enumSyntax.AttributeLists, definedEnum.Attributes);
            FillModifiers(enumSyntax.Modifiers, definedEnum.Modifiers);

            ParseDocumentation(definedEnum, enumSyntax);
            ParseComment(definedEnum, enumSyntax);

            return definedEnum;
        }

        private void DisplayHierachy(CompilationUnitSyntax root)
        {
            var indent = "";

            Console.WriteLine($"{indent} - {root.GetType().Name}");

            var trivias = root.DescendantTrivia();


            var nodes = root.ChildNodes();

            foreach (var node in nodes)
            {
                DisplayNode(node, indent + "  ");
            }

            for (int i = 0; i < nodes.Count(); i++)
            {

            }

            var tokens = root.ChildTokens();
        }

        private void DisplayNode(SyntaxNode node, string indent)
        {
            Console.WriteLine($"{indent} - {node.GetType().Name} :: {node.Kind()} ++ {node.ToFullString()}");
            var nodes = node.ChildNodes();

            if (node is ClassDeclarationSyntax)
            {
                var classDeclarion = node as ClassDeclarationSyntax;

                var trivias = classDeclarion.GetLeadingTrivia();

                foreach (var trivia in trivias)
                {
                    if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                    {
                        Console.WriteLine($"Comment :: {trivia.ToString()}");
                    }
                }
            }

            foreach (var node2 in nodes)
            {
                DisplayNode(node2, indent + "  ");
            }
        }

        private void ParseComment(IWithComment definedItem, SyntaxNode syntaxNode)
        {
            var trivias = syntaxNode.GetLeadingTrivia();

            foreach (var trivia in trivias.Where(o => o.IsKind(SyntaxKind.SingleLineCommentTrivia)))
            {
                if (definedItem.Comment == null)
                    definedItem.Comment = new Comment();
                definedItem.Comment.AddLine(trivia.ToString());
                SetPosition(trivia, definedItem.Comment);
            }
        }

        private void ParseDocumentation(IWithDocumentation definedItem, SyntaxNode syntaxNode)
        {
            var trivias = syntaxNode.GetLeadingTrivia();

            foreach (var trivia in trivias.Where(o => o.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)))
            {
                if (definedItem.Documentation == null)
                    definedItem.Documentation = new Documentation();
                definedItem.Documentation.AddDocumentation(trivia.ToFullString());
                SetPosition(trivia, definedItem.Documentation);
            }
        }

        private UsingNamespace GetUsing(UsingDirectiveSyntax u)
        {
            var result = new UsingNamespace(u.Name.ToString());
            var position = GetPosition(u.SyntaxTree, u.Span);
            result.StartPosition = position.Item1.ToPlaceInFile();
            result.EndPosition = position.Item2.ToPlaceInFile();
            return result;
        }

        private DefinedItem ParseInterface(InterfaceDeclarationSyntax interfaceSyntax)
        {
            var definedItem = new DefinedItem();
            definedItem.Name = interfaceSyntax.Identifier.ValueText;
            definedItem.KindOfItem = KindOfItem.Interface;
            SetPosition(interfaceSyntax.Keyword, definedItem.KindOfObjectUnit);

            SetPosition(interfaceSyntax.SyntaxTree, definedItem, interfaceSyntax);
            SetOpeningBracePosition(definedItem, interfaceSyntax.OpenBraceToken);
            SetClosingBracePosition(definedItem, interfaceSyntax.CloseBraceToken);

            FillAttributes(interfaceSyntax.AttributeLists, definedItem.Attributes);
            FillModifiers(interfaceSyntax.Modifiers, definedItem.Modifiers);
            FillProperties(definedItem.Properties, interfaceSyntax, definedItem);
            FillMethods(definedItem.Methods, interfaceSyntax, definedItem);
            FillDerivedTypes(definedItem, interfaceSyntax);

            ParseDocumentation(definedItem, interfaceSyntax);
            ParseComment(definedItem, interfaceSyntax);

            ParseGenericParameters(definedItem, interfaceSyntax.TypeParameterList);

            return definedItem;
        }

        private DefinedItem ParseClass(SyntaxTree syntaxTree, ClassDeclarationSyntax classSyntax)
        {
            var definedItem = new DefinedItem();
            definedItem.Name = classSyntax.Identifier.ValueText;
            definedItem.KindOfItem = KindOfItem.Class;
            SetPosition(classSyntax.Keyword, definedItem.KindOfObjectUnit);

            SetPosition(syntaxTree, definedItem, classSyntax);
            SetOpeningBracePosition(definedItem, classSyntax.OpenBraceToken);
            SetClosingBracePosition(definedItem, classSyntax.CloseBraceToken);

            FillAttributes(classSyntax.AttributeLists, definedItem.Attributes);

            FillModifiers(classSyntax.Modifiers, definedItem.Modifiers);

            FillFields(definedItem.Fields, classSyntax, definedItem);

            FillProperties(definedItem.Properties, classSyntax, definedItem);

            FillConstructors(definedItem.Constructors, classSyntax, definedItem);

            FillMethods(definedItem.Methods, classSyntax, definedItem);

            FillDerivedTypes(definedItem, classSyntax);

            ParseComment(definedItem, classSyntax);

            ParseDocumentation(definedItem, classSyntax);

            ParseGenericParameters(definedItem, classSyntax.TypeParameterList);

            var internalClasses = FindInternalClasses(classSyntax);

            foreach (var internalClass in internalClasses)
            {
                internalClass.Owner = definedItem;
                definedItem.InternalDefinedItems.Add(internalClass);
            }

            return definedItem;
        }

        private void ParseGenericParameters(DefinedItem definedItem, TypeParameterListSyntax parameterListSyntax)
        {
            ParseGenericParameters(definedItem.GenericParameters, parameterListSyntax);
        }

        private void ParseGenericParameters(
            IList<GenericParameter> genericParameters,
            TypeParameterListSyntax parameterListSyntax)
        {
            if (parameterListSyntax == null)
                return;

            foreach (var typeParameter in parameterListSyntax.Parameters)
            {
                var genericParameter = new GenericParameter
                {
                    Name = typeParameter.Identifier.ValueText
                };

                SetPosition(typeParameter.SyntaxTree, genericParameter, typeParameter);

                genericParameters.Add(genericParameter);
            }
        }

        private void FillDerivedTypes(DefinedItem definedItem, TypeDeclarationSyntax syntax)
        {
            if (syntax.BaseList != null)
                definedItem.SuperClassAndInterfaces.AddRange(
                    syntax.BaseList.Types.Select(o => GetDerivedType(o)));

        }

        private IEnumerable<DefinedItem> FindInternalClasses(ClassDeclarationSyntax externalClassSyntax)
        {
            foreach (var classSyntax in externalClassSyntax.Members.OfType<ClassDeclarationSyntax>())
            {
                var parsedClass = ParseClass(externalClassSyntax.SyntaxTree, classSyntax);
                yield return parsedClass;
            }
        }

        private DerivedObject GetDerivedType(BaseTypeSyntax baseTypeSyntax)
        {
            var result = new DerivedObject();

            result.Name = baseTypeSyntax.Type.GetTypeName();
            result.StartPosition = GetPosition(baseTypeSyntax.SyntaxTree, baseTypeSyntax.Span).Item1.ToPlaceInFile();
            result.EndPosition = GetPosition(baseTypeSyntax.SyntaxTree, baseTypeSyntax.Span).Item2.ToPlaceInFile();

            if (baseTypeSyntax.Type.IsGeneric())
            {
                var genericDetails = baseTypeSyntax.Type.GetGenericTypesDetails();
                result.Name = genericDetails.Item1;
                result.ParameterTypeNames.AddRange(genericDetails.Item2);
            }

            return result;
        }

        private void FillMethods(
            IList<Method> methods,
            TypeDeclarationSyntax typeDeclarationSyntax,
            DefinedItem parent)
        {
            foreach (var methodSyntax in typeDeclarationSyntax.Members.OfType<MethodDeclarationSyntax>())
            {
                var method = new Method();
                method.Name = methodSyntax.Identifier.ValueText;
                FillModifiers(methodSyntax.Modifiers, method.Modyfikatory);
                FillParameters(methodSyntax.ParameterList, method.Parametry);

                SetPosition(typeDeclarationSyntax.SyntaxTree, method, methodSyntax);

                method.ReturnType = new ReturnedType
                {
                    Name = methodSyntax.ReturnType.GetTypeName()
                };

                SetPosition(typeDeclarationSyntax.SyntaxTree, method.ReturnType, methodSyntax.ReturnType);

                FillBracesPositions(methodSyntax, method);

                FillAttributes(methodSyntax.AttributeLists, method.Atrybuty);

                method.Instructions.AddRange(ParseInstructions(methodSyntax, method));

                ParseDocumentation(method, methodSyntax);

                ParseComment(method, methodSyntax);

                ParseGenericParameters(method.GenericParameters, methodSyntax.TypeParameterList);

                method.Owner = parent;

                methods.Add(method);
            }
        }

        private void FillConstructors(
            IList<Constructor> constructors,
            ClassDeclarationSyntax classDeclarationSyntax,
            DefinedItem parent)
        {
            foreach (var constructorSyntax in classDeclarationSyntax.Members.OfType<ConstructorDeclarationSyntax>())
            {
                Constructor constructor = ParseConstructor(parent, constructorSyntax);

                constructors.Add(constructor);
            }
        }

        private Constructor ParseConstructor(
            DefinedItem parent,
            ConstructorDeclarationSyntax constructorDeclarationSyntax)
        {
            var constructor = new Constructor();

            SetPosition(constructorDeclarationSyntax.SyntaxTree, constructor, constructorDeclarationSyntax);
            constructor.Modifier =
                FindModifiers(constructorDeclarationSyntax.Modifiers)
                    .Select(o => o.Name)
                        .SingleOrDefault();

            FillParameters(constructorDeclarationSyntax.ParameterList, constructor.Parametry);

            FillBracesPositions(
                constructorDeclarationSyntax,
                (IWithParameterBraces)constructor);

            FillMethodBracesPositions(constructorDeclarationSyntax, (IWithBraces)constructor);

            ParseDocumentation(constructor, constructorDeclarationSyntax);

            constructor.Owner = parent;

            if (constructorDeclarationSyntax.Initializer != null
                && constructorDeclarationSyntax.Initializer.ArgumentList != null)
            {
                constructor.InitializationKeyWord =
                    constructorDeclarationSyntax.Initializer.ThisOrBaseKeyword.ToString();
                constructor.ParentClassContructorParameters =
                    constructorDeclarationSyntax
                        .Initializer
                            .ArgumentList
                                .Arguments
                                    .Select(o => o.ToString().Trim())
                                        .ToList();
            }

            constructor.Instructions.AddRange(ParseInstructions(constructorDeclarationSyntax, constructor));

            return constructor;
        }

        private IEnumerable<Instruction> ParseInstructions(
            BaseMethodDeclarationSyntax constructorSyntax,
            MethodConstructorBase parentCodeUnit)
        {

            if (constructorSyntax.Body != null)
            {
                foreach (StatementSyntax instructionSyntax in constructorSyntax.Body.Statements)
                {
                    Instruction instruction = GetInstruction(instructionSyntax);

                    instruction.Text = instructionSyntax.ToString();

                    SetPosition(instruction, instructionSyntax);

                    instruction.CodeUnit = parentCodeUnit;

                    yield return instruction;
                }
            }
        }

        private static Instruction GetInstruction(StatementSyntax instructionSyntax)
        {
            if (instructionSyntax is ExpressionStatementSyntax)
            {
                AssignmentExpressionSyntax assignmentExpressionSyntax =
                    (instructionSyntax as ExpressionStatementSyntax)
                        .Expression as AssignmentExpressionSyntax;

                if (assignmentExpressionSyntax != null)
                {
                    var expressionInstruction = new AssignmentInstruction();

                    expressionInstruction.LeftSide = assignmentExpressionSyntax.Left.ToString();

                    expressionInstruction.AssignedValue = assignmentExpressionSyntax.Right.ToString();

                    return expressionInstruction;
                }
            }

            return new Instruction();
        }

        private void FillBracesPositions(
            BaseMethodDeclarationSyntax syntax,
            IWithParameterBraces definedItem)
        {
            definedItem.StartingParameterBrace =
                GetPosistion(syntax.ParameterList.OpenParenToken).Item1.ToPlaceInFile();
            definedItem.ClosingParameterBrace =
                GetPosistion(syntax.ParameterList.CloseParenToken).Item1.ToPlaceInFile();
        }

        private void FillMethodBracesPositions(
            BaseMethodDeclarationSyntax syntax,
            IWithBraces definedItem)
        {
            definedItem.StartingBrace =
                GetPosistion(syntax.Body.OpenBraceToken)
                    .Item1.ToPlaceInFile();

            definedItem.ClosingBrace =
                GetPosistion(syntax.Body.CloseBraceToken)
                    .Item1.ToPlaceInFile();

        }

        private void FillParameters(ParameterListSyntax parameterList, IList<Parameter> parameters)
        {
            parameters.AddRange(parameterList.Parameters.Select(o => GetParameter(o)));
        }

        private Parameter GetParameter(ParameterSyntax parametrSyntax)
        {
            var parameter = new Parameter();
            parameter.ParameterName = parametrSyntax.Identifier.ValueText;
            parameter.TypeName = parametrSyntax.Type.GetTypeName();

            parameter.Modifier =
                FindModifiers(parametrSyntax.Modifiers)
                    .Select(o => o.Name)
                        .SingleOrDefault();

            if (parameter.Modifier == "this")
                parameter.WithThis = true;
            if (parameter.Modifier == "out")
                parameter.WithOut = true;
            if (parameter.Modifier == "params")
                parameter.WithParams = true;
            if (parameter.Modifier == "ref")
                parameter.WithRef = true;
            if (parametrSyntax.Default != null)
                parameter.DefaultValue =
                    parametrSyntax.Default.Value.ToFullString().Trim();

            FillAttributes(parametrSyntax.AttributeLists, parameter.Attributes);

            return parameter;
        }

        private void FillProperties(
            IList<Property> properties,
            TypeDeclarationSyntax typeDeclarationSyntax,
            DefinedItem definedItem)
        {
            var propertyDeclarationSyntax = typeDeclarationSyntax.Members.OfType<PropertyDeclarationSyntax>();

            foreach (var wlasciwoscSyntax in propertyDeclarationSyntax)
            {
                var property = new Property();
                property.Name = wlasciwoscSyntax.Identifier.ValueText;
                property.TypeName = wlasciwoscSyntax.Type.GetTypeName();
                FillAttributes(wlasciwoscSyntax.AttributeLists, property.Attributes);
                FillModifiers(wlasciwoscSyntax.Modifiers, property.Modifiers);
                SetPosition(wlasciwoscSyntax.SyntaxTree, property, wlasciwoscSyntax);
                ParseDocumentation(property, wlasciwoscSyntax);

                property.HasGet = IsAccessor(wlasciwoscSyntax, "get");
                property.HasSet = IsAccessor(wlasciwoscSyntax, "set");

                property.Owner = definedItem;

                properties.Add(property);
            }
        }

        private bool IsAccessor(PropertyDeclarationSyntax syntax, string name)
        {
            if (syntax.AccessorList == null)
                return false;

            return syntax.AccessorList.Accessors.Any(o => o.Keyword.ValueText == name);
        }

        private void FillFields(
            IList<Field> fields,
            ClassDeclarationSyntax classDeclarationSyntax,
            DefinedItem parent)
        {
            var fieldDeclarations = classDeclarationSyntax.Members.OfType<FieldDeclarationSyntax>();

            foreach (var declaredField in fieldDeclarations)
            {
                var field = new Field();
                var identifier = declaredField
                        .Declaration
                            .Variables
                                .SingleOrDefault();

                if (identifier == null)
                    continue;

                field.Name = identifier.Identifier.ValueText;

                field.TypeName = declaredField.Declaration.Type.GetTypeName();

                field.Modifiers.AddRange(
                    declaredField.Modifiers.Select(o => getModifier(o)));

                SetPosition(declaredField.SyntaxTree, field, declaredField);

                ParseDocumentation(field, declaredField);

                field.Owner = parent;

                fields.Add(field);
            }
        }


        private void FillField(
            IList<Field> fields,
            EnumDeclarationSyntax enumDeclarationSyntax,
            DefinedItem parent)
        {
            var fieldDeclarations = enumDeclarationSyntax.Members.OfType<EnumMemberDeclarationSyntax>();

            foreach (var declaredField in fieldDeclarations)
            {
                var field = new Field();
                field.Name = declaredField.Identifier.ValueText;

                SetPosition(declaredField.SyntaxTree, field, declaredField);

                ParseDocumentation(field, declaredField);

                field.Owner = parent;

                fields.Add(field);
            }
        }


        private void FillModifiers(
            SyntaxTokenList syntax,
            IList<Modifier> modifiers)
        {
            foreach (var modifier in FindModifiers(syntax))
                modifiers.Add(modifier);
        }

        private IEnumerable<Modifier> FindModifiers(SyntaxTokenList syntax)
        {
            return syntax.Select(o => getModifier(o));
        }

        private Modifier getModifier(SyntaxToken syntaxToken)
        {
            var modyfikator = new Modifier(syntaxToken.ValueText);

            SetPosition(syntaxToken, modyfikator);

            return modyfikator;
        }

        private void SetOpeningBracePosition(
            IWithBraces definedItem,
            SyntaxToken token)
        {
            var startBracePosition = GetPosition(token.SyntaxTree, token);
            definedItem.StartingBrace.Row =
                startBracePosition.Item1.Line;
            definedItem.StartingBrace.Column =
                startBracePosition.Item1.Character;
        }

        private void SetClosingBracePosition(
            IWithBraces definedItem,
            SyntaxToken token)
        {
            var closingBracePosition = GetPosition(token.SyntaxTree, token);
            definedItem.ClosingBrace = closingBracePosition.Item1.ToPlaceInFile();
        }

        private void FillAttributes(
            SyntaxList<AttributeListSyntax> attributeListsSyntax,
            List<ParserKodu.Models.Attribute> attributeList)
        {
            foreach (var attribute in attributeListsSyntax)
            {
                attributeList.AddRange(ParseAttribute(attribute));
            }
        }

        private IEnumerable<ParserKodu.Models.Attribute> ParseAttribute(
            AttributeListSyntax attributeList)
        {
            foreach (var attributeSyntax in attributeList.Attributes)
            {
                yield return ParseAttribute(attributeSyntax);
            }
        }

        private ParserKodu.Models.Attribute ParseAttribute(AttributeSyntax atrybutSyntax)
        {
            var attribute =
                new ParserKodu.Models.Attribute
                {
                    Name = atrybutSyntax.Name.GetText().ToString(),
                    Parameters = FindAttributeParameters(atrybutSyntax)
                };
            SetPosition(atrybutSyntax.SyntaxTree, attribute, atrybutSyntax);
            return attribute;
        }

        private IList<AttributeParameter> FindAttributeParameters(
            AttributeSyntax atrybutSyntax)
        {
            if (atrybutSyntax.ArgumentList == null)
                return new List<AttributeParameter>();
            else
                return
                    atrybutSyntax
                        .ArgumentList
                            .Arguments
                                .Select(o => GetAttributeParameter(o))
                                    .ToList();
        }

        private AttributeParameter GetAttributeParameter(AttributeArgumentSyntax attributeArgumentSyntax)
        {
            var result = new AttributeParameter();
            var position = GetPosition(attributeArgumentSyntax.SyntaxTree, attributeArgumentSyntax.Span);

            result.StartPosition = position.Item1.ToPlaceInFile();
            result.EndPosition = position.Item2.ToPlaceInFile();
            result.Value = attributeArgumentSyntax.Expression?.ToFullString()?.Trim();
            result.Name = attributeArgumentSyntax.NameEquals?.Name?.ToFullString()?.Trim();

            if (result.Name == null)
                result.Name = "";

            return result;

        }

        private void SetPosition(
            ParsedUnit parsedUnit,
            SyntaxNode syntaxNode)
        {
            SetPosition(
                syntaxNode.SyntaxTree,
                parsedUnit,
                syntaxNode);
        }

        private void SetPosition(
            SyntaxTree syntaxTree,
            ParsedUnit parsedUnit,
            SyntaxNode syntaxNode)
        {
            var l = syntaxTree.GetLineSpan(syntaxNode.Span);
            var l1 = l.StartLinePosition;
            var l2 = l.EndLinePosition;

            parsedUnit.StartPosition = new PlaceInFile(
                l1.Line + 1,
                l1.Character + 1);
            parsedUnit.EndPosition = new PlaceInFile(
                l2.Line + 1,
                l2.Character + 1);
        }

        private void SetPosition(SyntaxToken token, ParsedUnit parsedUnit)
        {
            var position = GetPosistion(token);
            parsedUnit.StartPosition = position.Item1.ToPlaceInFile();
            parsedUnit.EndPosition = position.Item2.ToPlaceInFile();
        }

        private void SetPosition(SyntaxTrivia trivia, ParsedUnit parsedUnit)
        {
            var position = GetPosition(trivia.SyntaxTree, trivia.FullSpan);
            parsedUnit.StartPosition = position.Item1.ToPlaceInFile();
            parsedUnit.EndPosition = position.Item2.ToPlaceInFile();
        }

        private Tuple<LinePosition, LinePosition> GetPosition(
            SyntaxTree syntaxTree,
            SyntaxToken token)
        {
            return GetPosition(syntaxTree, token.Span);
        }

        private Tuple<LinePosition, LinePosition> GetPosistion(SyntaxToken token)
        {
            return GetPosition(token.SyntaxTree, token.Span);
        }

        private Tuple<LinePosition, LinePosition> GetPosition(
            SyntaxTree syntaxTree,
            TextSpan span)
        {
            var l = syntaxTree.GetLineSpan(span);
            var l1 = l.StartLinePosition;
            var l2 = l.EndLinePosition;

            var newL1 = new LinePosition(l1.Line + 1, l1.Character + 1);
            var newL2 = new LinePosition(l2.Line + 1, l2.Character + 1);

            return Tuple.Create(newL1, newL2);
        }
    }
}