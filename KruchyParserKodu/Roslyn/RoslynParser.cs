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
                var definiowanyObiekt = ParsujKlase(syntaxTree, klasa);

                wynik.DefiniowaneObiekty.Add(definiowanyObiekt);
            }

            var interfejsy = namespaceDeclaration.Members.OfType<InterfaceDeclarationSyntax>();
            foreach (var interfaceSyntax in interfejsy)
            {
                wynik.DefiniowaneObiekty.Add(ParsujInterfejs(interfaceSyntax));
            }

            return wynik;
        }

        private Obiekt ParsujInterfejs(InterfaceDeclarationSyntax interfaceSyntax)
        {
            var definiowanyObiekt = new Obiekt();
            definiowanyObiekt.Rodzaj = RodzajObiektu.Interfejs;

            UstawPolozenie(interfaceSyntax.SyntaxTree, definiowanyObiekt, interfaceSyntax);
            UstawPolozeniePoczatkowejKlamerki(definiowanyObiekt, interfaceSyntax.OpenBraceToken);
            UstawPolozenieKoncowejKlamerki(definiowanyObiekt, interfaceSyntax.CloseBraceToken);

            UzupelnijAtrybuty(interfaceSyntax.AttributeLists, definiowanyObiekt.Atrybuty);
            UzupelnijWlasciwosci(definiowanyObiekt.Propertiesy, interfaceSyntax);
            UzupelnijMetody(definiowanyObiekt.Metody, interfaceSyntax, definiowanyObiekt);
            UzupelniejTypyDziedziczone(definiowanyObiekt, interfaceSyntax);

            return definiowanyObiekt;
        }

        private Obiekt ParsujKlase(SyntaxTree syntaxTree, ClassDeclarationSyntax klasa)
        {
            var definiowanyObiekt = new Obiekt();
            definiowanyObiekt.Nazwa = klasa.Identifier.ValueText;
            definiowanyObiekt.Rodzaj = RodzajObiektu.Klasa;

            UstawPolozenie(syntaxTree, definiowanyObiekt, klasa);
            UstawPolozeniePoczatkowejKlamerki(definiowanyObiekt, klasa.OpenBraceToken);

            UzupelnijAtrybuty(klasa.AttributeLists, definiowanyObiekt.Atrybuty);
            UzupelnijPola(definiowanyObiekt.Pola, klasa);

            UzupelnijWlasciwosci(definiowanyObiekt.Propertiesy, klasa);

            UzupelnijKontruktory(definiowanyObiekt.Konstruktory, klasa, definiowanyObiekt);

            UzupelnijMetody(definiowanyObiekt.Metody, klasa, definiowanyObiekt);

            UzupelniejTypyDziedziczone(definiowanyObiekt, klasa);

            var klasyWewnetrzne = SzukajKlasWewnetrznych(klasa);

            foreach (var klasaWewnetrzna in klasyWewnetrzne)
            {
                klasaWewnetrzna.Wlasciciel = definiowanyObiekt;
                definiowanyObiekt.ObiektyWewnetrzne.Add(klasaWewnetrzna);
            }

            return definiowanyObiekt;
        }

        private void UzupelniejTypyDziedziczone(Obiekt obiekt, TypeDeclarationSyntax syntax)
        {
            if (syntax.BaseList != null)
                obiekt.NadklasaIInterfejsy.AddRange(
                    syntax.BaseList.Types.Select(o => DajTypDziedziczony(o)));

        }

        private IEnumerable<Obiekt> SzukajKlasWewnetrznych(ClassDeclarationSyntax klasaZewnetrzna)
        {
            foreach (var klasaSyntax in klasaZewnetrzna.Members.OfType<ClassDeclarationSyntax>())
            {
                var klasa = ParsujKlase(klasaZewnetrzna.SyntaxTree, klasaSyntax);
                yield return klasa;
            }
        }

        private ObiektDziedziczony DajTypDziedziczony(BaseTypeSyntax o)
        {
            var wynik = new ObiektDziedziczony();

            wynik.Nazwa = o.Type.DajNazweTypu();
            wynik.Poczatek = DajPolozenie(o.SyntaxTree, o.Span).Item1.ToPozycjaWPliku();
            wynik.Koniec = DajPolozenie(o.SyntaxTree, o.Span).Item2.ToPozycjaWPliku();

            if (o.Type.JestGeneryczny())
            {
                var daneGenerycznego = o.Type.DajDaneTypuGenerycznego();
                wynik.Nazwa = daneGenerycznego.Item1;
                wynik.NazwyTypowParametrow.AddRange(daneGenerycznego.Item2);
            }

            return wynik;
        }

        private void UzupelnijMetody(
            IList<Metoda> metody,
            TypeDeclarationSyntax klasa,
            Obiekt obiektWlasciciela)
        {
            foreach (var metodaSyntax in klasa.Members.OfType<MethodDeclarationSyntax>())
            {
                var metoda = new Metoda();
                metoda.Nazwa = metodaSyntax.Identifier.ValueText;
                UzupelnijModyfikatory(metodaSyntax.Modifiers, metoda.Modyfikatory);
                UzupelnijParametry(metodaSyntax.ParameterList, metoda.Parametry);

                UstawPolozenie(klasa.SyntaxTree, metoda, metodaSyntax);

                metoda.TypZwracany = metodaSyntax.ReturnType.DajNazweTypu();

                UzupelnijPozycjeNawiasowOtwierajacychIZamykajacych(metodaSyntax, metoda);

                UzupelnijAtrybuty(metodaSyntax.AttributeLists, metoda.Atrybuty);

                metoda.Wlasciciel = obiektWlasciciela;

                metody.Add(metoda);
            }
        }

        private void UzupelnijKontruktory(
            IList<Konstruktor> konstruktory,
            ClassDeclarationSyntax klasa,
            Obiekt obiektWlasciciel)
        {
            foreach (var konstruktorSyntax in klasa.Members.OfType<ConstructorDeclarationSyntax>())
            {
                var konstruktor = new Konstruktor();

                UstawPolozenie(konstruktorSyntax.SyntaxTree, konstruktor, konstruktorSyntax);
                konstruktor.Modyfikator =
                    SzukajModyfikatorow(konstruktorSyntax.Modifiers)
                        .Select(o => o.Nazwa)
                            .SingleOrDefault();

                UzupelnijParametry(konstruktorSyntax.ParameterList, konstruktor.Parametry);

                UzupelnijPozycjeNawiasowOtwierajacychIZamykajacych(
                    konstruktorSyntax,
                    konstruktor);

                UzupelnijPozycjeKlamerek(konstruktorSyntax, konstruktor);

                konstruktor.Wlasciciel = obiektWlasciciel;

                if (konstruktorSyntax.Initializer != null
                    && konstruktorSyntax.Initializer.ArgumentList != null)
                {
                    konstruktor.SlowoKluczoweInicjalizacji =
                        konstruktorSyntax.Initializer.ThisOrBaseKeyword.ToString();
                    konstruktor.ParametryKonstruktoraZNadKlasy =
                        konstruktorSyntax
                            .Initializer
                                .ArgumentList
                                    .Arguments
                                        .Select(o => o.ToString().Trim())
                                            .ToList();
                }

                konstruktory.Add(konstruktor);
            }
        }

        private void UzupelnijPozycjeNawiasowOtwierajacychIZamykajacych(
            BaseMethodDeclarationSyntax syntax,
            IZNawiasamiOtwierajacymiZamykajacymiParametry obiekt)
        {
            obiekt.NawiasOtwierajacyParametry =
                DajPolozenie(syntax.ParameterList.OpenParenToken).Item1.ToPozycjaWPliku();
            obiekt.NawiasZamykajacyParametry =
                DajPolozenie(syntax.ParameterList.CloseParenToken).Item1.ToPozycjaWPliku();
        }

        private void UzupelnijPozycjeKlamerek(
            BaseMethodDeclarationSyntax syntax,
            IZPoczatkowaIKoncowaKlamerka obiekt)
        {
            obiekt.PoczatkowaKlamerka =
                DajPolozenie(syntax.Body.OpenBraceToken)
                    .Item1.ToPozycjaWPliku();

            obiekt.KoncowaKlamerka =
                DajPolozenie(syntax.Body.CloseBraceToken)
                    .Item1.ToPozycjaWPliku();

        }

        private void UzupelnijParametry(ParameterListSyntax parameterList, IList<Parametr> parametry)
        {
            parametry.AddRange(parameterList.Parameters.Select(o => DajParametr(o)));
        }

        private Parametr DajParametr(ParameterSyntax parametrSyntax)
        {
            var parametr = new Parametr();
            parametr.NazwaParametru = parametrSyntax.Identifier.ValueText;
            parametr.NazwaTypu = parametrSyntax.Type.DajNazweTypu();

            parametr.Modyfikator =
                SzukajModyfikatorow(parametrSyntax.Modifiers)
                    .Select(o => o.Nazwa)
                        .SingleOrDefault();

            if (parametr.Modyfikator == "this")
                parametr.ZThisem = true;
            if (parametr.Modyfikator == "out")
                parametr.ZOut = true;
            if (parametr.Modyfikator == "params")
                parametr.ZParams = true;
            if (parametr.Modyfikator == "ref")
                parametr.ZRef = true;
            if (parametrSyntax.Default != null)
                parametr.WartoscDomyslna =
                    parametrSyntax.Default.Value.ToFullString().Trim();

            return parametr;
        }

        private void UzupelnijWlasciwosci(
            IList<Property> propertiesy,
            TypeDeclarationSyntax klasa)
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
            if (syntax.AccessorList == null)
                return false;

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

                pole.Nazwa = identyfikator.Identifier.ValueText;

                pole.NazwaTypu = deklarowanePole.Declaration.Type.DajNazweTypu();

                pole.Modyfikatory.AddRange(
                    deklarowanePole.Modifiers.Select(o => DajModifikator(o)));

                pola.Add(pole);
            }
        }

        private void UzupelnijModyfikatory(
            SyntaxTokenList syntax,
            IList<Modyfikator> modyfikatory)
        {
            foreach (var modyfikator in SzukajModyfikatorow(syntax))
                modyfikatory.Add(modyfikator);
        }

        private IEnumerable<Modyfikator> SzukajModyfikatorow(SyntaxTokenList syntax)
        {
            return syntax.Select(o => DajModifikator(o));
        }

        private Modyfikator DajModifikator(SyntaxToken o)
        {
            var modyfikator = new Modyfikator(o.ValueText);

            UstawPolozenie(o, modyfikator);

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

        private void UstawPolozenieKoncowejKlamerki(
            IZPoczatkowaIKoncowaKlamerka obiekt,
            SyntaxToken token)
        {
            var pozycjaKoncowejKlamerki = DajPolozenie(token.SyntaxTree, token);
            obiekt.KoncowaKlamerka = pozycjaKoncowejKlamerki.Item1.ToPozycjaWPliku();
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
                    Nazwa = atrybutSyntax.Name.GetText().ToString(),
                    Parametry = SzukajParametrowAtrybutu(atrybutSyntax)
                };
            UstawPolozenie(atrybutSyntax.SyntaxTree, atrybut, atrybutSyntax);
            return atrybut;
        }

        private IList<ParametrAtrybutu> SzukajParametrowAtrybutu(
            AttributeSyntax atrybutSyntax)
        {
            if (atrybutSyntax.ArgumentList == null)
                return new List<ParametrAtrybutu>();
            else
                return
                    atrybutSyntax
                        .ArgumentList
                            .Arguments
                                .Select(o => DajParametrAtrybutu(o))
                                    .ToList();
        }

        private ParametrAtrybutu DajParametrAtrybutu(AttributeArgumentSyntax o)
        {
            var wynik = new ParametrAtrybutu();
            var polozenie = DajPolozenie(o.SyntaxTree, o.Span);

            wynik.Poczatek = polozenie.Item1.ToPozycjaWPliku();
            wynik.Koniec = polozenie.Item2.ToPozycjaWPliku();
            wynik.Wartosc = o.Expression?.ToFullString()?.Trim();
            wynik.Nazwa = o.NameEquals?.Name?.ToFullString()?.Trim();

            if (wynik.Nazwa == null)
                wynik.Nazwa = "";

            return wynik;

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
                l1.Line + 1,
                l1.Character + 1);
            obiekt.Koniec = new PozycjaWPliku(
                l2.Line + 1,
                l2.Character + 1);
        }

        private void UstawPolozenie(SyntaxToken token, ParsowanaJednostka obiekt)
        {
            var polozenie = DajPolozenie(token);
            obiekt.Poczatek = polozenie.Item1.ToPozycjaWPliku();
            obiekt.Koniec = polozenie.Item2.ToPozycjaWPliku();
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