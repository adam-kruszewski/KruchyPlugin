using FluentAssertions;
using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.Tests.Utils;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;
using Moq;
using NUnit.Framework;

namespace Kruchy.Plugin.Akcje.Tests.Unit.FillingConstructor
{
    [TestFixture]
    public class FillingConstructorWithOtherInstructionsTests
    {
        [Test]
        public void Uzupelnij_ConstructorWithoutAssignedReadOnlyFields_AddsConstructorWithOtherInstructions()
        {
            //arrange
            var solution = new SolutionWrapper(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("FillingConstructor.ConstructorWithOtherInstructionsNewConstructor.cs"));

            UnitModuleInitialization.SetSolutionToKonfiguracjaMock(solution);

            UnitModuleInitialization.KonfiguracjaMock
                .Setup(o => o.SortowacZaleznosciSerwisu())
                .Returns(false);

            //act
            new UzupelnianieKonstruktora(solution).Uzupelnij();

            //assert
            var expectedResult = new WczytywaczZawartosciPrzykladow()
                .DajZawartoscPrzykladu("FillingConstructor.ConstructorWithOtherInstructionsNewConstructorResult.cs");

            var actualResult = solution.CurenctDocument.GetContent();

            actualResult.Should().Be(expectedResult);
        }

        [Test]
        public void Uzupelnij_ConstructorWithoutAssignedNotAllReadOnlyFields_AddsConstructorWithOtherInstructionsAndLackingAsignments()
        {
            //arrange
            var solution = new SolutionWrapper(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("FillingConstructor.ConstructorWithOtherInstructionAddLackingAssignments.cs"));

            UnitModuleInitialization.KonfiguracjaMock
                .Setup(o => o.SortowacZaleznosciSerwisu())
                .Returns(false);

            UnitModuleInitialization.SetSolutionToKonfiguracjaMock(solution);

            //act
            new UzupelnianieKonstruktora(solution).Uzupelnij();

            //assert
            var expectedResult = new WczytywaczZawartosciPrzykladow()
                .DajZawartoscPrzykladu("FillingConstructor.ConstructorWithOtherInstructionAddLackingAssignmentsResult.cs");

            var actualResult = solution.CurenctDocument.GetContent();

            actualResult.Should().Be(expectedResult);
        }
    }
}
