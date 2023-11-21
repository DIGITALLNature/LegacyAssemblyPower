using System.Text;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    internal class HttpUtils
    {
        public const string Utf8 = "utf-8";
        public const string ApplicationJson = "application/json";
        public const string ApplicationXwwwFormUrlencoded = "application/x-www-form-urlencoded";
        public const string Bearer = "Bearer";
        public const string XRequestDigest = "X-RequestDigest";
        
        public static string UrlEncode(string text)
        {
            var builder = new StringBuilder(text.Length);
            foreach (var c in text)
            {
                switch (c)
                {
                    case ' ':
                        builder.Append("%20");
                        break;
                    case '!':
                        builder.Append("%21");
                        break;
                    case '"':
                        builder.Append("%22");
                        break;
                    case '#':
                        builder.Append("%23");
                        break;
                    case '$':
                        builder.Append("%24");
                        break;
                    case '%':
                        builder.Append("%25");
                        break;
                    case '&':
                        builder.Append("%26");
                        break;
                    case '\'':
                        builder.Append("%27");
                        break;
                    case '(':
                        builder.Append("%28");
                        break;
                    case ')':
                        builder.Append("%29");
                        break;
                    case '*':
                        builder.Append("%2A");
                        break;
                    case '+':
                        builder.Append("%2B");
                        break;
                    case ',':
                        builder.Append("%2C");
                        break;
                    case '-':
                        builder.Append("%2D");
                        break;
                    case '.':
                        builder.Append("%2E");
                        break;
                    case '/':
                        builder.Append("%2F");
                        break;
                    case ':':
                        builder.Append("%3A");
                        break;
                    case ';':
                        builder.Append("%3B");
                        break;
                    case '<':
                        builder.Append("%3C");
                        break;
                    case '=':
                        builder.Append("%3D");
                        break;
                    case '>':
                        builder.Append("%3E");
                        break;
                    case '?':
                        builder.Append("%3F");
                        break;
                    case '@':
                        builder.Append("%40");
                        break;
                    case '[':
                        builder.Append("%5B");
                        break;
                    case '\\':
                        builder.Append("%5C");
                        break;
                    case ']':
                        builder.Append("%5D");
                        break;
                    case '{':
                        builder.Append("%7B");
                        break;
                    case '|':
                        builder.Append("%7C");
                        break;
                    case '}':
                        builder.Append("%7D");
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }
            return builder.ToString();
        }
    }
}
