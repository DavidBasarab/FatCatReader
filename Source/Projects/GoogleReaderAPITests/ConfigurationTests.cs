using FluentAssertions;
using GoogleReaderAPI;
using NUnit.Framework;
using Rhino.Mocks;

namespace GoogleReaderAPITests
{
    [TestFixture]
    [Category("GoogleReaderAPITests")]
    public class ConfigurationTests
    {
        private const string TestUsername = "TEST_USER";
        private const string TestPassword = "SOME_PASSWORD";
        private DataRepository _dataRepository;
        private Configuration _configuration;
        private MockRepository MockRepository { get; set; }

        [SetUp]
        public void SetUp()
        {
            MockRepository = new MockRepository();
        }

        private void CreateConfiguration()
        {
            MockRepository.ReplayAll();

            _configuration = new Configuration(_dataRepository);
        }

        private void ExpectSaveUsername()
        {
            _dataRepository.Expect(v => v.SaveUsername(TestUsername));
        }

        private void MockDataRepository()
        {
            _dataRepository = MockRepository.StrictMock<DataRepository>();
        }

        [Test]
        public void WillGetPasswordOnGet()
        {
            MockDataRepository();

            _dataRepository.Expect(v => v.GetPassword()).Return(TestPassword);

            CreateConfiguration();

            _configuration.Password.Should().Be(TestPassword);

            MockRepository.VerifyAll();
        }

        [Test]
        public void WillGetUsernameOnGet()
        {
            MockDataRepository();

            _dataRepository.Expect(v => v.GetUsername()).Return(TestUsername);

            CreateConfiguration();

            _configuration.Username.Should().Be(TestUsername);

            MockRepository.VerifyAll();
        }

        [Test]
        public void WillSavePasswordOnSet()
        {
            MockDataRepository();

            _dataRepository.Expect(v => v.SavePassword(TestPassword));

            CreateConfiguration();

            _configuration.Password = TestPassword;

            MockRepository.VerifyAll();
        }

        [Test]
        public void WillSaveUsernameOnSet()
        {
            MockDataRepository();

            ExpectSaveUsername();

            CreateConfiguration();

            _configuration.Username = TestUsername;

            MockRepository.VerifyAll();
        }
    }
}