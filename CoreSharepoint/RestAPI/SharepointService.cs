using D365.Extension.Core.Contract.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public sealed class SharepointService : ISharepointService
    {
        private const int Timeout = 15000;

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

        private readonly Executor _executor;

        private string _sharePointUrl;
        private Func<string> _accessToken;

        public SharepointService(Executor executor)
        {
            _executor = executor;
        }

        public ISharepointService Init(string sharePointUrl, Func<string> accessToken)
        {
            _sharePointUrl = sharePointUrl;
            _accessToken = accessToken;
            return this;
        }

        public bool GetDigest(out SharepointResponse response)
        {
            const string url = "_api/contextinfo";
            var request = GetRequest(HttpMethod.Post, new Uri(_sharePointUrl + url), _accessToken.Invoke());
            request.Content = new StringContent("");
            request.Content.Headers.ContentType.CharSet = HttpUtils.Utf8;
            request.Content.Headers.ContentType.MediaType = HttpUtils.ApplicationJson;
            request.Content.Headers.ContentLength = 0;
            var result = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (result)
            {
                response = GetResponse<ContextWebInformationResponse>(result);
            }
            return response.StatusCode == (int)HttpStatusCode.OK;
        }

        public bool GetBinaryByServerRelativePath(string relativePath, out SharepointResponse response)
        {
            const string url = "_api/web/getfilebyserverrelativeurl({0})/$value";
            _executor.Delegate.TracingService.Trace(string.Format(url, $"'{relativePath}'"));
            var request = GetRequest(HttpMethod.Get, new Uri(_sharePointUrl + string.Format(url, HttpUtils.UrlEncode($"'{relativePath}'"))), _accessToken.Invoke());
            var result = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (result)
            {
                response = GetBinaryResponse<BinaryDataResponse>(result);
            }
            return response.StatusCode == (int)HttpStatusCode.OK;
        }

        public bool GetFileByServerRelativePath(string relativePath, out SharepointResponse response)
        {
            const string url = "_api/Web/getfilebyserverrelativeurl({0})/ListItemAllFields";
            _executor.Delegate.TracingService.Trace(string.Format(url, $"'{relativePath}'"));
            var request = GetRequest(HttpMethod.Get, new Uri(_sharePointUrl + string.Format(url, HttpUtils.UrlEncode($"'{relativePath}'"))), _accessToken.Invoke());
            var result = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (result)
            {
                response = GetResponse<ListItemAllFieldsResponse>(result);
            }
            return response.StatusCode == (int)HttpStatusCode.OK;
        }

        public bool GetFilesByServerRelativePath(string relativePath, out SharepointResponse response, string filter = null)
        {
            //$expand=ListItemAllFields
            //$select=ListItemAllFields/File_x0020_Type or $select=Name,
            //$filter=(ListItemAllFields/File_x0020_Type eq 'xlsx')
            const string url = "_api/Web/GetFolderByServerRelativePath(decodedurl={0})/Files";
            var filterUrl = "";
            if (!string.IsNullOrWhiteSpace(filter))
            {
                _executor.Delegate.TracingService.Trace(string.Format(url, $"'{relativePath}'") + $"?$filter={filter}");
                filterUrl = $"?$filter={HttpUtils.UrlEncode(filter)}";
            }
            else
            {
                _executor.Delegate.TracingService.Trace(string.Format(url, $"'{relativePath}'"));
            }
            var request = GetRequest(HttpMethod.Get, new Uri(_sharePointUrl + string.Format(url, HttpUtils.UrlEncode($"'{relativePath}'")) + filterUrl), _accessToken.Invoke());
            var result = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (result)
            {
                response = GetResponse<FilesResponse>(result);
            }
            return response.StatusCode == (int)HttpStatusCode.OK;
        }

        public bool CheckFolderByServerRelativePath(string relativePath, out SharepointResponse response)
        {
            const string url = "_api/Web/GetFolderByServerRelativePath(decodedurl={0})/ListItemAllFields";
            _executor.Delegate.TracingService.Trace(string.Format(url, $"'{relativePath}'"));
            var request = GetRequest(HttpMethod.Get, new Uri(_sharePointUrl + string.Format(url, HttpUtils.UrlEncode($"'{relativePath}'"))), _accessToken.Invoke());
            var result = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (result)
            {
                response = GetResponse<ListItemAllFieldsResponse>(result);
            }
            return response.StatusCode == (int)HttpStatusCode.OK;
        }

        public bool CreateFolderByServerRelativePath(string relativePath, string digest, out SharepointResponse response)
        {
            const string url = "_api/Web/Folders/add({0})";
            _executor.Delegate.TracingService.Trace(string.Format(url, $"'{relativePath}'"));
            var request = GetRequest(HttpMethod.Post, new Uri(_sharePointUrl + string.Format(url, HttpUtils.UrlEncode($"'{relativePath}'"))), _accessToken.Invoke());
            request.Headers.Add(HttpUtils.XRequestDigest, digest);
            request.Content = new StringContent("");
            request.Content.Headers.ContentType.CharSet = HttpUtils.Utf8;
            request.Content.Headers.ContentType.MediaType = HttpUtils.ApplicationJson;
            request.Content.Headers.ContentLength = 0;
            var result = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (result)
            {
                response = GetResponse<FolderResponse>(result);
            }
            return response.StatusCode == (int)HttpStatusCode.OK;
        }

        public bool GetSharePointFolderId(string relativePath, out SharepointResponse response)
        {
            const string url = "_api/Web/GetFolderByServerRelativePath(decodedurl={0})/ListItemAllFields/Id";
            _executor.Delegate.TracingService.Trace(string.Format(url, $"'{relativePath}'"));
            var request = GetRequest(HttpMethod.Get, new Uri(_sharePointUrl + string.Format(url, HttpUtils.UrlEncode($"'{relativePath}'"))), _accessToken.Invoke());
            var result = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (result)
            {
                response = GetResponse<FolderIdResponse>(result);
            }
            return response.StatusCode == (int)HttpStatusCode.OK;
        }

        public bool GetListTypeName(string title, out SharepointResponse response)
        {
            const string url = "_api/web/lists/GetByTitle({0})?$select=ListItemEntityTypeFullName";
            _executor.Delegate.TracingService.Trace(string.Format(url, $"'{title}'"));
            var request = GetRequest(HttpMethod.Get, new Uri(_sharePointUrl + string.Format(url, HttpUtils.UrlEncode($"'{title}'"))), _accessToken.Invoke());
            var result = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (result)
            {
                response = GetResponse<ListItemEntityTypeResponse>(result);
            }
            return response.StatusCode == (int)HttpStatusCode.OK;
        }

        public bool UpdateSharePointItem(string title, int itemId, string listItemEntityTypeFullName, string columnName, string columnValue, string digest, out SharepointResponse response)
        {
            const string url = "_api/web/lists/GetByTitle({0})/items({1})";
            _executor.Delegate.TracingService.Trace(string.Format(url, $"'{title}'", itemId));
            var request = GetRequest(HttpMethod.Post, new Uri(_sharePointUrl + string.Format(url, HttpUtils.UrlEncode($"'{title}'"), itemId)), _accessToken.Invoke());
            request.Headers.Add(HttpUtils.XRequestDigest, digest);
            request.Headers.Add("X-HTTP-Method", "MERGE");
            request.Headers.Add("If-Match", "*");
            var content = $"{{ '__metadata': {{ 'type': '{listItemEntityTypeFullName}' }}, '{columnName}':'{columnValue}'}}";
            request.Content = new StringContent(content);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json;odata=verbose");
            request.Content.Headers.ContentType.CharSet = HttpUtils.Utf8;
            request.Content.Headers.ContentLength = Encoding.UTF8.GetBytes(content).Length;
            var result = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (result)
            {
                response = GetResponse<NoContentResponse>(result);
            }
            return response.StatusCode == (int)HttpStatusCode.NoContent;
        }

        public bool BreakRoleInheritance(string title, int itemId, string digest, out SharepointResponse response)
        {
            const string url = "_api/web/lists/GetByTitle({0})/items({1})/breakroleinheritance(copyRoleAssignments=false,clearSubscopes=false)";
            _executor.Delegate.TracingService.Trace(string.Format(url, $"'{title}'", itemId));
            var request = GetRequest(HttpMethod.Post, new Uri(_sharePointUrl + string.Format(url, HttpUtils.UrlEncode($"'{title}'"), itemId)), _accessToken.Invoke());
            request.Headers.Add(HttpUtils.XRequestDigest, digest);
            request.Content = new StringContent("");
            request.Content.Headers.ContentType.CharSet = HttpUtils.Utf8;
            request.Content.Headers.ContentType.MediaType = HttpUtils.ApplicationJson;
            request.Content.Headers.ContentLength = 0;
            var result = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (result)
            {
                response = GetResponse<NullResponse>(result);
            }
            return response.StatusCode == (int)HttpStatusCode.OK;
        }

        public bool GetSiteUser(string user, out SharepointResponse response)
        {
            //For SharePoint Online or on - premise forms authentication, use the below login name format:
            //          i:0#.f|membership|user@domain.com
            //For SharePoint 2013 on - premise Windows authentication, use the below login name format:
            //          i:0#.w|domain\user
            //For SharePoint 2013 on - premise SAML based authentication, use the below login name format: 
            //          i:05:t| adfs with roles| user@domain.com
            const string url = "_api/web/siteusers(@v)?@v={0}";
            var spUser = $"'i:0#.f|membership|{user}'";
            _executor.Delegate.TracingService.Trace(string.Format(url, spUser));
            var request = GetRequest(HttpMethod.Get, new Uri(_sharePointUrl + string.Format(url, HttpUtils.UrlEncode(spUser))), _accessToken.Invoke());
            var result = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (result)
            {
                response = GetResponse<UserResponse>(result);
            }
            return response.StatusCode == (int)HttpStatusCode.OK;
        }

        public bool EnsureSiteUser(string user, string digest, out SharepointResponse response)
        {
            //For SharePoint Online or on - premise forms authentication, use the below login name format:
            //          i:0#.f|membership|user@domain.com
            //For SharePoint 2013 on - premise Windows authentication, use the below login name format:
            //          i:0#.w|domain\user
            //For SharePoint 2013 on - premise SAML based authentication, use the below login name format: 
            //          i:05:t| adfs with roles| user@domain.com
            const string url = "_api/web/ensureuser";
            var spUser = $"i:0#.f|membership|{user}";
            _executor.Delegate.TracingService.Trace("{0}", url + ", Payload:" + spUser);
            var request = GetRequest(HttpMethod.Post, new Uri(_sharePointUrl + url), _accessToken.Invoke());
            request.Headers.Add(HttpUtils.XRequestDigest, digest);
            var content = $"{{ 'logonName': '{spUser}'}}";
            request.Content = new StringContent(content);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json;odata=verbose");
            request.Content.Headers.ContentType.CharSet = HttpUtils.Utf8;
            request.Content.Headers.ContentLength = Encoding.UTF8.GetBytes(content).Length;
            var result = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (result)
            {
                response = GetResponse<UserResponse>(result);
            }
            return response.StatusCode == (int)HttpStatusCode.OK;
        }

        public bool GetSiteGroup(string group, out SharepointResponse response)
        {
            const string url = "_api/web/sitegroups/getbyname({0})";
            _executor.Delegate.TracingService.Trace(string.Format(url, $"'{group}'"));
            var request = GetRequest(HttpMethod.Get, new Uri(_sharePointUrl + string.Format(url, HttpUtils.UrlEncode($"'{group}'"))), _accessToken.Invoke());
            var result = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (result)
            {
                response = GetResponse<GroupResponse>(result);
            }
            return response.StatusCode == (int)HttpStatusCode.OK;
        }

        public bool GetRoleDefinition(string roleDefinition, out SharepointResponse response)
        {
            const string url = "_api/web/roledefinitions/getbyname({0})";
            _executor.Delegate.TracingService.Trace(string.Format(url, $"'{roleDefinition}'"));
            var request = GetRequest(HttpMethod.Get, new Uri(_sharePointUrl + string.Format(url, HttpUtils.UrlEncode($"'{roleDefinition}'"))), _accessToken.Invoke());
            var result = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (result)
            {
                response = GetResponse<RoleDefinitionResponse>(result);
            }
            return response.StatusCode == (int)HttpStatusCode.OK;
        }

        public bool RoleAssignment(string title, int itemId, int principalId, int roleDefId, string digest, out SharepointResponse response)
        {
            const string url = "_api/web/lists/GetByTitle({0})/items({1})/roleassignments/addroleassignment(principalid={2},roleDefId={3})";
            _executor.Delegate.TracingService.Trace("{0}", string.Format(url, $"'{title}'", itemId, principalId, roleDefId));
            var request = GetRequest(HttpMethod.Post, new Uri(_sharePointUrl + string.Format(url, HttpUtils.UrlEncode($"'{title}'"), itemId, principalId, roleDefId)), _accessToken.Invoke());
            request.Headers.Add(HttpUtils.XRequestDigest, digest);
            request.Content = new StringContent("");
            request.Content.Headers.ContentType.CharSet = HttpUtils.Utf8;
            request.Content.Headers.ContentType.MediaType = HttpUtils.ApplicationJson;
            request.Content.Headers.ContentLength = 0;
            var result = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (result)
            {
                response = GetResponse<NullResponse>(result);
            }
            return response.StatusCode == (int)HttpStatusCode.OK;
        }

        public bool UploadFileByServerRelativePath(string relativePath, string fileName, ByteArrayContent fileContent, string digest, out SharepointResponse response, bool overwrite = true)
        {
            const string url = "_api/web/GetFolderByServerRelativeUrl({0})/Files/add(url={1}, overwrite={2})";
            _executor.Delegate.TracingService.Trace("{0}", string.Format(url, $"'{relativePath}'", $"'{fileName}'", overwrite.ToString().ToLower()));
            var request = GetRequest(HttpMethod.Post, new Uri(_sharePointUrl + string.Format(url, HttpUtils.UrlEncode($"'{relativePath}'"), HttpUtils.UrlEncode($"'{fileName}'"), overwrite.ToString().ToLower())), _accessToken.Invoke());
            request.Content = fileContent;
            request.Headers.Add(HttpUtils.XRequestDigest, digest);
            request.Headers.Add("binaryStringRequestBody", "true");
            var result = HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            using (request)
            using (result)
            {
                response = GetResponse<ListItemAllFieldsResponse>(result);
            }
            return response.StatusCode == (int)HttpStatusCode.OK;
        }

        private static HttpRequestMessage GetRequest(HttpMethod method, Uri uri, string accessToken)
        {
            var request = new HttpRequestMessage(method, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(HttpUtils.Bearer, accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpUtils.ApplicationJson));
            request.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue(HttpUtils.Utf8));
            request.Properties["RequestTimeout"] = TimeSpan.FromMilliseconds(Timeout);
            return request;
        }

        private SharepointResponse GetResponse<T>(HttpResponseMessage response) where T : ISharepointPayload
        {
            var json = response.Content.ReadAsStringAsync().Result;
            _executor.Delegate.TracingService.Trace(json);
            SharepointResponse result;
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    result = new SharepointResponse
                    {
                        StatusCode = (int)response.StatusCode,
                        Payload = _executor.SerializerService.JsonDeserialize<T>(json)
                    };
                    break;
                case HttpStatusCode.NoContent:
                    result = new SharepointResponse
                    {
                        StatusCode = (int)response.StatusCode,
                        Payload = default(T)
                    };
                    break;
                case HttpStatusCode.BadRequest:
                    {
                        if (!string.IsNullOrEmpty(json) && json.Contains("odata.error"))
                        {
                            result = new SharepointResponse
                            {
                                StatusCode = (int)response.StatusCode,
                                Payload = _executor.SerializerService.JsonDeserialize<ErrorResponse>(json)
                            };
                        }
                        else
                        {
                            result = new SharepointResponse
                            {
                                StatusCode = (int)response.StatusCode,
                                Payload = _executor.SerializerService.JsonDeserialize<BadRequestResponse>(json)
                            };
                        }
                        break;
                    }
                case HttpStatusCode.NotFound:
                    {
                        if (!string.IsNullOrEmpty(json) && json.Contains("odata.error"))
                        {
                            result = new SharepointResponse
                            {
                                StatusCode = (int)response.StatusCode,
                                Payload = _executor.SerializerService.JsonDeserialize<ErrorResponse>(json)
                            };
                        }
                        else if (!string.IsNullOrEmpty(json) && (json.StartsWith("<!DOCTYPE html") || json.StartsWith("<?xml")))//html/xml returned, nasty but true
                        {
                            result = new SharepointResponse
                            {
                                StatusCode = (int)response.StatusCode,
                                Payload = new HtmlOrXmlResponse
                                {
                                    HtmlOrXml = json
                                }
                            };
                        }
                        else
                        {
                            result = new SharepointResponse
                            {
                                StatusCode = (int)response.StatusCode,
                                Payload = new UnknownResponse
                                {
                                    Content = _executor.SerializerService.JsonDeserialize<Dictionary<string, object>>(json)
                                }
                            };
                        }
                        break;
                    }
                default:
                    {
                        if (string.IsNullOrEmpty(json))
                        {
                            result = new SharepointResponse
                            {
                                StatusCode = (int)response.StatusCode,
                                Payload = new UnknownResponse
                                {
                                    Content = new Dictionary<string, object>()
                                }
                            };
                        }
                        else if (!string.IsNullOrEmpty(json) && (json.StartsWith("<!DOCTYPE html") || json.StartsWith("<?xml")))//html/xml returned, nasty but true
                        {
                            result = new SharepointResponse
                            {
                                StatusCode = (int)response.StatusCode,
                                Payload = new HtmlOrXmlResponse
                                {
                                    HtmlOrXml = json
                                }
                            };
                        }
                        else
                        {
                            result = new SharepointResponse
                            {
                                StatusCode = (int)response.StatusCode,
                                Payload = new UnknownResponse
                                {
                                    Content = _executor.SerializerService.JsonDeserialize<Dictionary<string, object>>(json)
                                }
                            };
                        }
                        break;
                    }
            }
            return result;
        }

        private SharepointResponse GetBinaryResponse<T>(HttpResponseMessage response) where T : ISharepointPayload
        {
            if (response.IsSuccessStatusCode)
            {
                var stream = new MemoryStream();

                response.Content.CopyToAsync(stream);
                stream.Position = 0;

                _executor.Delegate.TracingService.Trace("Data retrieved.");

                var answer = new SharepointResponse
                {
                    StatusCode = (int)response.StatusCode,
                    Payload = new BinaryDataResponse
                    {
                        Stream = stream
                    }
                };

                return answer;
            }

            return GetResponse<T>(response);
        }
    }
}
