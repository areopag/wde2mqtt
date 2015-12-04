using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wde2mqtt
{
    public class Config
    {
        public String comPort { get; set; }
        public String mqttServer { get; set; }
        public String mqttTopicPrefix { get; set; }
    }
}
