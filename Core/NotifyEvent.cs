using System;

namespace D365.Extension.Core
{
    public class NotifyEvent
    {
        public DateTime EventTime { get; set; } = DateTime.UtcNow;

        public string EventOrigin { get; set; }

        public string EventMessage { get; set; }

        public Exception EventException { get; set; }
    }
}
