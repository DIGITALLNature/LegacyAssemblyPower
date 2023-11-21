using System.Linq;
using D365.Extension.Model;
using Microsoft.Xrm.Sdk;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    /// <summary>
    /// TransactionCurrency extensions
    /// </summary>
    public static class CurrencyExtension
    {
        /// <summary>
        /// Get TransactionCurrency by ISO currency code
        /// </summary>
        /// <param name="executor">self</param>
        /// <param name="code">ISO currency code</param>
        /// <returns></returns>
        public static TransactionCurrency CurrencyByCode(this Executor executor, string code)
        {
            var cacheKey = $"CurrencyExtension-{code}";

            if (executor.CacheService.TryGet(cacheKey, out object value))
            {
                return (TransactionCurrency)value;
            }

            using (var serviceContext = executor.DataContext(executor.ElevatedOrganizationService))
            {
                var currency = (from cur in serviceContext.TransactionCurrencySet
                                where cur.ISOCurrencyCode == code
                                select cur).SingleOrDefault();
                if (currency == null)
                {
                    throw new InvalidPluginExecutionException($"ISO Currency Code {code} missing!");
                }
                executor.CacheService.SetAbsolute(cacheKey, currency, 121);
                return currency;
            }
        }
    }
}