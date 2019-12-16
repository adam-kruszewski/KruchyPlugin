using System;
using System.Collections.Generic;
using System.Linq;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace KruchyParserKodu.Roslyn
{
    class RoslynParser : IParser
    {
        public Plik Parsuj(string zawartosc)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(zawartosc);
            var root = syntaxTree.GetRoot() as CompilationUnitSyntax;

            var wynik = new Plik();

            var namespaceDeclaration =
                root.Members.OfType<NamespaceDeclarationSyntax>().First();
            wynik.Namespace = namespaceDeclaration.Name.ToString();

            foreach (var u in root.Usings)
                wynik.Usingi.Add(new UsingNamespace(u.Name.ToString()));

            var klasy =
                namespaceDeclaration.Members.OfType<ClassDeclarationSyntax>();

            foreach (var klasa in klasy)
            {
                var definiowanyObiekt = new Obiekt();
                definiowanyObiekt.Nazwa = klasa.Identifier.ValueText;
                definiowanyObiekt.Rodzaj = RodzajObiektu.Klasa;

                UstawPolozenie(syntaxTree, definiowanyObiekt, klasa);
                UstawPolozeniePoczatkowejKlamerki(definiowanyObiekt, klasa.OpenBraceToken);

                UzupelnijAtrybuty(klasa.AttributeLists, definiowanyObiekt.Atrybuty);
                UzupelnijPola(definiowanyObiekt.Pola, klasa);

                UzupelnijWlasciwosci(definiowanyObiekt.Propertiesy, klasa);

                wynik.DefiniowaneObiekty.Add(definiowanyObiekt);
            }
            return wynik;
        }

        private void UzupelnijWlasciwosci(
            IList<Property> propertiesy,
            ClassDeclarationSyntax klasa)
        {
            var wlasciwosciSyntax = klasa.Members.OfType<PropertyDeclarationSyntax>();

            foreach (var wlasciwoscSyntax in wlasciwosciSyntax)
            {
                var properties = new Property();
                properties.Nazwa = wlasciwoscSyntax.Identifier.ValueText;
                properties.NazwaTypu = wlasciwoscSyntax.Type.DajNazweTypu();
                UzupelnijAtrybuty(wlasciwoscSyntax.AttributeLists, properties.Atrybuty);
                UzupelnijModyfikatory(wlasciwoscSyntax.Modifiers, properties.Modyfikatory);
                UstawPolozenie(wlasciwoscSyntax.SyntaxTree, properties, wlasciwoscSyntax);

                properties.JestGet = JestAccessorr(wlasciwoscSyntax, "get");
                properties.JestSet = JestAccessorr(wlasciwoscSyntax, "set");

                propertiesy.Add(properties);
            }
        }

        private bool JestAccessorr(PropertyDeclarationSyntax syntax, string nazwa)
        {
            return syntax.AccessorList.Accessors.Any(o => o.Keyword.ValueText == nazwa);
        }

        private void UzupelnijPola(IList<Pole> pola, ClassDeclarationSyntax klasa)
        {
            var deklaracjePol = klasa.Members.OfType<FieldDeclarationSyntax>();

            foreach (var deklarowanePole in deklaracjePol)
            {
                var pole = new Pole();
                var identyfikator = deklarowanePole
                        .Declaration
                            .Variables
                                .SingleOrDefault();

                if (identyfikator == null)
                    continue;

                var identyfikatorTypu = deklarowanePole.Declaration.Type as IdentifierNameSyntax;
                var alternatywnyIdentyfikatorTypu =
                    deklarowanePole.Declaration.Type as GenericNameSyntax;

                pole.Nazwa = identyfikator.Identifier.ValueText;

                if (identyfikatorTypu != null)
                    pole.NazwaTypu = identyfikatorTypu.Identifier.ValueText;
                if (alternatywnyIdentyfikatorTypu != null)
                    pole.NazwaTypu = alternatywnyIdentyfikatorTypu.ToFullString().Trim();
                //Identifier.ToString();

                pole.Modyfikatory.AddRange(
                    deklarowanePole.Modifiers.Select(o => DajModifikator(o)));

                pola.Add(pole);
            }
        }

        private void UzupelnijModyfikatory(
            SyntaxTokenList syntax,
            IList<Modyfikator> modyfikatory)
        {
            foreach (var modyfikator in syntax.Select(o => DajModifikator(o)))
                modyfikatory.Add(modyfikator);
        }

        private static Modyfikator DajModifikator(SyntaxToken o)
        {
            var modyfikator = new Modyfikator(o.ValueText);
            return modyfikator;
        }

        private void UstawPolozeniePoczatkowejKlamerki(
            IZPoczatkowaIKoncowaKlamerka obiekt,
            SyntaxToken token)
        {
            var pozycjaPoczatkowejKlamerki = DajPolozenie(token.SyntaxTree, token);
            obiekt.PoczatkowaKlamerka.Wiersz =
                pozycjaPoczatkowejKlamerki.Item1.Line;
            obiekt.PoczatkowaKlamerka.Kolumna =
                pozycjaPoczatkowejKlamerki.Item1.Character;

        }

        private void UzupelnijAtrybuty(
            SyntaxList<AttributeListSyntax> attributeLists,
            List<Atrybut> listaAtrybutow)
        {
            foreach (var atrybut in attributeLists)
            {
                listaAtrybutow.AddRange(ParsujAtrybut(atrybut));
            }
        }

        private IEnumerable<Atrybut> ParsujAtrybut(AttributeListSyntax listaAtrybutow)
        {
            foreach (var atrybutSyntax in listaAtrybutow.Attributes)
            {
                yield return ParsujAtrybut(atrybutSyntax);
            }
        }

        private Atrybut ParsujAtrybut(AttributeSyntax atrybutSyntax)
        {
            var atrybut =
                new Atrybut
                {
                    Nazwa = atrybutSyntax.Name.GetText().ToString()
                };
            UstawPolozenie(atrybutSyntax.SyntaxTree, atrybut, atrybutSyntax);
            return atrybut;
        }

        private void UstawPolozenie(
            SyntaxTree syntaxTree,
            ParsowanaJednostka obiekt,
            SyntaxNode wezel)
        {
            var l = syntaxTree.GetLineSpan(wezel.Span);
            var l1 = l.StartLinePosition;
            var l2 = l.EndLinePosition;

            obiekt.Poczatek = new PozycjaWPliku(
                l1.Line,
                l1.Character);
            obiekt.Koniec = new PozycjaWPliku(
                l2.Line,
                l2.Character);
        }

        private Tuple<LinePosition, LinePosition> DajPolozenie(
            SyntaxTree syntaxTree,
            SyntaxNode wezel)
        {
            return DajPolozenie(syntaxTree, wezel.Span);
        }

        private Tuple<LinePosition, LinePosition> DajPolozenie(
            SyntaxTree syntaxTree,
            SyntaxToken token)
        {
            return DajPolozenie(syntaxTree, token.Span);
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