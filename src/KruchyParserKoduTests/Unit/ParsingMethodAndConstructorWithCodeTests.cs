using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
using KruchyParserKodu.ParserKodu.Models.Instructions;
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

        IList<Instruction> _contructorInstructions;

        Konstruktor _constructor;

        Instruction _constructorInstruction1;

        Instruction _constructorInstruction2;

        Metoda _method;

        IList<Instruction> _methodInstructions;

        Instruction _methodInstruction1;

        Instruction _methodInstruction2;

        [SetUp]
        public void SetUpEachTest()
        {
            parsedCode = Parser.Parsuj(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("MethodAndConstructorWithCode.cs"));

            _constructor = parsedCode.DefiniowaneObiekty.Single().Konstruktory.Single();

            _contructorInstructions = _constructor.Instructions;

            if (_contructorInstructions != null)
            {
                _constructorInstruction1 = _contructorInstructions.FirstOrDefault();

                _constructorInstruction2 = _contructorInstructions.Skip(1).FirstOrDefault();
            }

            _method = parsedCode.DefiniowaneObiekty.Single().Metody.Single();

            _methodInstructions = _method.Instructions;

            if ( _methodInstructions != null )
            {
                _methodInstruction1 = _methodInstructions.FirstOrDefault();

                _methodInstruction2 = _methodInstructions.Skip(1).FirstOrDefault();
            }
        }

        [Test]
        public void ParsedCode_Constructor_ShouldHaveInstructions()
        {
            _contructorInstructions.Should().HaveCount(2);
        }

        [Test]
        public void ParsedCode_Constructor_ShouldInstructionsSetPosition()
        {
            _constructorInstruction1.StartPosition.Wiersz.Should().Be(10);

            _constructorInstruction1.StartPosition.Kolumna.Should().Be(13);

            _constructorInstruction1.EndPosition.Wiersz.Should().Be(10);

            _constructorInstruction1.EndPosition.Kolumna.Should().Be(22);

            _constructorInstruction2.StartPosition.Wiersz.Should().Be(12);

            _constructorInstruction2.StartPosition.Kolumna.Should().Be(13);

            _constructorInstruction2.EndPosition.Wiersz.Should().Be(12);

            _constructorInstruction2.EndPosition.Kolumna.Should().Be(28);
        }

        [Test]
        public void ParsedCode_Constructor_ShouldInstructionsHaveSetCodeUnit()
        {
            _constructorInstruction1.CodeUnit.Should().Be(_constructor);

            _constructorInstruction2.CodeUnit.Should().Be(_constructor);
        }

        [Test]
        public void ParsedCode_Constructor_ShouldInstruction1BeOfTypeAssignmentInstruction()
        {
            _constructorInstruction1.GetType().Should().Be(typeof(AssignmentInstruction));
        }

        [Test]
        public void ParsedCode_Method_ShouldHaveInstructions()
        {
            _methodInstructions.Should().HaveCount(2);
        }

        [Test]
        public void ParsedCode_Method_ShouldInstructionsSetPosition()
        {
            _methodInstruction1.StartPosition.Wiersz.Should().Be(17);

            _methodInstruction1.StartPosition.Kolumna.Should().Be(13);

            _methodInstruction1.EndPosition.Wiersz.Should().Be(17);

            _methodInstruction1.EndPosition.Kolumna.Should().Be(26);

            _methodInstruction2.StartPosition.Wiersz.Should().Be(19);

            _methodInstruction2.StartPosition.Kolumna.Should().Be(13);

            _methodInstruction2.EndPosition.Wiersz.Should().Be(22);

            _methodInstruction2.EndPosition.Kolumna.Should().Be(14);
        }

        [Test]
        public void ParsedCode_Method_ShouldInstructionsHaveSetCodeUnit()
        {
            _methodInstruction1.CodeUnit.Should().Be(_method);

            _methodInstruction2.CodeUnit.Should().Be(_method);
        }
    }
}
