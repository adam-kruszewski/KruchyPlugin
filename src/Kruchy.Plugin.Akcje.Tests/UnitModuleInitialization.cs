using Kruchy.Plugin.Utils.UI;
using Moq;
using NUnit.Framework;

namespace Kruchy.Plugin.Akcje.Tests
{
    [SetUpFixture]
    public class UnitModuleInitialization
    {
        public static Mock<IUIFactory> UiFactoryMock { get; private set; }

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            UiFactoryMock = new Mock<IUIFactory>();

            UIObjects.FactoryInstance = UiFactoryMock.Object;
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            UIObjects.FactoryInstance = null;
        }
    }
}
