using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wde2mqtt
{
    public class WDEDataCollection
    {
        public DateTime Timestamp { get; private set; }
        public Decimal[] Temperatures { get; private set; }
        public Decimal[] Humidities { get; private set; }
        public Decimal WindSpeed { get; private set; }
        public Decimal Rainfall { get; private set; }
        public Boolean Rain { get; private set; }
        
        public WDEDataCollection(String rawData)
        {
            this.Timestamp = DateTime.Now;

            Temperatures = new decimal[9];
            Humidities = new decimal[9];

            String[] splits = rawData.Split(';');

            if (splits.Length == 25)
            {
                for (int i = 3; i <= 10; i++)
                {
                    Temperatures[i - 3] = getDecimal(splits[i]);
                }

                for (int i = 11; i <= 18; i++)
                {
                    Humidities[i - 11] = getDecimal(splits[i]);
                }

                Temperatures[8] = getDecimal(splits[19]);
                Humidities[8] = getDecimal(splits[20]);

                WindSpeed = getDecimal(splits[21]);
                Rainfall = getDecimal(splits[22]);

                Rain = false;
                if (splits[23] == "1")
                    Rain = true;
            }
        }

        private decimal getDecimal(String str)
        {
            Decimal value;

            if (Decimal.TryParse(str, out value))
                return value;
            else
                return -128;
        }

    }
}
