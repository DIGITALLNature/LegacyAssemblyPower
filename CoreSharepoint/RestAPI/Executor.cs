// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public abstract partial class Executor
    {
        public ISharepointService SharepointService => new SharepointService(this);
    }
}
