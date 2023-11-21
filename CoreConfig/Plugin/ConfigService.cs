
// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public sealed class ConfigService : IConfigService
    {
        private readonly Executor _executor;

        public ConfigService(Executor executor)
        {
            _executor = executor;
        }

        public string GetConfig(string key, int lcid = 1033, string defaultValue = null)
        {
            var cacheKey = $"ConfigService-{key.ToLowerInvariant()}.{lcid}";

            if (_executor.CacheService.TryGet(cacheKey, out object value))
            {
                return (string)value;
            }
            var configValue = _executor.GetConfig(key, lcid, defaultValue);

            _executor.CacheService.SetSliding(cacheKey, configValue ?? string.Empty, 121);
            return configValue;
        }
    }
}
