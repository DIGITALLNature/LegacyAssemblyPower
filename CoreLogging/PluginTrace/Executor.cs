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
            executor.LoggingService.Trace("LoggingService.BeforeExecuteListener");
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
            executor.LoggingService.Trace($"LoggingService.AfterExecuteListener; Result:{result}; {timer.ElapsedMilliseconds}ms");
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
            executor.LoggingService.Error($"LoggingService.ExecuteExceptionListener; Exception:{e.RootException()?.Message}; {timer.ElapsedMilliseconds}ms", e);
        }
    }
}
