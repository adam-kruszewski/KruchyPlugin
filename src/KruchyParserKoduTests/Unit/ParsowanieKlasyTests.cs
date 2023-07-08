using System.IO;
using System.Linq;
using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
using KruchyParserKoduTests.Utils;
using NUnit.Framework;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsowanieKlasyTests
    {
        [Test]
        public void ParsujeKlase()
        {
            //arrange
            var zawartosc =
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("KlasaDoParsowania.cs");
            //act
            var plik = Parser.Parsuj(zawartosc);

            //assert
            plik.Should().NotBeNull();
            plik.Namespace.Should().Be("KruchyCompany.KruchyPlugin1Tests.ParserTests");
            plik.Usings.Count.Should().Be(6);
            plik.Usings[5].StartPosition.Sprawdz(6, 1);
            plik.Usings[5].EndPosition.Sprawdz(6, 35);

            var obiekt = plik.DefinedItems.First();
            obiekt.KindOfItem.Should().Be(KindOfItem.Class);
            obiekt.Name.Should().Be("KlasaDoParsowania");
            obiekt.StartingBrace.Row.Should().Be(12);
            obiekt.ClosingBrace.Column.Should().Be(5);
            obiekt.Attributes.Count().Should().Be(1);
            obiekt.Attributes.First().Name.Should().Be("Testowo");
            obiekt.Modifiers.Should().BeEmpty();

            SprawdzPola(obiekt);

            SprawdzPropertiesy(obiekt);

            SprawdzKonstruktory(obiekt);

            SprawdzMetody(obiekt);
        }

        private static void SprawdzPola(DefinedItem obiekt)
        {
            obiekt.Fields.Count().Should().Be(2);
            var pole = obiekt.Fields.First();
            pole.Name.Should().Be("PoleReadOnly");
            pole.TypeName.Should().Be("IParser");
            pole.Modifiers[0].Name.Should().Be("private");
            pole.Modifiers[1].Name.Should().Be("readonly");
            pole.StartPosition.Sprawdz(13, 9);
            pole.EndPosition.Sprawdz(13, 47);

            var poleStringReadonly = obiekt.Fields[1];
            poleStringReadonly.Name.Should().Be("PoleStringReadOnly");
            poleStringReadonly.TypeName.Should().Be("IList<string>");
            poleStringReadonly.Modifiers[0].Name.Should().Be("public");
            poleStringReadonly.Modifiers[1].Name.Should().Be("readonly");
        }

        private static void SprawdzPropertiesy(DefinedItem obiekt)
        {
            obiekt.Properties.Count().Should().Be(2);
            var wlasciwosc = obiekt.Properties.First();
            wlasciwosc.Name.Should().Be("Wlasciwosc");
            wlasciwosc.TypeName.Should().Be("int");
            wlasciwosc.Modifiers[0].Name.Should().Be("public");
            wlasciwosc.HasGet.Should().BeTrue();
            wlasciwosc.HasSet.Should().BeTrue();

            var wlasciwosc2 = obiekt.Properties[1];
            wlasciwosc2.HasGet.Should().BeTrue();
            wlasciwosc2.HasSet.Should().BeFalse();
        }

        private void SprawdzKonstruktory(DefinedItem obiekt)
        {
            obiekt.Constructors.Count().Should().Be(2);

            var bezparametrowyKonstruktor = obiekt.Constructors[1];
            bezparametrowyKonstruktor.Parametry.Count().Should().Be(0);
            SprawdzPozycje(bezparametrowyKonstruktor.StartPosition, 26, 9);
            SprawdzPozycje(bezparametrowyKonstruktor.EndPosition, 29, 10);

            var konstrZ1Parametrem = obiekt.Constructors.First();
            konstrZ1Parametrem.Parametry.Count().Should().Be(1);
            var parametr = konstrZ1Parametrem.Parametry.First();
            parametr.ParameterName.Should().Be("a");
            parametr.TypeName.Should().Be("int");
            SprawdzPozycje(konstrZ1Parametrem.StartPosition, 19, 9);
            SprawdzPozycje(konstrZ1Parametrem.EndPosition, 24, 10);
            SprawdzPozycje(
                konstrZ1Parametrem.StartingParameterBrace,
                19, 33);
            SprawdzPozycje(
                konstrZ1Parametrem.ClosingParameterBrace,
                19, 39);
            SprawdzPozycje(
                konstrZ1Parametrem.StartingBrace,
                20, 9);
            SprawdzPozycje(
                konstrZ1Parametrem.ClosingBrace,
                24, 9);
            konstrZ1Parametrem.Modifier.Should().Be("public");
        }

        private void SprawdzMetody(DefinedItem obiekt)
        {
            obiekt.Methods.Count().Should().Be(2);

            var metodaStatyczna = obiekt.Methods[0];
            metodaStatyczna.Name.Should().Be("MetodaStatyczna");
            metodaStatyczna.Parametry.Count().Should().Be(3);
            SprawdzModyfikatorMetodyStatycznej(metodaStatyczna);
            var p = metodaStatyczna.Parametry.First();
            p.TypeName.Should().Be("string");
            p.ParameterName.Should().Be("b");
            metodaStatyczna.Parametry[1].TypeName.Should().Be("int?");
            metodaStatyczna.Parametry[2].TypeName.Should().Be("DateTime?");
            metodaStatyczna.Modyfikatory[0].Name.Should().Be("private");
            metodaStatyczna.Modyfikatory[1].Name.Should().Be("static");
            metodaStatyczna.ReturnType.Nazwa.Should().Be("void");
            metodaStatyczna.StartingParameterBrace.Row.Should().Be(31);
            metodaStatyczna.StartingParameterBrace.Column.Should().Be(44);
            metodaStatyczna.ClosingParameterBrace.Row.Should().Be(31);
            metodaStatyczna.ClosingParameterBrace.Column.Should().Be(74);

            var metodaZwykla = obiekt.Methods[1];
            metodaZwykla.Modyfikatory.First().Name.Should().Be("private");
            metodaZwykla.Name.Should().Be("MetodaZwykla");
            metodaZwykla.ReturnType.Nazwa.Should().Be("int");
            metodaZwykla.Parametry.Count().Should().Be(2);
            metodaZwykla.Parametry[0].TypeName.Should().Be("System.DateTime");
            metodaZwykla.Parametry[1].TypeName.Should().Be("System.DateTime?");
            metodaZwykla.Atrybuty.Count().Should().Be(1);
            metodaZwykla.Atrybuty.First().Name.Should().Be("Testowo");
        }

        private void SprawdzModyfikatorMetodyStatycznej(Method metodaStatyczna)
        {
            metodaStatyczna.Modyfikatory.Count().Should().Be(2);
            var modyfikatorPublic = metodaStatyczna.Modyfikatory[0];
            modyfikatorPublic.Name.Should().Be("private");
            SprawdzPozycje(modyfikatorPublic.StartPosition, 31, 9);
            SprawdzPozycje(modyfikatorPublic.EndPosition, 31, 16);
            var modyfikatorStatic = metodaStatyczna.Modyfikatory[1];
            modyfikatorStatic.Name.Should().Be("static");
        }

        private void SprawdzPozycje(PlaceInFile pozycja, int wiersz, int kolumna)
        {
            pozycja.Row.Should().Be(wiersz);
            pozycja.Column.Should().Be(kolumna);
        }
    }
}
