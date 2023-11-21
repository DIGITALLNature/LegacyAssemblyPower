
// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public abstract partial class Executor
    {
        public IConfigService ConfigService => new ConfigService(this);

        public abstract string GetConfig(string key, int lcid = 1033, string defaultValue = null);
    }
}