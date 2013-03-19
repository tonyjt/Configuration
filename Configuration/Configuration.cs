using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;


namespace Configuration
{
    public class Configuration
    {
        XmlNode root;
        public static Configuration GetConfig(string key)
        {
            return (Configuration)ConfigurationManager.GetSection(key);
        }

        public void LoadValuesFromConfigurationXml(XmlNode node)
        {
            root = node;
        }

        public XmlNode GetSection(string nodePath)
        {
            return root.SelectSingleNode(nodePath);
        }
    }
}
