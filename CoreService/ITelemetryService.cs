using System;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public interface ITelemetryService
    {
        void Exception(Exception exception);

        void Metric(double duration, string name = null);

        void Event(string name);
    }
}
