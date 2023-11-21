using D365.Extension.Core.Contract;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Xrm.Sdk;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    //https://docs.microsoft.com/en-us/previous-versions/azure/dn645543(v=azure.100)
    public static class AuthenticationExtension
    {
        private const int Timeout = 60000;

        //https://docs.microsoft.com/de-de/powerapps/developer/common-data-service/best-practices/business-logic/set-keepalive-false-interacting-external-hosts-plugin
        //by default, Lazy objects are thread-safe.
        private static readonly Lazy<HttpClient> Lazy = new Lazy<HttpClient>(() =>
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(Timeout)
            };
            client.DefaultRequestHeaders.ConnectionClose = true;
            return client;
        });

        private static HttpClient HttpClient => Lazy.Value;

        //https://accounts.accesscontrol.windows.net/<realm>/tokens/OAuth/2
        public static AccessControlAccessToken GetAccessControlAccessToken(this Executor executor, string tenantId, string resource, string clientId, string clientSecret)
        {
            const string requestBody = "grant_type=client_credentials&resource={0}&client_id={1}&client_secret={2}";
            var content = string.Format(requestBody, HttpUtils.UrlEncode(resource), HttpUtils.UrlEncode(clientId), HttpUtils.UrlEncode(clientSecret));

            var request = GetRequest(HttpMethod.Post, new Uri($"https://accounts.accesscontrol.windows.net/{tenantId}/tokens/OAuth/2"));
            request.Content = new StringContent(content);
            request.Content.Headers.ContentType.CharSet = HttpUtils.Utf8;
            request.Content.Headers.ContentType.MediaType = HttpUtils.ApplicationXwwwFormUrlencoded;

            var response = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (response)
            {
                var json = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                if (response.StatusCode != HttpStatusCode.OK) throw new InvalidPluginExecutionException($"Status {response.StatusCode}: {json}");
                var accessToken = executor.SerializerService.JsonDeserialize<AccessControlAccessToken>(json);
                return accessToken;
            }
        }

        //https://login.microsoftonline.com/<tenant id>/oauth2/token
        public static MicrosoftOnlineAccessToken GetMicrosoftOnlineAccessToken(this Executor executor, string tenantId, string resource, string clientId, string clientSecret)
        {
            const string requestBody = "grant_type=client_credentials&resource={0}&client_id={1}&client_secret={2}";
            var content = string.Format(requestBody, HttpUtils.UrlEncode(resource), HttpUtils.UrlEncode(clientId), HttpUtils.UrlEncode(clientSecret));

            var request = GetRequest(HttpMethod.Post, new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/token"));
            request.Content = new StringContent(content);
            request.Content.Headers.ContentType.CharSet = HttpUtils.Utf8;
            request.Content.Headers.ContentType.MediaType = HttpUtils.ApplicationXwwwFormUrlencoded;

            var response = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (response)
            {
                var json = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                if (response.StatusCode != HttpStatusCode.OK) throw new InvalidPluginExecutionException($"Status {response.StatusCode}: {json}");
                var accessToken = executor.SerializerService.JsonDeserialize<MicrosoftOnlineAccessToken>(json);
                return accessToken;
            }
        }
        
        private static HttpRequestMessage GetRequest(HttpMethod method, Uri uri)
        {
            var request = new HttpRequestMessage(method, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpUtils.ApplicationJson));
            request.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue(HttpUtils.Utf8));
            request.Properties["RequestTimeout"] = TimeSpan.FromMilliseconds(Timeout);
            return request;
        }
    }
}
