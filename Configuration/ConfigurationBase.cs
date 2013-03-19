using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Xml;

namespace Configuration
{
    [Serializable]
    public abstract class ConfigurationBase
    {
        static string GetCacheKey(Type type)
        {
            return string.Format("AppConfig-{0}", type.FullName);
        }

        protected static ConfigurationBase GetConfig<T>(XmlNode section)
             where T : ConfigurationBase, new()
        {
            Type thisType = typeof(T);
            return GetConfig(thisType, section);
        }


        protected static T GetConfig<T>(string configKey,string sectionName)
             where T : ConfigurationBase, new()
        {
            Type thisType = typeof(T);
            ConfigurationBase config = GetConfig(configKey,thisType, sectionName);
            //return GenericHelper.GenericCast<ConfigurationBase, T>(config);
            return (T)config;
        }

        protected static ConfigurationBase GetConfig(string configKey,Type type, string sectionName)
        {
            Configuration config = Configuration.GetConfig(sectionName);

            string key = GetCacheKey(type);
            ConfigurationBase typeConfig = null;
            //if (useCache)
            //{
            //    typeConfig = MemoryCache.Get(key) as ConfigurationBase;
            //}

            if (typeConfig == null)
            {
                if (config != null)
                {
                    XmlNode node = config.GetSection(configKey);
                    typeConfig = GetConfig(type, node);
                }
                else
                {
                    typeConfig = (ConfigurationBase)Activator.CreateInstance(type);
                    typeConfig.LoadDefaultConfigurations();
                    //MemoryCache.Max(key, typeConfig);
                }
            }
            return typeConfig;
        }

        protected static ConfigurationBase GetConfig(Type type
            , XmlNode node)
        {
            ConfigurationBase config = null;
            string key = GetCacheKey(type);

            //if (useCache)
            //{
            //    config = MemoryCache.Get(key) as ConfigurationBase;
            //}

            if (config == null)
            {
                config = Activator.CreateInstance(type) as ConfigurationBase;
                if (config != null)
                {
                    bool singleCacheFile = false;
                    string filename = null;
                    if (node != null)
                    {
                        XmlAttribute attConfigSource = node.Attributes["configSource"];
                        if (attConfigSource != null && !string.IsNullOrEmpty(attConfigSource.Value))
                        {
                            filename = PathHelper.MapPath(attConfigSource.Value);
                            if (File.Exists(filename))
                            {
                                XmlDocument doc = new XmlDocument();
                                try
                                {
                                    doc.Load(filename);
                                    node = doc.DocumentElement;
                                    singleCacheFile = true;
                                }
                                catch (Exception ex)
                                {
                                    throw new ConfigurationException("load config file failed");
                                }
                            }
                        }
                    }
                    config.LoadDefaultConfigurations();
                    config.LoadValuesFromConfigurationXml(node);
                    //if (singleCacheFile)
                    //{
                    //    MemoryCache.Max(key, config, new CacheDependency(filename));
                    //}
                    //else
                    //{
                    //    MemoryCache.Max(key, config);
                    //}
                }
            }
            return config;
        }

        protected virtual void LoadDefaultConfigurations()
        {

        }

        protected virtual void LoadValuesFromConfigurationXml(XmlNode node)
        {

        }

        #region reflector

        protected void LoadModules<T>(XmlNode node, ref Dictionary<string, T> modules, params object[] args)
        {
            if (modules == null)
                modules = new Dictionary<string, T>();

            if (node != null)
            {
                foreach (XmlNode n in node.ChildNodes)
                {
                    if (n.NodeType != XmlNodeType.Comment)
                    {
                        switch (n.Name)
                        {
                            case "clear":
                                modules.Clear();
                                break;
                            case "remove":
                                XmlAttribute removeNameAtt = n.Attributes["name"];
                                string removeName = removeNameAtt == null ? null : removeNameAtt.Value;

                                if (!string.IsNullOrEmpty(removeName) && modules.ContainsKey(removeName))
                                    modules.Remove(removeName);

                                break;
                            case "add":
                                XmlAttribute en = n.Attributes["enabled"];
                                if (en != null && en.Value == "false")
                                    continue;

                                XmlAttribute nameAtt = n.Attributes["name"];
                                XmlAttribute typeAtt = n.Attributes["type"];
                                string name = nameAtt == null ? null : nameAtt.Value;
                                string itype = typeAtt == null ? null : typeAtt.Value;

                                if (string.IsNullOrEmpty(name))
                                    continue;

                                if (string.IsNullOrEmpty(itype))
                                    continue;

                                Type type = Type.GetType(itype);

                                if (type == null)
                                    continue;

                                T mod = default(T);

                                try
                                {
                                    mod = (T)Activator.CreateInstance(type, n);
                                }
                                catch (Exception ex)
                                {
                                    throw new ConfigurationException("load modules failed");
                                }

                                if (mod == null)
                                    continue;

                                modules[name] = mod;
                                break;

                        }
                    }
                }
            }
        }

        #endregion


        public static string GetStringAttribute(XmlAttributeCollection attributes, string key, string defaultValue)
        {
            if (attributes[key] != null
                && !string.IsNullOrEmpty(attributes[key].Value))
                return attributes[key].Value;
            return defaultValue;
        }

        public static int GetIntAttribute(XmlAttributeCollection attributes, string key, int defaultValue)
        {
            int val = defaultValue;

            if (attributes[key] != null
                && !string.IsNullOrEmpty(attributes[key].Value))
            {
                int.TryParse(attributes[key].Value, out val);
            }
            return val;
        }

        public static float GetFloatAttribute(XmlAttributeCollection attributes, string key, float defaultValue)
        {
            float val = defaultValue;

            if (attributes[key] != null
                && !string.IsNullOrEmpty(attributes[key].Value))
            {
                float.TryParse(attributes[key].Value, out val);
            }
            return val;
        }

        public static bool GetBoolAttribute(XmlAttributeCollection attributes, string key, bool defaultValue)
        {
            bool val = defaultValue;

            if (attributes[key] != null
                && !string.IsNullOrEmpty(attributes[key].Value))
            {
                bool.TryParse(attributes[key].Value, out val);
            }
            return val;
        }

        
    }
}
