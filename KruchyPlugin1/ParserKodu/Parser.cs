using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.CSharp;

namespace KruchyCompany.KruchyPlugin1.ParserKodu
{
    interface IParser
    {

    }

    class Parser
    {
        public static Plik Parsuj(string zawartosc)
        {
            var drzewo = new CSharpParser().Parse(zawartosc);
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


        private static bool WezelUsing(AstNode wezel)
        {
            UsingDeclaration ud = wezel as UsingDeclaration;
            return ud != null;
        }

        private static string ParsujUsing(AstNode wezel)
        {
            UsingDeclaration ud = wezel as UsingDeclaration;
            return ud.Namespace;
        }

        private static bool WezelNamespace(AstNode wezel)
        {
            NamespaceDeclaration nd = wezel as NamespaceDeclaration;
            return nd != null;
        }

        private static void ParsujNamespace(AstNode wezel, Plik wynik)
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

        private static bool WezelKlasa(AstNode wezel)
        {
            TypeDeclaration td = wezel as TypeDeclaration;
            return td != null;
        }

        private static Obiekt ParsujKlase(AstNode wezel)
        {
            TypeDeclaration td = wezel as TypeDeclaration;
            var wynik = new Obiekt();

            wynik.Rodzaj = MapujRodzajObiektu(td.ClassType);
            wynik.Nazwa = td.Name;

            foreach (var dziecko in wezel.Children)
            {
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
                    wynik.Konstruktory.Add(ParsujKonstruktor(dziecko));
                    continue;
                }

                if (DefinicjaMetody(dziecko))
                {
                    wynik.Metody.Add(ParsujMetode(dziecko));
                    continue;
                }
            }
            UstawPolozenie(wynik, wezel);
            return wynik;
        }

        private static bool DefinicjaMetody(AstNode wezel)
        {
            MethodDeclaration md = wezel as MethodDeclaration;
            return md != null;
        }

        private static Metoda ParsujMetode(AstNode wezel)
        {
            MethodDeclaration md = wezel as MethodDeclaration;
            var wynik = new Metoda();

            wynik.Nazwa = md.Name;
            wynik.TypZwracany = SzukajTypuPola(md.ReturnType);

            foreach (var dziecko in wezel.Children)
            {
                var modyfikator = SzukajModyfikatora(dziecko);
                if (modyfikator != null)
                    wynik.Modyfikatory.Add(modyfikator);
            }

            foreach (var param in md.Parameters)
                wynik.Parametry.Add(ParsujParametr(param));

            UstawPolozenie(wynik, wezel);
            return wynik;
        }

        private static bool DefinicjaKonstruktora(AstNode wezel)
        {
            ConstructorDeclaration cd = wezel as ConstructorDeclaration;
            return cd != null;
        }

        private static Konstruktor ParsujKonstruktor(AstNode wezel)
        {
            ConstructorDeclaration cd = wezel as ConstructorDeclaration;
            var wynik = new Konstruktor();
            UstawPolozenie(wynik, wezel);

            foreach (var param in cd.Parameters)
            {
                wynik.Parametry.Add(ParsujParametr(param));
            }

            wynik.Modyfikator = cd.Modifiers.ToString().ToLower();
            return wynik;
        }

        private static Parametr ParsujParametr(ParameterDeclaration param)
        {
            var wynik = new Parametr();
            wynik.NazwaParametru = param.Name;
            wynik.NazwaTypu = SzukajTypuPola(param.Type);
            return wynik;
        }

        private static bool DefinicjaPropertiesa(AstNode wezel)
        {
            PropertyDeclaration pd = wezel as PropertyDeclaration;
            return pd != null;
        }

        private static Property ParsujPropertiesa(AstNode wezel)
        {
            PropertyDeclaration pd = wezel as PropertyDeclaration;
            var wynik = new Property();
            wynik.Nazwa = pd.Name;
            wynik.NazwaTypu = SzukajTypuPola(pd.ReturnType);

            foreach (var dziecko in pd.Children)
            {
                var modyfikator = SzukajModyfikatora(dziecko);
                if (modyfikator != null)
                    wynik.Modyfikatory.Add(modyfikator);
            }
            UstawPolozenie(wynik, wezel);
            return wynik;
        }

        private static bool DefinicjaPola(AstNode wezel)
        {
            FieldDeclaration fd = wezel as FieldDeclaration;
            return fd != null;
        }

        private static Pole ParsujPole(AstNode wezel)
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

        private static bool SzukajModyfikatora(AstNode wezel, Pole pole)
        {
            CSharpModifierToken csmt = wezel as CSharpModifierToken;
            if (csmt != null)
            {
                pole.Modyfikatory.Add(MapujModyfikator(csmt.Modifier));
                return true;
            }
            return false;
        }

        private static string SzukajModyfikatora(AstNode wezel)
        {
            CSharpModifierToken csmt = wezel as CSharpModifierToken;
            if (csmt != null)
            {
                return MapujModyfikator(csmt.Modifier);
            }
            return null;
        }

        private static string MapujModyfikator(Modifiers modifiers)
        {
            return modifiers.ToString().ToLower();
        }

        private static bool SzukajTypuPola(AstNode wezel, Pole pole)
        {
            var nazwaTypu = SzukajTypuPola(wezel);
            if (nazwaTypu != null)
            {
                pole.NazwaTypu = nazwaTypu;
                return true;
            }
            return false;
        }

        private static string SzukajTypuPola(AstNode wezel)
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
                return null;
        }

        private static IEnumerable<string> DajNazwyParametrowTypu(SimpleType st)
        {
            return st.TypeArguments.Select(o => SzukajTypuPola(o));
        }

        private static bool SzukajNazwyPola(AstNode wezel, Pole pole)
        {
            VariableInitializer vi = wezel as VariableInitializer;
            if (vi != null)
            {
                pole.Nazwa = vi.Name;
                return true;
            }
            return false;
        }

        private static RodzajObiektu MapujRodzajObiektu(ClassType classType)
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

        private static void UstawPolozenie(
            ParsowanaJednostka obiekt, AstNode wezel)
        {
            obiekt.Poczatek = new PozycjaWPliku(
                wezel.StartLocation.Line,
                wezel.StartLocation.Column);
            obiekt.Koniec = new PozycjaWPliku(
                wezel.EndLocation.Line,
                wezel.EndLocation.Column);
        }
    }
}
