using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Configuration
{
    internal class ConfigurationHandler : IConfigurationSectionHandler
    {
        public virtual object Create(Object parent, Object context, XmlNode node)
        {
            Configuration config = new Configuration();
            config.LoadValuesFromConfigurationXml(node);
            return config;
        }
    }
}
