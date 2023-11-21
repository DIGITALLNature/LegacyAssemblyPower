
// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public abstract partial class Executor
    {
        public IConfigService EnvironmentVariablesService => new EnvironmentVariablesService(this);
    }
}