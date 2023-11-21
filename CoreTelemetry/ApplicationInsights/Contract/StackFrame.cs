using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Telemetry.Contract
{
    [DataContract]
    [Description("Stack frame information.")]
    public class StackFrame
    {
        [DataMember(Name = "level", IsRequired = true)]
        [Required]
        [Description("Level in the call stack. For the long stacks SDK may not report every function in a call stack.")]
        public int Level;

        [DataMember(Name = "method", IsRequired = true)]
        [Required]
        [Description("Method name.")]
        [StringLength(1024, ErrorMessage = "String length should be max. 1024")]
        public string Method;

        [DataMember(Name = "assembly", IsRequired = false, EmitDefaultValue = false)]
        [Description("Name of the assembly (dll, jar, etc.) containing this function.")]
        [StringLength(1024, ErrorMessage = "String length should be max. 1024")]
        public string Assembly;

        [DataMember(Name = "fileName", IsRequired = false, EmitDefaultValue = false)]
        [Description("File name or URL of the method implementation.")]
        [StringLength(1024, ErrorMessage = "String length should be max. 1024")]
        public string FileName;

        [DataMember(Name = "line", IsRequired = false, EmitDefaultValue = false)]
        [Description("Line number of the code implementation.")]
        public int Line;
    }
}
