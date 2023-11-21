using System;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public abstract partial class Executor
    {
        private readonly TelemetryServiceAfterExecuteListener _afterTelemetryService = new TelemetryServiceAfterExecuteListener();
        private readonly TelemetryServiceExecuteExceptionListener _exceptionTelemetryService = new TelemetryServiceExecuteExceptionListener();
        private readonly TelemetryServiceNotifyListener _notifyTelemetryService = new TelemetryServiceNotifyListener();

        public ITelemetryService TelemetryService => new TelemetryService(this);

        public abstract bool Telemetry { get; }
    }

    public class TelemetryServiceAfterExecuteListener : IAfterExecuteListener
    {
        public TelemetryServiceAfterExecuteListener()
        {
            ExecutorService.AfterExecuteListener.TryAdd(GetType().Name, this);
        }

        public void Fire(Executor executor, ExecutionResult result, Stopwatch timer)
        {
            if (executor.Telemetry && (result == ExecutionResult.Ok || result == ExecutionResult.Failure)) executor.TelemetryService.Metric(timer.ElapsedMilliseconds);
        }
    }

    public class TelemetryServiceExecuteExceptionListener : IExecuteExceptionListener
    {
        public TelemetryServiceExecuteExceptionListener()
        {
            ExecutorService.ExecuteExceptionListener.TryAdd(GetType().Name, this);
        }

        public void Fire(Executor executor, Exception e, Stopwatch timer)
        {
            if (executor.Telemetry) executor.TelemetryService.Exception(e.RootException());
        }
    }

    public class TelemetryServiceNotifyListener : INotifyListener
    {
        public TelemetryServiceNotifyListener()
        {
            ExecutorService.NotifyListener.TryAdd(GetType().Name, this);
        }

        public void Fire(Executor executor, NotifyEvent notifyEvent)
        {
            if (executor.Telemetry && notifyEvent.EventException != null) if (notifyEvent.EventException != null) executor.TelemetryService.Exception(notifyEvent.EventException);
        }
    }
}
