using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wde2mqtt
{
    public delegate void WeatherDataReceivedHandler(object sender, WDEDataCollection dataCollection);

    interface IWDEDevice
    {
        void Start();
        void Stop();
        event WeatherDataReceivedHandler WeatherDataReceived;
    }
}
