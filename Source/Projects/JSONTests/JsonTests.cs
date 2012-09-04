using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using WinRTJSON;

namespace JSONTests
{
    [TestClass]
    public class JsonTests
    {
        private const string TestJSON = @"{
                            ""access_token"" : ""ya29.AHES6ZRMNAQajiBLgjBJFMT2U-jp4NnVchhPZtPAradiYMSjHOprFw"",
                            ""token_type"" : ""Bearer"",
                            ""expires_in"" : 3600,
                            ""refresh_token"" : ""1/Qe-7zv5zNOb4i95zJ5K_L4qNuJjTGp3Js9HAkH_fUM0""
                        }";

        [TestMethod]
        public void WillConvertJsonIntoDictionary()
        {
            var results = JSON.JsonDecode(TestJSON);

            Assert.IsTrue(results is IDictionary<string, object>);
        }

        [TestMethod]
        public void WillConverJSONIntoItemsInTheDictionary()
        {
            var results = JSON.JsonDecode(TestJSON) as IDictionary<string, object>;

            Assert.AreEqual("ya29.AHES6ZRMNAQajiBLgjBJFMT2U-jp4NnVchhPZtPAradiYMSjHOprFw", results["access_token"].ToString());
            Assert.AreEqual("Bearer", results["token_type"].ToString());
            Assert.AreEqual(3600, (double)results["expires_in"]);
            Assert.AreEqual("1/Qe-7zv5zNOb4i95zJ5K_L4qNuJjTGp3Js9HAkH_fUM0", results["refresh_token"].ToString());
        }
    }
}