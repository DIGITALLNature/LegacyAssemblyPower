using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Logging.Contract
{
    [DataContract]
    [Description("System variables for a telemetry item.")]
    public class Envelope<T>
    {
        [DataMember(Name = "ver", IsRequired = false)]
        [Description("Envelope version. For internal use only. By assigning this the default, it will not be serialized within the payload unless changed to a value other than #1.")]
        public int SchemaVersion = 1;

        [DataMember(Name = "name", IsRequired = true)]
        [Required]
        [Description("Type name of telemetry data item.")]
        [StringLength(1024, ErrorMessage = "String length should be max. 1024")]
        public string Name;

        //[DataMember(Name = "time", IsRequired = true)]
        //[Required]
        //[Description("Event date time when telemetry item was created. This is the wall clock time on the client when the event was generated. There is no guarantee that the client's time is accurate. This field must be formatted in UTC ISO 8601 format, with a trailing 'Z' character, as described publicly on https://en.wikipedia.org/wiki/ISO_8601#UTC. Note: the number of decimal seconds digits provided are variable (and unspecified). Consumers should handle this, i.e. managed code consumers should not use format 'O' for parsing as it specifies a fixed length. Example: 2009-06-15T13:45:30.0000000Z.")]
        //public DateTimeOffset Time = DateTimeOffset.UtcNow;

        [DataMember(Name = "time", IsRequired = true)]
        [Required]
        [Description("Event date time when telemetry item was created. This is the wall clock time on the client when the event was generated. There is no guarantee that the client's time is accurate. This field must be formatted in UTC ISO 8601 format, with a trailing 'Z' character, as described publicly on https://en.wikipedia.org/wiki/ISO_8601#UTC. Note: the number of decimal seconds digits provided are variable (and unspecified). Consumers should handle this, i.e. managed code consumers should not use format 'O' for parsing as it specifies a fixed length. Example: 2009-06-15T13:45:30.0000000Z.")]
        public string Time = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        //[OnSerializing]
        //public void OnSerializing(StreamingContext context)
        //{
        //TimeSerialized = Time.ToString("yyyy-MM-ddTHH:mm:ssZ");
        //}

        //[OnDeserialized]
        //public void OnDeserializing(StreamingContext context)
        //{
        //Time = TimeSerialized == null ? default(DateTimeOffset) : DateTimeOffset.Parse(TimeSerialized);
        //}

        [DataMember(Name = "sampleRate", IsRequired = false, EmitDefaultValue = false)]
        [Description("Sampling rate used in application. This telemetry item represents 1 / sampleRate actual telemetry items.")]
        public double SampleRate;

        [DataMember(Name = "seq", IsRequired = false, EmitDefaultValue = false)]
        [Description("Sequence field used to track absolute order of uploaded events.")]
        [StringLength(64, ErrorMessage = "String length should be max. 64")]
        public string SequenceNumber;

        [DataMember(Name = "iKey", IsRequired = true)]
        [Required]
        [Description("The application's instrumentation key. The key is typically represented as a GUID, but there are cases when it is not a guid. No code should rely on iKey being a GUID. Instrumentation key is case insensitive.")]
        [StringLength(40, ErrorMessage = "String length should be max. 40")]
        public string InstrumentationKey;

        [DataMember(Name = "flags", IsRequired = false, EmitDefaultValue = false)]
        [Description("A collection of values bit-packed to represent how the event was processed. Currently represents whether IP address needs to be stripped out from event (set 0x200000) or should be preserved.")]
        public long Flags;

        [DataMember(Name = "tags", IsRequired = false, EmitDefaultValue = false)]
        [Description("Key/value collection of context properties. See ContextTagKeys for information on available properties.")]
        public Dictionary<string, string> Tags;

        [DataMember(Name = "data", IsRequired = true)]
        [Required]
        [Description("Telemetry data item.")]
        public Data<T> Data;
    }
}
