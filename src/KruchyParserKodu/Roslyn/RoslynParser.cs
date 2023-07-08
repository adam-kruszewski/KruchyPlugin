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
        public FileWithCode Parsuj(string zawartosc)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(zawartosc);
            var root = syntaxTree.GetRoot() as CompilationUnitSyntax;

            DisplayHierachy(root);

            var wynik = new FileWithCode();


            BaseNamespaceDeclarationSyntax namespaceDeclaration =
                root.Members.OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

            var fileScopedNamespaceDeclaration =
                root.Members.OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault();

            if (fileScopedNamespaceDeclaration != null)
                namespaceDeclaration = fileScopedNamespaceDeclaration;

            if (namespaceDeclaration != null)
                wynik.Namespace = namespaceDeclaration.Name.ToString();


            foreach (var u in root.Usings)
                wynik.Usings.Add(DajUsing(u));

            var klasy =
                namespaceDeclaration.Members.OfType<ClassDeclarationSyntax>();

            foreach (var klasa in klasy)
            {
                var definiowanyObiekt = ParsujKlase(syntaxTree, klasa);

                wynik.DefinedItems.Add(definiowanyObiekt);
            }

            var interfejsy = namespaceDeclaration.Members.OfType<InterfaceDeclarationSyntax>();
            foreach (var interfaceSyntax in interfejsy)
            {
                wynik.DefinedItems.Add(ParsujInterfejs(interfaceSyntax));
            }

            var enumeracje = namespaceDeclaration.Members.OfType<EnumDeclarationSyntax>();
            foreach (var enumSyntax in enumeracje)
            {
                wynik.DefinedEnumerations.Add(ParsujEnumeracje(enumSyntax));
            }

            return wynik;
        }

        private Enumeration ParsujEnumeracje(EnumDeclarationSyntax enumSyntax)
        {
            var definiowanaEnumeracja = new Enumeration();
            definiowanaEnumeracja.Name = enumSyntax.Identifier.ValueText;
            UstawPolozenie(enumSyntax.SyntaxTree, definiowanaEnumeracja, enumSyntax);

            UzupelnijPola(definiowanaEnumeracja.Fields, enumSyntax, null);

            UstawPolozeniePoczatkowejKlamerki(definiowanaEnumeracja, enumSyntax.OpenBraceToken);
            UstawPolozenieKoncowejKlamerki(definiowanaEnumeracja, enumSyntax.CloseBraceToken);

            UzupelnijAtrybuty(enumSyntax.AttributeLists, definiowanaEnumeracja.Attributes);
            UzupelnijModyfikatory(enumSyntax.Modifiers, definiowanaEnumeracja.Modifiers);

            ParsujDokumentacje(definiowanaEnumeracja, enumSyntax);
            ParsujKomentarz(definiowanaEnumeracja, enumSyntax);

            return definiowanaEnumeracja;
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

        private void ParsujKomentarz(IWithComment obiekt, SyntaxNode syntaxNode)
        {
            var trivias = syntaxNode.GetLeadingTrivia();

            foreach (var trivia in trivias.Where(o => o.IsKind(SyntaxKind.SingleLineCommentTrivia)))
            {
                if (obiekt.Comment == null)
                    obiekt.Comment = new Comment();
                obiekt.Comment.AddLine(trivia.ToString());
                UstawPolozenie(trivia, obiekt.Comment);
            }
        }

        private void ParsujDokumentacje(IWithDocumentation obiekt, SyntaxNode syntaxNode)
        {
            var trivias = syntaxNode.GetLeadingTrivia();

            foreach (var trivia in trivias.Where(o => o.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)))
            {
                if (obiekt.Documentation == null)
                    obiekt.Documentation = new Documentation();
                obiekt.Documentation.AddDocumentation(trivia.ToFullString());
                UstawPolozenie(trivia, obiekt.Documentation);
            }
        }

        private UsingNamespace DajUsing(UsingDirectiveSyntax u)
        {
            var wynik = new UsingNamespace(u.Name.ToString());
            var polozenie = DajPolozenie(u.SyntaxTree, u.Span);
            wynik.StartPosition = polozenie.Item1.ToPozycjaWPliku();
            wynik.EndPosition = polozenie.Item2.ToPozycjaWPliku();
            return wynik;
        }

        private DefinedItem ParsujInterfejs(InterfaceDeclarationSyntax interfaceSyntax)
        {
            var definiowanyObiekt = new DefinedItem();
            definiowanyObiekt.Name = interfaceSyntax.Identifier.ValueText;
            definiowanyObiekt.KindOfItem = KindOfItem.Interface;
            UstawPolozenie(interfaceSyntax.Keyword, definiowanyObiekt.KindOfObjectUnit);

            UstawPolozenie(interfaceSyntax.SyntaxTree, definiowanyObiekt, interfaceSyntax);
            UstawPolozeniePoczatkowejKlamerki(definiowanyObiekt, interfaceSyntax.OpenBraceToken);
            UstawPolozenieKoncowejKlamerki(definiowanyObiekt, interfaceSyntax.CloseBraceToken);

            UzupelnijAtrybuty(interfaceSyntax.AttributeLists, definiowanyObiekt.Attributes);
            UzupelnijModyfikatory(interfaceSyntax.Modifiers, definiowanyObiekt.Modifiers);
            UzupelnijWlasciwosci(definiowanyObiekt.Properties, interfaceSyntax, definiowanyObiekt);
            UzupelnijMetody(definiowanyObiekt.Methods, interfaceSyntax, definiowanyObiekt);
            UzupelniejTypyDziedziczone(definiowanyObiekt, interfaceSyntax);

            ParsujDokumentacje(definiowanyObiekt, interfaceSyntax);
            ParsujKomentarz(definiowanyObiekt, interfaceSyntax);

            ParsujParametryGeneryczne(definiowanyObiekt, interfaceSyntax.TypeParameterList);

            return definiowanyObiekt;
        }

        private DefinedItem ParsujKlase(SyntaxTree syntaxTree, ClassDeclarationSyntax klasa)
        {
            var definiowanyObiekt = new DefinedItem();
            definiowanyObiekt.Name = klasa.Identifier.ValueText;
            definiowanyObiekt.KindOfItem = KindOfItem.Class;
            UstawPolozenie(klasa.Keyword, definiowanyObiekt.KindOfObjectUnit);

            UstawPolozenie(syntaxTree, definiowanyObiekt, klasa);
            UstawPolozeniePoczatkowejKlamerki(definiowanyObiekt, klasa.OpenBraceToken);
            UstawPolozenieKoncowejKlamerki(definiowanyObiekt, klasa.CloseBraceToken);

            UzupelnijAtrybuty(klasa.AttributeLists, definiowanyObiekt.Attributes);

            UzupelnijModyfikatory(klasa.Modifiers, definiowanyObiekt.Modifiers);

            UzupelnijPola(definiowanyObiekt.Fields, klasa, definiowanyObiekt);

            UzupelnijWlasciwosci(definiowanyObiekt.Properties, klasa, definiowanyObiekt);

            UzupelnijKontruktory(definiowanyObiekt.Constructors, klasa, definiowanyObiekt);

            UzupelnijMetody(definiowanyObiekt.Methods, klasa, definiowanyObiekt);

            UzupelniejTypyDziedziczone(definiowanyObiekt, klasa);

            ParsujKomentarz(definiowanyObiekt, klasa);

            ParsujDokumentacje(definiowanyObiekt, klasa);

            ParsujParametryGeneryczne(definiowanyObiekt, klasa.TypeParameterList);

            var klasyWewnetrzne = SzukajKlasWewnetrznych(klasa);

            foreach (var klasaWewnetrzna in klasyWewnetrzne)
            {
                klasaWewnetrzna.Owner = definiowanyObiekt;
                definiowanyObiekt.InternalDefinedItems.Add(klasaWewnetrzna);
            }

            return definiowanyObiekt;
        }

        private void ParsujParametryGeneryczne(DefinedItem obiekt, TypeParameterListSyntax parameterListSyntax)
        {
            ParsujParametryGeneryczne(obiekt.GenericParameters, parameterListSyntax);
        }

        private void ParsujParametryGeneryczne(
            IList<GenericParameter> parametryGeneryczne,
            TypeParameterListSyntax parameterListSyntax)
        {
            if (parameterListSyntax == null)
                return;

            foreach (var typeParameter in parameterListSyntax.Parameters)
            {
                var parametrGeneryczny = new GenericParameter
                {
                    Name = typeParameter.Identifier.ValueText
                };

                UstawPolozenie(typeParameter.SyntaxTree, parametrGeneryczny, typeParameter);

                parametryGeneryczne.Add(parametrGeneryczny);
            }
        }

        private void UzupelniejTypyDziedziczone(DefinedItem obiekt, TypeDeclarationSyntax syntax)
        {
            if (syntax.BaseList != null)
                obiekt.SuperClassAndInterfaces.AddRange(
                    syntax.BaseList.Types.Select(o => DajTypDziedziczony(o)));

        }

        private IEnumerable<DefinedItem> SzukajKlasWewnetrznych(ClassDeclarationSyntax klasaZewnetrzna)
        {
            foreach (var klasaSyntax in klasaZewnetrzna.Members.OfType<ClassDeclarationSyntax>())
            {
                var klasa = ParsujKlase(klasaZewnetrzna.SyntaxTree, klasaSyntax);
                yield return klasa;
            }
        }

        private DerivedObject DajTypDziedziczony(BaseTypeSyntax o)
        {
            var wynik = new DerivedObject();

            wynik.Name = o.Type.DajNazweTypu();
            wynik.StartPosition = DajPolozenie(o.SyntaxTree, o.Span).Item1.ToPozycjaWPliku();
            wynik.EndPosition = DajPolozenie(o.SyntaxTree, o.Span).Item2.ToPozycjaWPliku();

            if (o.Type.JestGeneryczny())
            {
                var daneGenerycznego = o.Type.DajDaneTypuGenerycznego();
                wynik.Name = daneGenerycznego.Item1;
                wynik.ParameterTypeNames.AddRange(daneGenerycznego.Item2);
            }

            return wynik;
        }

        private void UzupelnijMetody(
            IList<Method> metody,
            TypeDeclarationSyntax klasa,
            DefinedItem obiektWlasciciela)
        {
            foreach (var metodaSyntax in klasa.Members.OfType<MethodDeclarationSyntax>())
            {
                var metoda = new Method();
                metoda.Name = metodaSyntax.Identifier.ValueText;
                UzupelnijModyfikatory(metodaSyntax.Modifiers, metoda.Modyfikatory);
                UzupelnijParametry(metodaSyntax.ParameterList, metoda.Parametry);

                UstawPolozenie(klasa.SyntaxTree, metoda, metodaSyntax);

                metoda.ReturnType = new TypZwracany
                {
                    Nazwa = metodaSyntax.ReturnType.DajNazweTypu()
                };

                UstawPolozenie(klasa.SyntaxTree, metoda.ReturnType, metodaSyntax.ReturnType);

                UzupelnijPozycjeNawiasowOtwierajacychIZamykajacych(metodaSyntax, metoda);

                UzupelnijAtrybuty(metodaSyntax.AttributeLists, metoda.Atrybuty);

                metoda.Instructions.AddRange(ParseInstructions(metodaSyntax, metoda));

                ParsujDokumentacje(metoda, metodaSyntax);

                ParsujKomentarz(metoda, metodaSyntax);

                ParsujParametryGeneryczne(metoda.GenericParameters, metodaSyntax.TypeParameterList);

                metoda.Owner = obiektWlasciciela;

                metody.Add(metoda);
            }
        }

        private void UzupelnijKontruktory(
            IList<Constructor> konstruktory,
            ClassDeclarationSyntax klasa,
            DefinedItem obiektWlasciciel)
        {
            foreach (var konstruktorSyntax in klasa.Members.OfType<ConstructorDeclarationSyntax>())
            {
                Constructor konstruktor = ParseConstructor(obiektWlasciciel, konstruktorSyntax);

                konstruktory.Add(konstruktor);
            }
        }

        private Constructor ParseConstructor(
            DefinedItem obiektWlasciciel,
            ConstructorDeclarationSyntax konstruktorSyntax)
        {
            var konstruktor = new Constructor();

            UstawPolozenie(konstruktorSyntax.SyntaxTree, konstruktor, konstruktorSyntax);
            konstruktor.Modifier =
                SzukajModyfikatorow(konstruktorSyntax.Modifiers)
                    .Select(o => o.Name)
                        .SingleOrDefault();

            UzupelnijParametry(konstruktorSyntax.ParameterList, konstruktor.Parametry);

            UzupelnijPozycjeNawiasowOtwierajacychIZamykajacych(
                konstruktorSyntax,
                konstruktor);

            UzupelnijPozycjeKlamerek(konstruktorSyntax, konstruktor);

            ParsujDokumentacje(konstruktor, konstruktorSyntax);

            konstruktor.Owner = obiektWlasciciel;

            if (konstruktorSyntax.Initializer != null
                && konstruktorSyntax.Initializer.ArgumentList != null)
            {
                konstruktor.InitializationKeyWord =
                    konstruktorSyntax.Initializer.ThisOrBaseKeyword.ToString();
                konstruktor.ParentClassContructorParameters =
                    konstruktorSyntax
                        .Initializer
                            .ArgumentList
                                .Arguments
                                    .Select(o => o.ToString().Trim())
                                        .ToList();
            }

            konstruktor.Instructions.AddRange(ParseInstructions(konstruktorSyntax, konstruktor));

            return konstruktor;
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

                    UstawPolozenie(instruction, instructionSyntax);

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

        private void UzupelnijPozycjeNawiasowOtwierajacychIZamykajacych(
            BaseMethodDeclarationSyntax syntax,
            IWithParameterBraces obiekt)
        {
            obiekt.StartingParameterBrace =
                DajPolozenie(syntax.ParameterList.OpenParenToken).Item1.ToPozycjaWPliku();
            obiekt.ClosingParameterBrace =
                DajPolozenie(syntax.ParameterList.CloseParenToken).Item1.ToPozycjaWPliku();
        }

        private void UzupelnijPozycjeKlamerek(
            BaseMethodDeclarationSyntax syntax,
            IWithBraces obiekt)
        {
            obiekt.StartingBrace =
                DajPolozenie(syntax.Body.OpenBraceToken)
                    .Item1.ToPozycjaWPliku();

            obiekt.ClosingBrace =
                DajPolozenie(syntax.Body.CloseBraceToken)
                    .Item1.ToPozycjaWPliku();

        }

        private void UzupelnijParametry(ParameterListSyntax parameterList, IList<Parameter> parametry)
        {
            parametry.AddRange(parameterList.Parameters.Select(o => DajParametr(o)));
        }

        private Parameter DajParametr(ParameterSyntax parametrSyntax)
        {
            var parametr = new Parameter();
            parametr.ParameterName = parametrSyntax.Identifier.ValueText;
            parametr.TypeName = parametrSyntax.Type.DajNazweTypu();

            parametr.Modifier =
                SzukajModyfikatorow(parametrSyntax.Modifiers)
                    .Select(o => o.Name)
                        .SingleOrDefault();

            if (parametr.Modifier == "this")
                parametr.WithThis = true;
            if (parametr.Modifier == "out")
                parametr.WithOut = true;
            if (parametr.Modifier == "params")
                parametr.WithParams = true;
            if (parametr.Modifier == "ref")
                parametr.WithRef = true;
            if (parametrSyntax.Default != null)
                parametr.DefaultValue =
                    parametrSyntax.Default.Value.ToFullString().Trim();

            UzupelnijAtrybuty(parametrSyntax.AttributeLists, parametr.Attributes);

            return parametr;
        }

        private void UzupelnijWlasciwosci(
            IList<Property> propertiesy,
            TypeDeclarationSyntax klasa,
            DefinedItem definiowanyObiekt)
        {
            var wlasciwosciSyntax = klasa.Members.OfType<PropertyDeclarationSyntax>();

            foreach (var wlasciwoscSyntax in wlasciwosciSyntax)
            {
                var properties = new Property();
                properties.Name = wlasciwoscSyntax.Identifier.ValueText;
                properties.TypeName = wlasciwoscSyntax.Type.DajNazweTypu();
                UzupelnijAtrybuty(wlasciwoscSyntax.AttributeLists, properties.Attributes);
                UzupelnijModyfikatory(wlasciwoscSyntax.Modifiers, properties.Modifiers);
                UstawPolozenie(wlasciwoscSyntax.SyntaxTree, properties, wlasciwoscSyntax);
                ParsujDokumentacje(properties, wlasciwoscSyntax);

                properties.HasGet = JestAccessorr(wlasciwoscSyntax, "get");
                properties.HasSet = JestAccessorr(wlasciwoscSyntax, "set");

                properties.Owner = definiowanyObiekt;

                propertiesy.Add(properties);
            }
        }

        private bool JestAccessorr(PropertyDeclarationSyntax syntax, string nazwa)
        {
            if (syntax.AccessorList == null)
                return false;

            return syntax.AccessorList.Accessors.Any(o => o.Keyword.ValueText == nazwa);
        }

        private void UzupelnijPola(IList<Field> pola, ClassDeclarationSyntax klasa, DefinedItem wlasciciel)
        {
            var deklaracjePol = klasa.Members.OfType<FieldDeclarationSyntax>();

            foreach (var deklarowanePole in deklaracjePol)
            {
                var pole = new Field();
                var identyfikator = deklarowanePole
                        .Declaration
                            .Variables
                                .SingleOrDefault();

                if (identyfikator == null)
                    continue;

                pole.Name = identyfikator.Identifier.ValueText;

                pole.TypeName = deklarowanePole.Declaration.Type.DajNazweTypu();

                pole.Modifiers.AddRange(
                    deklarowanePole.Modifiers.Select(o => DajModifikator(o)));

                UstawPolozenie(deklarowanePole.SyntaxTree, pole, deklarowanePole);

                ParsujDokumentacje(pole, deklarowanePole);

                pole.Owner = wlasciciel;

                pola.Add(pole);
            }
        }


        private void UzupelnijPola(IList<Field> pola, EnumDeclarationSyntax klasa, DefinedItem wlasciciel)
        {
            var deklaracjePol = klasa.Members.OfType<EnumMemberDeclarationSyntax>();

            foreach (var deklarowanePole in deklaracjePol)
            {
                var pole = new Field();
                pole.Name = deklarowanePole.Identifier.ValueText;

                UstawPolozenie(deklarowanePole.SyntaxTree, pole, deklarowanePole);

                ParsujDokumentacje(pole, deklarowanePole);

                pole.Owner = wlasciciel;

                pola.Add(pole);
            }
        }


        private void UzupelnijModyfikatory(
            SyntaxTokenList syntax,
            IList<Modifier> modyfikatory)
        {
            foreach (var modyfikator in SzukajModyfikatorow(syntax))
                modyfikatory.Add(modyfikator);
        }

        private IEnumerable<Modifier> SzukajModyfikatorow(SyntaxTokenList syntax)
        {
            return syntax.Select(o => DajModifikator(o));
        }

        private Modifier DajModifikator(SyntaxToken o)
        {
            var modyfikator = new Modifier(o.ValueText);

            UstawPolozenie(o, modyfikator);

            return modyfikator;
        }

        private void UstawPolozeniePoczatkowejKlamerki(
            IWithBraces obiekt,
            SyntaxToken token)
        {
            var pozycjaPoczatkowejKlamerki = DajPolozenie(token.SyntaxTree, token);
            obiekt.StartingBrace.Row =
                pozycjaPoczatkowejKlamerki.Item1.Line;
            obiekt.StartingBrace.Column =
                pozycjaPoczatkowejKlamerki.Item1.Character;
        }

        private void UstawPolozenieKoncowejKlamerki(
            IWithBraces obiekt,
            SyntaxToken token)
        {
            var pozycjaKoncowejKlamerki = DajPolozenie(token.SyntaxTree, token);
            obiekt.ClosingBrace = pozycjaKoncowejKlamerki.Item1.ToPozycjaWPliku();
        }

        private void UzupelnijAtrybuty(
            SyntaxList<AttributeListSyntax> attributeLists,
            List<ParserKodu.Models.Attribute> listaAtrybutow)
        {
            foreach (var atrybut in attributeLists)
            {
                listaAtrybutow.AddRange(ParsujAtrybut(atrybut));
            }
        }

        private IEnumerable<ParserKodu.Models.Attribute> ParsujAtrybut(AttributeListSyntax listaAtrybutow)
        {
            foreach (var atrybutSyntax in listaAtrybutow.Attributes)
            {
                yield return ParsujAtrybut(atrybutSyntax);
            }
        }

        private ParserKodu.Models.Attribute ParsujAtrybut(AttributeSyntax atrybutSyntax)
        {
            var atrybut =
                new ParserKodu.Models.Attribute
                {
                    Name = atrybutSyntax.Name.GetText().ToString(),
                    Parameters = SzukajParametrowAtrybutu(atrybutSyntax)
                };
            UstawPolozenie(atrybutSyntax.SyntaxTree, atrybut, atrybutSyntax);
            return atrybut;
        }

        private IList<AttributeParameter> SzukajParametrowAtrybutu(
            AttributeSyntax atrybutSyntax)
        {
            if (atrybutSyntax.ArgumentList == null)
                return new List<AttributeParameter>();
            else
                return
                    atrybutSyntax
                        .ArgumentList
                            .Arguments
                                .Select(o => DajParametrAtrybutu(o))
                                    .ToList();
        }

        private AttributeParameter DajParametrAtrybutu(AttributeArgumentSyntax o)
        {
            var wynik = new AttributeParameter();
            var polozenie = DajPolozenie(o.SyntaxTree, o.Span);

            wynik.StartPosition = polozenie.Item1.ToPozycjaWPliku();
            wynik.EndPosition = polozenie.Item2.ToPozycjaWPliku();
            wynik.Value = o.Expression?.ToFullString()?.Trim();
            wynik.Name = o.NameEquals?.Name?.ToFullString()?.Trim();

            if (wynik.Name == null)
                wynik.Name = "";

            return wynik;

        }

        private void UstawPolozenie(
            ParsedUnit obiekt,
            SyntaxNode wezel)
        {
            UstawPolozenie(
                wezel.SyntaxTree,
                obiekt,
                wezel);
        }

        private void UstawPolozenie(
            SyntaxTree syntaxTree,
            ParsedUnit obiekt,
            SyntaxNode wezel)
        {
            var l = syntaxTree.GetLineSpan(wezel.Span);
            var l1 = l.StartLinePosition;
            var l2 = l.EndLinePosition;

            obiekt.StartPosition = new PlaceInFile(
                l1.Line + 1,
                l1.Character + 1);
            obiekt.EndPosition = new PlaceInFile(
                l2.Line + 1,
                l2.Character + 1);
        }

        private void UstawPolozenie(SyntaxToken token, ParsedUnit obiekt)
        {
            var polozenie = DajPolozenie(token);
            obiekt.StartPosition = polozenie.Item1.ToPozycjaWPliku();
            obiekt.EndPosition = polozenie.Item2.ToPozycjaWPliku();
        }

        private void UstawPolozenie(SyntaxTrivia trivia, ParsedUnit jednostka)
        {
            var polozenie = DajPolozenie(trivia.SyntaxTree, trivia.FullSpan);
            jednostka.StartPosition = polozenie.Item1.ToPozycjaWPliku();
            jednostka.EndPosition = polozenie.Item2.ToPozycjaWPliku();
        }

        private Tuple<LinePosition, LinePosition> DajPolozenie(
            SyntaxTree syntaxTree,
            SyntaxToken token)
        {
            return DajPolozenie(syntaxTree, token.Span);
        }

        private Tuple<LinePosition, LinePosition> DajPolozenie(SyntaxToken token)
        {
            return DajPolozenie(token.SyntaxTree, token.Span);
        }

        private Tuple<LinePosition, LinePosition> DajPolozenie(
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