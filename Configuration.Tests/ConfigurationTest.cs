using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CustomExtension;
namespace Configuration.Tests
{
    [TestFixture, Category("Configuration")]
    public class ConfigurationTest
    {
        [Test]
        public void GetTest()
        {
            string header = TestConfiguration.GetConfig().Header;

            Assert.AreEqual("localhost", header);
        }
        internal class TestConfiguration:ConfigurationBase
        {
            private string header;

            public string Header { get { return header; } }

            public static TestConfiguration GetConfig()
            {
                return GetConfig<TestConfiguration>("redis", "cache", true);
            }

            protected override void LoadValuesFromConfigurationXml(XmlNode node)
            {
                header = node.GetStringAttribute("header", "redis");
            }
        }
        
    }
}
