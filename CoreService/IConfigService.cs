// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConfigService
    {
        string GetConfig(string key, int lcid = 1033, string defaultValue = null);
    }
}
