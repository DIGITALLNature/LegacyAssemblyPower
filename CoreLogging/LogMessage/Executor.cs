using System;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public abstract partial class Executor
    {
        private readonly LoggingServiceBeforeExecuteListener _beforeLoggingService = new LoggingServiceBeforeExecuteListener();
        private readonly LoggingServiceAfterExecuteListener _afterLoggingService = new LoggingServiceAfterExecuteListener();
        private readonly LoggingServiceExecuteExceptionListener _exceptionLoggingService = new LoggingServiceExecuteExceptionListener();
        private readonly LoggingServiceNotifyListener _notifyLoggingService = new LoggingServiceNotifyListener();

        public ILoggingService LoggingService => new LoggingService(this);
    }

    public class LoggingServiceBeforeExecuteListener : IBeforeExecuteListener
    {
        public LoggingServiceBeforeExecuteListener()
        {
            ExecutorService.BeforeExecuteListener.TryAdd(GetType().Name, this);
        }

        public void Fire(Executor executor)
        {
            executor.LoggingService.Trace("BeforeExecute");
        }
    }

    public class LoggingServiceAfterExecuteListener : IAfterExecuteListener
    {
        public LoggingServiceAfterExecuteListener()
        {
            ExecutorService.AfterExecuteListener.TryAdd(GetType().Name, this);
        }

        public void Fire(Executor executor, ExecutionResult result, Stopwatch timer)
        {
            executor.LoggingService.Trace($"AfterExecute; Result:{result}; {timer.ElapsedMilliseconds}ms");
        }
    }

    public class LoggingServiceExecuteExceptionListener : IExecuteExceptionListener
    {
        public LoggingServiceExecuteExceptionListener()
        {
            ExecutorService.ExecuteExceptionListener.TryAdd(GetType().Name, this);
        }

        public void Fire(Executor executor, Exception e, Stopwatch timer)
        {
            executor.LoggingService.Error($"ExecuteException; Exception:{e.RootException()?.Message}; {timer.ElapsedMilliseconds}ms", e);
        }
    }

    public class LoggingServiceNotifyListener : INotifyListener
    {
        public LoggingServiceNotifyListener()
        {
            ExecutorService.NotifyListener.TryAdd(GetType().Name, this);
        }

        public void Fire(Executor executor, NotifyEvent notifyEvent)
        {
            if (notifyEvent.EventException == null)
            {
                executor.LoggingService.Warn($"Notify; Message:{notifyEvent.EventMessage}; Origin:{notifyEvent.EventOrigin}");
            }
            else
            {
                executor.LoggingService.Error($"Notify; Message:{notifyEvent.EventMessage}; Exception:{notifyEvent.EventException?.Message}; Origin:{notifyEvent.EventOrigin}", notifyEvent.EventException);
            }
        }
    }
}
