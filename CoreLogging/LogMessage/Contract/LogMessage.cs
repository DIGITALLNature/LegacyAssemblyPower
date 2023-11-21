using System;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core.Contract
{
    public class LogMessage
    {
        public string Message { get; set; }

        public string EntityName { get; set; }

        public Guid EntityId { get; set; }
    }
}
