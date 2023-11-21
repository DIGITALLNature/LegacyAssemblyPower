using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Telemetry.Contract
{
    [DataContract]
    [Description("Exception details of the exception in a chain.")]
    public class ExceptionDetails
    {
        [DataMember(Name = "id", IsRequired = false, EmitDefaultValue = false)]
        [Description("In case exception is nested (outer exception contains inner one), the id and outerId properties are used to represent the nesting.")]
        public int Id;

        [DataMember(Name = "outerId", IsRequired = false, EmitDefaultValue = false)]
        [Description("The value of outerId is a reference to an element in ExceptionDetails that represents the outer exception")]
        public int OuterId;

        [DataMember(Name = "typeName", IsRequired = true)]
        [Required]
        [Description("Exception type name.")]
        [StringLength(1024, ErrorMessage = "String length should be max. 1024")]
        public string TypeName;

        [DataMember(Name = "message", IsRequired = true)]
        [Required]
        [Description("Exception message.")]
        [StringLength(32768, ErrorMessage = "String length should be max. 32768")]
        public string Message;

        [DataMember(Name = "hasFullStack", IsRequired = false)]
        [Description("Indicates if full exception stack is provided in the exception. The stack may be trimmed, such as in the case of a StackOverflow exception.")]
        public bool HasFullStack = true;

        [DataMember(Name = "stack", IsRequired = false)]
        [Description("Text describing the stack. Either stack or parsedStack should have a value.")]
        [StringLength(32768, ErrorMessage = "String length should be max. 32768")]
        public string Stack = "";

        [DataMember(Name = "parsedStack", IsRequired = false, EmitDefaultValue = false)]
        [Description("List of stack frames. Either stack or parsedStack should have a value.")]
        public List<StackFrame> ParsedStack = new List<StackFrame>();
    }
}
