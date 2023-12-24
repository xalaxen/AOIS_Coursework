using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace Lab_3.Classes
{
    public class SaveAndLoad
    {
        public static void SaveToFile<T>(String FileName, ObservableCollection<T> SerializableObjects)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (StreamWriter sw = new StreamWriter(FileName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, SerializableObjects);
            }
        }

        public static List<T> LoadFromFile<T>(string FileName)
        {
            if (File.Exists(FileName))
            {
                using (StreamReader file = new StreamReader(FileName))
                {
                    string json = file.ReadToEnd();
                    return JsonConvert.DeserializeObject<List<T>>(json);
                }
            }
            throw new FileNotFoundException("Файл не найден!");
        }
    }
}
