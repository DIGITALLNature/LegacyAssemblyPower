using System;
using System.Collections.Concurrent;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public sealed class ExecutorService : IExecutorService
    {
        //TODO: do some research to find a smarter solution

        //by default, Lazy objects are thread-safe.
        private static readonly Lazy<ConcurrentDictionary<string, IBeforeExecuteListener>> LazyBeforeExecuteListener = new Lazy<ConcurrentDictionary<string, IBeforeExecuteListener>>(() => new ConcurrentDictionary<string, IBeforeExecuteListener>());
        private static readonly Lazy<ConcurrentDictionary<string, IAfterExecuteListener>> LazyAfterExecuteListener = new Lazy<ConcurrentDictionary<string, IAfterExecuteListener>>(() => new ConcurrentDictionary<string, IAfterExecuteListener>());
        private static readonly Lazy<ConcurrentDictionary<string, IExecuteExceptionListener>> LazyExecuteExceptionListener = new Lazy<ConcurrentDictionary<string, IExecuteExceptionListener>>(() => new ConcurrentDictionary<string, IExecuteExceptionListener>());
        private static readonly Lazy<ConcurrentDictionary<string, INotifyListener>> LazyNotifyListener = new Lazy<ConcurrentDictionary<string, INotifyListener>>(() => new ConcurrentDictionary<string, INotifyListener>());

        public static ConcurrentDictionary<string, IBeforeExecuteListener> BeforeExecuteListener => LazyBeforeExecuteListener.Value;
        public static ConcurrentDictionary<string, IAfterExecuteListener> AfterExecuteListener => LazyAfterExecuteListener.Value;
        public static ConcurrentDictionary<string, IExecuteExceptionListener> ExecuteExceptionListener => LazyExecuteExceptionListener.Value;
        public static ConcurrentDictionary<string, INotifyListener> NotifyListener => LazyNotifyListener.Value;

        /// <summary>
        /// 
        /// </summary>
        public void BeforeExecute(Executor executor)
        {
            foreach (var beforeExecuteListener in BeforeExecuteListener.Values)
            {
                beforeExecuteListener.Fire(executor);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void AfterExecute(Executor executor, ExecutionResult result, Stopwatch timer)
        {
            foreach (var afterExecuteListener in AfterExecuteListener.Values)
            {
                afterExecuteListener.Fire(executor, result, timer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExecuteException(Executor executor, Exception e, Stopwatch timer)
        {
            foreach (var executeExceptionListener in ExecuteExceptionListener.Values)
            {
                executeExceptionListener.Fire(executor, e, timer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Notify(Executor executor, NotifyEvent notifyEvent)
        {
            foreach (var notifyListener in NotifyListener.Values)
            {
                notifyListener.Fire(executor, notifyEvent);
            }
        }
    }
}
