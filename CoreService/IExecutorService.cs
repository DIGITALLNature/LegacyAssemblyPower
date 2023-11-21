using System;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public interface IExecutorService
    {
        void BeforeExecute(Executor executor);

        void AfterExecute(Executor executor, ExecutionResult result, Stopwatch timer);

        void ExecuteException(Executor executor, Exception e, Stopwatch timer);

        //Warning or Error, not for tracing/debugging!
        void Notify(Executor executor, NotifyEvent notifyEvent);
    }

    public interface IBeforeExecuteListener
    {
        void Fire(Executor executor);
    }

    public interface IAfterExecuteListener
    {
        void Fire(Executor executor, ExecutionResult result, Stopwatch timer);
    }

    public interface IExecuteExceptionListener
    {
        void Fire(Executor executor, Exception e, Stopwatch timer);
    }

    public interface INotifyListener
    {
        void Fire(Executor executor, NotifyEvent notifyEvent);
    }
}
