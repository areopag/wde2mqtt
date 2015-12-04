using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace wde2mqtt
{
    public class ConfigHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
           (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void createConfig(String filename)
        {
            Config config = new Config();
            saveConfig(filename, config);

            log.Info("Config file created");
        }

        public static Config loadConfig(String filename)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Config));

            TextReader reader = new StreamReader(filename);
            object obj = deserializer.Deserialize(reader);
            Config config = (Config)obj;
            reader.Close();

            log.Info("Config file loaded");

            return config;
        }

        public static void saveConfig(String filename, Config config)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            using(TextWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, config);
            }

            log.Info("Config file saved");
        }
    }
}
