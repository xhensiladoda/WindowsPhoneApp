using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
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
        public static List<T> DeserializeJSONArray<T>(string json)
        {
            var instance = Activator.CreateInstance<List<T>>();
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(instance.GetType());
                return (List<T>)deserializer.ReadObject(ms);
            }
        }

        /**
         * Crea un oggetto GeoCoordinate da una stringa nel formato "lat,lon".
         * @return: GeoCoordinate del PoI.
         */
        public static GeoCoordinate String2GeoCoordinate(string position)
        {
            // estrae le due stringhe con le due posizioni
            string[] coordinate = position.Split(new char[] { ',' });
            // converte le singole stringe in float
            float latitude = float.Parse(coordinate[0], CultureInfo.InvariantCulture);
            float longitude = float.Parse(coordinate[1], CultureInfo.InvariantCulture);
            return new GeoCoordinate(latitude, longitude);
        }
    }
}
