using Common;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Storage;

namespace Tests.Common
{
    [TestClass]
    public class ConfiguraitonTests
    {
        [TestMethod]
        public void UserNameIsStoredInLocal()
        {
            var configuration = new Configuration
            {
                Username = "TEST_USER"
            };

            var localSettings = ApplicationData.Current.LocalSettings;

            var container = localSettings.Containers["FatCatReader"];

            Assert.IsNotNull(container);

            container.Values["Username"] = "TEST_USER";
        }
    }
}