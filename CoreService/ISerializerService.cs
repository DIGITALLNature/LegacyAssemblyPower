using System.Runtime.Serialization.Json;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    /// <summary>
    /// Json SerializerService Interface
    /// </summary>
    public interface ISerializerService
    {
        /// <summary>
        /// Convert Object to Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="settings">optional DataContractJsonSerializerSettings; otherwise using this as default UseSimpleDictionaryFormat = true, DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ"</param>
        /// <returns></returns>
        string JsonSerialize<T>(object data, DataContractJsonSerializerSettings settings = default);

        /// <summary>
        /// Convert Json to Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">JSON string</param>
        /// <param name="settings">optional DataContractJsonSerializerSettings; otherwise using this as default UseSimpleDictionaryFormat = true, DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ".</param>
        /// <param name="tidy">Optional. Set it to true to convert empty string attribute values to null.</param>
        /// <returns></returns>
        T JsonDeserialize<T>(string json, DataContractJsonSerializerSettings settings = default, bool tidy = false);
    }
}