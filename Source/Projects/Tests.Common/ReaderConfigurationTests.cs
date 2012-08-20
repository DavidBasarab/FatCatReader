using Common;
using FluentAssertions;
using Microsoft.Win32;
using NUnit.Framework;

namespace Tests.Common
{
    [TestFixture]
    [Category("Common")]
    public class ReaderConfigurationTests
    {
        [TearDown]
        public void TearDown()
        {
            //Registry.CurrentUser.DeleteSubKeyTree(@"Software\FatCatReader");
        }

        [Test]
        public void WillSetUsernameInConfiguration()
        {
            const string testUserName = "TEST_USERNAME";

            ReaderConfiguration.Username = testUserName;

            var subKey = Registry.CurrentUser.OpenSubKey("FatCatReader");

            subKey.Should().NotBeNull();

            var result = Registry.CurrentUser.GetValue("Username");

            result.Should().Be(testUserName);
        }
    }
}