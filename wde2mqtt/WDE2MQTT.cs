using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;

namespace wde2mqtt
{
    public class WDE2MQTT
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
           (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Boolean shutdownTasks;
        private Thread consoleThreadInstance;
        private IWDEDevice wdeDevice;
        private MqttClient mqttClient;
        private Config appConfig;
        
        public void Start()
        {
            Console.Title = "WDE 2 MQTT";

            appConfig = ConfigHelper.loadConfig("config.xml");

            wdeDevice = new WDEUSB1(appConfig.comPort);
            wdeDevice.WeatherDataReceived += _wdeusb_WeatherDataReceived;
            wdeDevice.Start();

            mqttClient = new MqttClient(appConfig.mqttServer);

            String clientId = Guid.NewGuid().ToString();
            mqttClient.Connect(clientId);
            log.Info("MQTT broker connection established");

            shutdownTasks = false;

            consoleThreadInstance = new Thread(new ThreadStart(delegate { consoleThread(); }));
            consoleThreadInstance.Name = "consoleThread";
            consoleThreadInstance.Start();

            log.Info("Application started");
        }

        private void _wdeusb_WeatherDataReceived(object sender, WDEDataCollection dataCollection)
        {
            for (int i = 0; i <= 8; i++)
            {
                publishWeatherData("temperature/" + i, dataCollection.Temperatures[i]);
                publishWeatherData("humidity/" + i, dataCollection.Humidities[i]);                
            }

            publishWeatherData("windspeed", dataCollection.WindSpeed);
            publishWeatherData("rainfall", dataCollection.Rainfall);
            publishWeatherData("rain", dataCollection.Rain);

            log.Info("WeatherDataReceived and published via MQTT");
        }

        private void publishWeatherData(String topic, Decimal value)
        {
            if(value > -128)
            {
                byte[] bytesvalue = System.Text.UTF8Encoding.UTF8.GetBytes(value.ToString().Replace(',', '.'));
                mqttClient.Publish(appConfig.mqttTopicPrefix + "/" + topic, bytesvalue);
                log.Debug("MQTT publish to topic '" + appConfig.mqttTopicPrefix + "/" + topic + "': '" + value.ToString() + "'");
            }
        }

        private void publishWeatherData(String topic, Boolean value)
        {
            byte[] bytesvalue = System.Text.UTF8Encoding.UTF8.GetBytes(value.ToString());
            mqttClient.Publish(appConfig.mqttTopicPrefix + "/" + topic, bytesvalue);
            log.Debug("MQTT publish to topic '" + appConfig.mqttTopicPrefix + "/" + topic + "': '" + value.ToString() + "'");
        }

        public void Stop()
        {
            log.Info("Shutdown application...");
            shutdownTasks = true;
            wdeDevice.Stop();
            mqttClient.Disconnect();
            consoleThreadInstance.Join(100);
        }

        private void consoleThread()
        {
            ConsoleKeyInfo key;

            log.Debug("Console thread started");

            while(!shutdownTasks)
            {
                key = Console.ReadKey();
                if(key.Key == ConsoleKey.X && key.Modifiers == ConsoleModifiers.Control)
                {
                    // shutdown the application
                    Stop();
                }                
            }

            log.Debug("Console thread stopped");
        }
    }
}
