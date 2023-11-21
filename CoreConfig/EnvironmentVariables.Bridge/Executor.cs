// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public abstract partial class Executor
    {
        public IConfigService ConfigService => new EnvironmentVariablesService(this, ConfigTranslator);

        public abstract string ConfigTranslator(string key, int lcid = 1033);
    }
}