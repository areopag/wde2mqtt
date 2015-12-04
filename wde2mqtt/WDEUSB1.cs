using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;

namespace wde2mqtt
{    
    public class WDEUSB1 : IWDEDevice
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
           (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private String serialPortName;
        private SerialPort serialPort;
        private Boolean shutdownThreads;
        private Thread readSerialPortThreadInstance;

        public event WeatherDataReceivedHandler WeatherDataReceived;

        public WDEUSB1(String serialPortName)
        {            
            this.serialPortName = serialPortName;
        }

        public void Start()
        {
            serialPort = new SerialPort(serialPortName, 9600, Parity.None, 8, StopBits.One);
            serialPort.ReadTimeout = 200;

            shutdownThreads = false;
            readSerialPortThreadInstance = new Thread(new ThreadStart(delegate { readSerialPortThread(); }));
            readSerialPortThreadInstance.Name = "readSerialPortThread";
            readSerialPortThreadInstance.Start();

            log.Info("WDEUSB1 device started");
        }

        public void Stop()
        {
            shutdownThreads = true;
            readSerialPortThreadInstance.Join(250);

            log.Info("WDEUSB1 device thread stopped");
        }

        private void readSerialPortThread()
        {
            log.Debug("readSerialPortThread started");

            while (!shutdownThreads)
            {
                if(!serialPort.IsOpen)
                {
                    try {
                        serialPort.Open();
                        log.Info("USBWDE1 connection established");
                    }
                    catch(Exception ex)
                    {
                        log.Warn("Error while opening comport: " + ex.ToString() + "\nTry again in 10 seconds...");
                        Thread.Sleep(10000);
                        continue;
                    }
                }

                try {
                    String line = serialPort.ReadLine();

                    log.Debug("USBWDE1 raw data readed: " + line);

                    WDEDataCollection dataCollection = new WDEDataCollection(line);
                    WeatherDataReceived?.Invoke(this, dataCollection);
                }
                catch (System.TimeoutException) {
                    // do nothing
                }                
            }

            serialPort.Close();

            log.Debug("readSerialPortThread stopped");
        }

    }
}
