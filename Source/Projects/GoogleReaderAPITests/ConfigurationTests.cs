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
        private DataRepository _dataRepository;
        private Configuration _configuration;
        internal MockRepository MockRepository { get; set; }

        [SetUp]
        public void SetUp()
        {
            MockRepository = new MockRepository();
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

        private void ExpectSaveUsername()
        {
            _dataRepository.Expect(v => v.SaveUsername(TestUsername));
        }

        private void MockDataRepository()
        {
            _dataRepository = MockRepository.StrictMock<DataRepository>();
        }

        private void CreateConfiguration()
        {
            MockRepository.ReplayAll();

            _configuration = new Configuration(_dataRepository);
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
    }
}