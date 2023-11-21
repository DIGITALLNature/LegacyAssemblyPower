using System;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public abstract partial class Executor
    {
        #region base interfaces
        /// <summary>
        /// 
        /// </summary>
        public ICacheService CacheService => new CacheService();

        /// <summary>
        /// 
        /// </summary>
        public ISerializerService SerializerService => new SerializerService();

        /// <summary>
        /// 
        /// </summary>
        public IExecutorService ExecutorService => new ExecutorService();

        /// <summary>
        /// 
        /// </summary>
        public IDateTimeService DateTimeService => new DateTimeService();

        /// <summary>
        /// 
        /// </summary>
        public IRandomService RandomService => new RandomService();
        #endregion
    }
}
