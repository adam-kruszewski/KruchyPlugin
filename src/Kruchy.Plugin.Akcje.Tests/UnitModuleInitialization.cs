using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;
using Kruchy.Plugin.Utils.UI;
using Moq;
using NUnit.Framework;

namespace Kruchy.Plugin.Akcje.Tests
{
    [SetUpFixture]
    public class UnitModuleInitialization
    {
        public static Mock<IUIFactory> UiFactoryMock { get; private set; }

        public static Mock<Konfiguracja> KonfiguracjaMock { get; private set; }

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            UiFactoryMock = new Mock<IUIFactory>();

            UIObjects.FactoryInstance = UiFactoryMock.Object;

            KonfiguracjaMock = new Mock<Konfiguracja>();

            Konfiguracja.SetInstance(KonfiguracjaMock.Object);
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            UIObjects.FactoryInstance = null;
            Konfiguracja.SetInstance(null);
        }

        public static void SetSolutionToKonfiguracjaMock(
            SolutionWrapper solutionWrapper)
        {
            KonfiguracjaMock.Reset();

            KonfiguracjaMock
                .Setup(o => o.Solution)
                .Returns(solutionWrapper);
        }
    }
}
