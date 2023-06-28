using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKoduTests.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsingMethodAndConstructorWithCodeTests
    {
        Plik parsedCode;

        IList<Instruction> _instructions;

        Konstruktor _constructor;

        Instruction _instruction1;

        Instruction _instruction2;

        [SetUp]
        public void SetUpEachTest()
        {
            parsedCode = Parser.Parsuj(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("MethodAndConstructorWithCode.cs"));

            _constructor = parsedCode.DefiniowaneObiekty.Single().Konstruktory.Single();

            _instructions = _constructor.Instructions;

            if (_instructions != null)
            {
                _instruction1 = _instructions.FirstOrDefault();

                _instruction2 = _instructions.Skip(1).FirstOrDefault();
            }
        }

        [Test]
        public void ParsedCode_Constructor_ShouldHaveInstructions()
        {
            _instructions.Should().HaveCount(2);
        }

        [Test]
        public void ParsedCode_Constructor_ShouldInstructionsSetPosition()
        {
            _instruction1.StartPosition.Wiersz.Should().Be(10);

            _instruction1.StartPosition.Kolumna.Should().Be(13);

            _instruction1.EndPosition.Wiersz.Should().Be(10);

            _instruction1.EndPosition.Kolumna.Should().Be(22);

            _instruction2.StartPosition.Wiersz.Should().Be(12);

            _instruction2.StartPosition.Kolumna.Should().Be(13);

            _instruction2.EndPosition.Wiersz.Should().Be(12);

            _instruction2.EndPosition.Kolumna.Should().Be(28);
        }

        [Test]
        public void ParsedCode_Constructor_ShouldInstructionsHaveSetCodeUnit()
        {
            _instruction1.CodeUnit.Should().Be(_constructor);

            _instruction2.CodeUnit.Should().Be(_constructor);
        }
    }
}
