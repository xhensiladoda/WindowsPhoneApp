using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using Windows.Devices.Geolocation;

namespace AppSDEM
{
    public static class Utils
    {
        /**
         * Deserializza una stringa formattata come un Array JSON in una lista di oggetti
         * @param json: stringa JSON
         * @param T: tipo di oggetti presenti nell'array JSON
         */
        public static List<T> deserializeJSONArray<T>(string json)
        {
            var instance = Activator.CreateInstance<List<T>>();
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(instance.GetType());
                return (List<T>)deserializer.ReadObject(ms);
            }
        }
    }
}
