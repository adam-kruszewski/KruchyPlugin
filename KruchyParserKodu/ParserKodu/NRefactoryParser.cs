﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;

namespace KruchyParserKodu.ParserKodu
{
    class NRefactoryParser : IParser
    {
        public Plik Parsuj(string zawartosc)
        {
            var csharpParser = new CSharpParser();
            var wersja = new Version(7, 3);

            csharpParser.CompilerSettings.LanguageVersion = wersja;
            var drzewo = csharpParser.Parse(zawartosc);
            var wynik = new Plik();

            foreach (var wezel in drzewo.Children)
            {
                if (WezelUsing(wezel))
                {
                    wynik.Usingi.Add(ParsujUsing(wezel));
                    continue;
                }
                if (WezelNamespace(wezel))
                {
                    ParsujNamespace(wezel, wynik);
                    continue;
                }
            }

            return wynik;
        }

        public Plik ParsujPlik(string nazwaPliku)
        {
            var zawartosc = File.ReadAllText(nazwaPliku, Encoding.UTF8);
            return Parsuj(zawartosc);
        }


        private bool WezelUsing(AstNode wezel)
        {
            UsingDeclaration ud = wezel as UsingDeclaration;
            return ud != null;
        }

        private UsingNamespace ParsujUsing(AstNode wezel)
        {
            UsingDeclaration ud = wezel as UsingDeclaration;
            var wynik = new UsingNamespace(ud.Namespace);
            UstawPolozenie(wynik, wezel);
            return wynik;
        }

        private bool WezelNamespace(AstNode wezel)
        {
            NamespaceDeclaration nd = wezel as NamespaceDeclaration;
            return nd != null;
        }

        private void ParsujNamespace(AstNode wezel, Plik wynik)
        {
            NamespaceDeclaration nd = wezel as NamespaceDeclaration;
            wynik.Namespace = nd.Name;

            foreach (var dziecko in wezel.Children)
            {
                if (WezelKlasa(dziecko))
                {
                    var klasa = ParsujKlase(dziecko);
                    wynik.DefiniowaneObiekty.Add(klasa);
                    continue;
                }
            }
        }

        private bool WezelKlasa(AstNode wezel)
        {
            TypeDeclaration td = wezel as TypeDeclaration;
            return td != null;
        }

        private Obiekt ParsujKlase(AstNode wezel)
        {
            TypeDeclaration td = wezel as TypeDeclaration;
            var wynik = new Obiekt();

            wynik.Rodzaj = MapujRodzajObiektu(td.ClassType);
            wynik.Nazwa = td.Name;

            foreach (var dziecko in wezel.Children)
            {
                if (DefinicjaAtrybutow(dziecko))
                    wynik.Atrybuty.AddRange(ParsujAtrybuty(dziecko));

                if (DefinicjaPola(dziecko))
                {
                    wynik.Pola.Add(ParsujPole(dziecko));
                    continue;
                }
                if (DefinicjaPropertiesa(dziecko))
                {
                    wynik.Propertiesy.Add(ParsujPropertiesa(dziecko));
                }

                if (DefinicjaKonstruktora(dziecko))
                {
                    var konstruktor = ParsujKonstruktor(dziecko);
                    konstruktor.Wlasciciel = wynik;
                    wynik.Konstruktory.Add(konstruktor);
                    continue;
                }

                if (DefinicjaMetody(dziecko))
                {
                    var metoda = ParsujMetode(dziecko);
                    metoda.Wlasciciel = wynik;
                    wynik.Metody.Add(metoda);
                    continue;
                }

                if (dziecko is CSharpTokenNode)
                {
                    ParsujKlamerki(wynik, dziecko);
                }

                if (dziecko is SimpleType)
                {
                    var dziedziczony = new ObiektDziedziczony();
                    dziedziczony.Nazwa = (dziecko as SimpleType).Identifier;
                    ParsujParametryGeneryczne(dziecko as SimpleType, dziedziczony);
                    UstawPolozenie(dziedziczony, dziecko);
                    wynik.NadklasaIInterfejsy.Add(dziedziczony);
                }

                if (WezelKlasa(dziecko))
                {
                    var wewnetrznyObiekt = ParsujKlase(dziecko);
                    wewnetrznyObiekt.Wlasciciel = wynik;
                    wynik.ObiektyWewnetrzne.Add(wewnetrznyObiekt);
                }
            }
            UstawPolozenie(wynik, wezel);
            return wynik;
        }

        private void ParsujParametryGeneryczne(
            SimpleType wezel,
            ObiektDziedziczony dziedziczony)
        {
            foreach (var dziecko in wezel.TypeArguments)
            {
                var typ = SzukajNazwyTypu(dziecko);
                if (typ != null)
                    dziedziczony.NazwyTypowParametrow.Add(typ);

            }
        }

        private string SzukajNazwyTypu(AstType dziecko)
        {
            var wezel = dziecko as SimpleType;
            if (wezel != null)
                return wezel.Identifier;

            return null;
        }

        private bool DefinicjaAtrybutow(AstNode dziecko)
        {
            return dziecko is AttributeSection;
        }

        private IEnumerable<Atrybut> ParsujAtrybuty(AstNode wezel)
        {
            var atrybut = wezel as AttributeSection;
            foreach (var dziecko in wezel.Children)
            {
                if (dziecko is ICSharpCode.NRefactory.CSharp.Attribute)
                {
                    yield return ParsujAtrybut(dziecko as ICSharpCode.NRefactory.CSharp.Attribute);
                }
            }
        }

        private Atrybut ParsujAtrybut(
            ICSharpCode.NRefactory.CSharp.Attribute wezelAtrybutow)
        {
            var wynik = new Atrybut();
            wynik.Nazwa = wezelAtrybutow.FirstChild.ToString();
            UstawPolozenie(wynik, wezelAtrybutow);

            foreach (var dziecko in wezelAtrybutow.Children)
            {
                if (dziecko is NamedExpression)
                {
                    var ne = dziecko as NamedExpression;
                    var parametr = new ParametrAtrybutu();
                    parametr.Nazwa = ne.Name;
                    if (ne.Expression is PrimitiveExpression)
                    {
                        parametr.Wartosc =
                            (ne.Expression as PrimitiveExpression).Value.ToString();
                    }
                    else
                    {
                        parametr.Wartosc = ne.Expression.ToString();
                    }
                    wynik.Parametry.Add(parametr);
                }
                if (dziecko is PrimitiveExpression)
                {
                    var pe = dziecko as PrimitiveExpression;
                    var parametr = new ParametrAtrybutu();
                    parametr.Nazwa = string.Empty;
                    parametr.Wartosc = pe.Value.ToString();
                    wynik.Parametry.Add(parametr);
                }
                if (dziecko is TypeOfExpression)
                {
                    var tofe = dziecko as TypeOfExpression;
                    var parametr = new ParametrAtrybutu();
                    parametr.Wartosc = tofe.ToString();
                    wynik.Parametry.Add(parametr);
                }
            }

            return wynik;
        }

        private string ParsujWartosc(AstNode wezel)
        {
            if (wezel is NamedExpression)
            {
                var ne = wezel as NamedExpression;
                if (ne.Expression is PrimitiveExpression)
                {
                    return (ne.Expression as PrimitiveExpression).Value.ToString();
                }
                else
                {
                    return ne.Expression.ToString();
                }
            }

            if (wezel is PrimitiveExpression)
            {
                var pe = wezel as PrimitiveExpression;
                return pe.LiteralValue;
            }
            return string.Empty;
        }

        private void ParsujKlamerki(
            IZPoczatkowaIKoncowaKlamerka wynik,
            AstNode wezel)
        {
            var tn = wezel as CSharpTokenNode;
            var tekst = tn.GetText();
            if (tekst == "{")
            {
                wynik.PoczatkowaKlamerka.Wiersz = tn.StartLocation.Line;
                wynik.PoczatkowaKlamerka.Kolumna = tn.StartLocation.Column;
            }
            if (tekst == "}")
            {
                wynik.KoncowaKlamerka.Wiersz = tn.EndLocation.Line;
                wynik.KoncowaKlamerka.Kolumna = tn.EndLocation.Column;
            }
        }

        private bool DefinicjaMetody(AstNode wezel)
        {
            MethodDeclaration md = wezel as MethodDeclaration;
            return md != null;
        }

        private Metoda ParsujMetode(AstNode wezel)
        {
            MethodDeclaration md = wezel as MethodDeclaration;
            var wynik = new Metoda();

            wynik.Nazwa = md.Name;
            wynik.TypZwracany = SzukajTypuPola(md.ReturnType);

            bool bylNawiasOtwierajacy = false;
            bool bylNawiasZammykajacy = false;
            foreach (var dziecko in wezel.Children)
            {
                var modyfikator = SzukajModyfikatora(dziecko);
                if (modyfikator != null)
                    wynik.Modyfikatory.Add(modyfikator);

                if (DefinicjaAtrybutow(dziecko))
                    wynik.Atrybuty.AddRange(ParsujAtrybuty(dziecko));

                if (dziecko is CSharpTokenNode)
                {
                    ParsujNawiasy(
                        wynik,
                        dziecko as CSharpTokenNode,
                        ref bylNawiasOtwierajacy,
                        ref bylNawiasZammykajacy);
                }

            }

            foreach (var param in md.Parameters)
                wynik.Parametry.Add(ParsujParametr(param));

            UstawPolozenie(wynik, wezel);
            return wynik;
        }

        private void ParsujNawiasy(
            IZNawiasamiOtwierajacymiZamykajacymiParametry wynik,
            CSharpTokenNode wezel,
            ref bool bylNawiasOtwierajacy,
            ref bool bylNawiasZamykajacy)
        {
            if (bylNawiasOtwierajacy && bylNawiasZamykajacy)
                return;
            var tekst = wezel.GetText();
            if (tekst == "(")
            {
                wynik.NawiasOtwierajacyParametry.Wiersz = wezel.StartLocation.Line;
                wynik.NawiasOtwierajacyParametry.Kolumna = wezel.StartLocation.Column;
            }
            if (tekst == ")")
            {
                wynik.NawiasZamykajacyParametry.Wiersz = wezel.StartLocation.Line;
                wynik.NawiasZamykajacyParametry.Kolumna = wezel.StartLocation.Column;
            }
        }

        private bool DefinicjaKonstruktora(AstNode wezel)
        {
            ConstructorDeclaration cd = wezel as ConstructorDeclaration;
            return cd != null;
        }

        private Konstruktor ParsujKonstruktor(AstNode wezel)
        {
            ConstructorDeclaration cd = wezel as ConstructorDeclaration;
            var wynik = new Konstruktor();
            UstawPolozenie(wynik, wezel);

            bool bylNawiasOtwierajacy = false;
            bool bylNawiasZammykajacy = false;
            foreach (var dziecko in wezel.Children)
            {
                if (dziecko is CSharpModifierToken)
                    continue;
                if (dziecko is BlockStatement)
                {
                    wynik.PoczatkowaKlamerka = DajPozycje(dziecko.StartLocation);
                    wynik.KoncowaKlamerka = DajPozycje(dziecko.EndLocation);
                    wynik.KoncowaKlamerka.Kolumna -= 1;
                }

                if (dziecko is CSharpTokenNode)
                {
                    ParsujNawiasy(
                        wynik,
                        dziecko as CSharpTokenNode,
                        ref bylNawiasOtwierajacy,
                        ref bylNawiasZammykajacy);
                }

                if (dziecko is ConstructorInitializer)
                {
                    var ci = dziecko as ConstructorInitializer;
                    wynik.ParametryKonstruktoraZNadKlasy = new List<string>();
                    foreach (IdentifierExpression argument in ci.Arguments)
                    {
                        wynik.ParametryKonstruktoraZNadKlasy.Add(argument.Identifier);
                    }
                    wynik.SlowoKluczoweInicjalizacji = ci.Keyword.ToString();
                }
            }

            foreach (var param in cd.Parameters)
            {
                wynik.Parametry.Add(ParsujParametr(param));
            }

            wynik.Modyfikator = cd.Modifiers.ToString().ToLower();
            return wynik;
        }

        private Parametr ParsujParametr(ParameterDeclaration param)
        {
            var wynik = new Parametr();
            wynik.NazwaParametru = param.Name;
            wynik.NazwaTypu = SzukajTypuPola(param.Type);
            ObsluzModyfikator(param, wynik);

            if (param.DefaultExpression != null)
                wynik.WartoscDomyslna = ParsujWartosc(param.DefaultExpression);
            return wynik;
        }

        private void ObsluzModyfikator(
            ParameterDeclaration param,
            Parametr wynik)
        {
            if (param.ParameterModifier == ParameterModifier.This)
                wynik.ZThisem = true;
            if (param.ParameterModifier == ParameterModifier.Out)
                wynik.ZOut = true;
            if (param.ParameterModifier == ParameterModifier.Params)
                wynik.ZParams = true;
            if (param.ParameterModifier == ParameterModifier.Ref)
                wynik.ZRef = true;
        }

        private bool DefinicjaPropertiesa(AstNode wezel)
        {
            PropertyDeclaration pd = wezel as PropertyDeclaration;
            return pd != null;
        }

        private Property ParsujPropertiesa(AstNode wezel)
        {
            PropertyDeclaration pd = wezel as PropertyDeclaration;
            var wynik = new Property();
            wynik.Nazwa = pd.Name;
            wynik.NazwaTypu = SzukajTypuPola(pd.ReturnType);

            if (pd.Setter.Keyword.ToString().Contains("set"))
                wynik.JestSet = true;
            if (pd.Getter.Keyword.ToString().Contains("get"))
                wynik.JestGet = true;

            foreach (var dziecko in pd.Children)
            {
                var modyfikator = SzukajModyfikatora(dziecko);
                if (modyfikator != null)
                    wynik.Modyfikatory.Add(modyfikator);
                if (DefinicjaAtrybutow(dziecko))
                    wynik.Atrybuty.AddRange(ParsujAtrybuty(dziecko));
            }
            UstawPolozenie(wynik, wezel);
            return wynik;
        }

        private bool DefinicjaPola(AstNode wezel)
        {
            FieldDeclaration fd = wezel as FieldDeclaration;
            return fd != null;
        }

        private Pole ParsujPole(AstNode wezel)
        {
            FieldDeclaration fd = wezel as FieldDeclaration;
            var pole = new Pole();
            foreach (var dziecko in wezel.Children)
            {
                if (SzukajNazwyPola(dziecko, pole))
                    continue;
                if (SzukajTypuPola(dziecko, pole))
                    continue;
                if (SzukajModyfikatora(dziecko, pole))
                    continue;
            }
            UstawPolozenie(pole, wezel);
            return pole;
        }

        private bool SzukajModyfikatora(AstNode wezel, Pole pole)
        {
            CSharpModifierToken csmt = wezel as CSharpModifierToken;
            if (csmt != null)
            {
                pole.Modyfikatory.Add(MapujModyfikator(csmt.Modifier));
                return true;
            }
            return false;
        }

        private Modyfikator SzukajModyfikatora(AstNode wezel)
        {
            CSharpModifierToken csmt = wezel as CSharpModifierToken;
            if (csmt != null)
            {
                var wynik = MapujModyfikator(csmt.Modifier);
                UstawPolozenie(wynik, wezel);
                return wynik;
            }
            return null;
        }

        private Modyfikator MapujModyfikator(Modifiers modifiers)
        {
            return new Modyfikator(modifiers.ToString().ToLower());
        }

        private bool SzukajTypuPola(AstNode wezel, Pole pole)
        {
            var nazwaTypu = SzukajTypuPola(wezel);
            if (nazwaTypu != null)
            {
                pole.NazwaTypu = nazwaTypu;
                return true;
            }
            return false;
        }

        private string SzukajTypuPola(AstNode wezel)
        {
            PrimitiveType pt = wezel as PrimitiveType;
            if (pt != null)
                return pt.Keyword;
            SimpleType st = wezel as SimpleType;
            if (st != null)
            {
                var dodatek = string.Empty;
                if (st.TypeArguments.Count > 0)
                {
                    dodatek = "<" + string.Join(", ", DajNazwyParametrowTypu(st)) + ">";
                }
                return st.Identifier + dodatek;
            }
            ComposedType ct = wezel as ComposedType;
            if (ct != null)
            {
                return ct.ToString();
            }
            MemberType mt = wezel as MemberType;
            if (mt != null)
                return mt.ToString();
            return null;
        }

        private IEnumerable<string> DajNazwyParametrowTypu(SimpleType st)
        {
            return st.TypeArguments.Select(o => SzukajTypuPola(o));
        }

        private bool SzukajNazwyPola(AstNode wezel, Pole pole)
        {
            VariableInitializer vi = wezel as VariableInitializer;
            if (vi != null)
            {
                pole.Nazwa = vi.Name;
                return true;
            }
            return false;
        }

        private RodzajObiektu MapujRodzajObiektu(ClassType classType)
        {
            switch (classType)
            {
                case ClassType.Class:
                    return RodzajObiektu.Klasa;
                case ClassType.Struct:
                    return RodzajObiektu.Struct;
                case ClassType.Interface:
                    return RodzajObiektu.Interfejs;
                case ClassType.Enum:
                    return RodzajObiektu.Enum;
                default:
                    throw new Exception("Niepoprawny typ obiektu");
            }
        }

        private void UstawPolozenie(
            ParsowanaJednostka obiekt, AstNode wezel)
        {
            obiekt.Poczatek = new PozycjaWPliku(
                wezel.StartLocation.Line,
                wezel.StartLocation.Column);
            obiekt.Koniec = new PozycjaWPliku(
                wezel.EndLocation.Line,
                wezel.EndLocation.Column);
        }

        private PozycjaWPliku DajPozycje(TextLocation textLocation)
        {
            return new PozycjaWPliku(textLocation.Line, textLocation.Column);
        }
    }
}
