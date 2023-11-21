using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SerializerService : ISerializerService
    {
        public DataContractJsonSerializerSettings Settings = new DataContractJsonSerializerSettings
        {
            UseSimpleDictionaryFormat = true,
            DateTimeFormat = new DateTimeFormat("yyyy-MM-ddTHH:mm:ssZ")
        };

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public string JsonSerialize<T>(object data, DataContractJsonSerializerSettings settings = default)
        {
            if (data == null) return null;
            if (settings == default)
            {
                settings = Settings;
            }
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(typeof(T), settings).WriteObject(ms, data);
                var json = ms.ToArray();
                return Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="settings"></param>
        /// <param name="tidy"></param>
        /// <returns></returns>
        public T JsonDeserialize<T>(string json, DataContractJsonSerializerSettings settings = default, bool tidy = false)
        {
            if (json == null) return default;

            var cleanJson = !tidy ? json : json.Replace("\"\"", "null"); // Circumvent "" for numbers which are NaN, undefined or null for stringified objects

            if (settings == default)
            {
                settings = Settings;
            }
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(cleanJson)))
            {
                return (T)new DataContractJsonSerializer(typeof(T), settings).ReadObject(ms);
            }
        }
    }
}
