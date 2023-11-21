using System;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
  public static class ConfigTranslatorExtension
  {
    private static readonly Regex Regex = new Regex("[^a-z0-9_]", RegexOptions.Compiled);

    /// <summary>
    /// default translator, use like this:<br/>
    /// <b><pre>public override string ConfigTranslator(string key, int lcid = 1033) => this.TranslateConfigKey(key);</pre></b>
    /// </summary>
    /// <param name="executor">self</param>
    /// <param name="key">env var schema name</param>
    /// <param name="prefix">env var prefix </param>
    /// <param name="lcid">language code, for future use only</param>
    /// <returns></returns>
    internal static string TranslateConfigKey(this Executor executor, string key, string prefix = "dgt_", int lcid = 1033)
    {
      return string.IsNullOrEmpty(key) || (key.StartsWith(prefix) && key.EndsWith("_env")) ? key : $"{prefix}{Sanitize(key)}_env";
    }

    private static string Sanitize(string key)
    {
      //be strict, this is a generic service
      return Regex.Replace(string.Join("_", key.Trim().ToLowerInvariant().Split(new[] { ' ', '\t', '.', ';', '_', '/', '\\', '#', ':', '!', '@', '+', '-', '~', '&', '(', ')', '[', ']', '{', '}', '?' }, StringSplitOptions.RemoveEmptyEntries)),"");
    }
  }
}