using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfoTrackSEO.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoTrackSEO.Providers.Tests
{
    [TestClass()]
    public class GoogleSearchProviderTests
    {
        public GoogleSearchProviderTests()
        {
            SecretProviders.LoadSecrets();
        }

        [TestMethod()]
        public void GetSearchResultsTest()
        {
            var provider = new GoogleSearchProvider();
            var blocks = provider.GetSearchResults("efiling integration", 100).Result;
            Assert.IsTrue(blocks.Count > 0);
        }
    }
}