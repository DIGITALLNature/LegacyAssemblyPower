// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public interface IMessageService
    {
        string GetMessage(string code, int lcid = 1033, string defaultValue = null);
    }
}
