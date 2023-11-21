using System;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class RandomService : IRandomService
    {
        private readonly Random _random = new Random();

        public int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        public void Sleep()
        {
            Thread.Sleep(Next(123, 357));
        }

        public void Sleep(int minValue, int maxValue)
        {
            Thread.Sleep(Next(minValue, maxValue));
        }
    }
}
