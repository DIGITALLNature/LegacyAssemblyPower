
// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Contract
{
    public enum LogLevel
    {
        /// <summary>
        /// All logging levels
        /// </summary>
        All = 596030000,
        /// <summary>
        /// A trace logging level
        /// </summary>
        Trace = 596030001,
        /// <summary>
        /// A debug logging level
        /// </summary>
        Debug = 596030002,
        /// <summary>
        /// A info logging level
        /// </summary>
        Info = 596030004,
        /// <summary>
        /// A warn logging level
        /// </summary>
        Warn = 596030008,
        /// <summary>
        /// An error logging level
        /// </summary>
        Error = 596030016,
        /// <summary>
        /// A fatal logging level
        /// </summary>
        Fatal = 596030032,
        /// <summary>
        /// Do not log anything.
        /// </summary>
        Off = 596030064
    }
}
