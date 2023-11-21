// ReSharper disable once CheckNamespace

using System;

namespace D365.Extension.Core
{
    public interface ISharepointService
    {
        /// <summary>
        /// Init the service
        /// </summary>
        /// <param name="sharePointUrl"></param>
        /// <param name="accessToken"></param>
        ISharepointService Init(string sharePointUrl, Func<string> accessToken);

        /// <summary>
        /// API: _api/contextinfo
        /// </summary>
        /// <param name="response"></param>
        /// <returns>is successful (expected result/http status code)</returns>
        bool GetDigest(out SharepointResponse response);

        /// <summary>
        /// API: _api/web/getfilebyserverrelativeurl({0})/$value
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="response"></param>
        /// <returns>is successful (expected result/http status code)</returns>
        bool GetBinaryByServerRelativePath(string relativePath, out SharepointResponse response);

        /// <summary>
        /// API: _api/Web/getfilebyserverrelativeurl({0})/ListItemAllFields
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="response"></param>
        /// <returns>is successful (expected result/http status code)</returns>
        bool GetFileByServerRelativePath(string relativePath, out SharepointResponse response);

        /// <summary>
        /// API: _api/Web/GetFolderByServerRelativePath(decodedurl={0})/Files
        /// API: _api/Web/GetFolderByServerRelativePath(decodedurl={0})/Files?$filter={1}
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="response"></param>
        /// <param name="filter"></param>
        /// <returns>is successful (expected result/http status code)</returns>
        bool GetFilesByServerRelativePath(string relativePath, out SharepointResponse response, string filter = null);

        /// <summary>
        /// API: _api/Web/GetFolderByServerRelativePath(decodedurl={0})/ListItemAllFields
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="response"></param>
        /// <returns>is successful (expected result/http status code)</returns>
        bool CheckFolderByServerRelativePath(string relativePath, out SharepointResponse response);

        /// <summary>
        /// API: _api/Web/Folders/add({0})
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="digest"></param>
        /// <param name="response"></param>
        /// <returns>is successful (expected result/http status code)</returns>
        bool CreateFolderByServerRelativePath(string relativePath, string digest, out SharepointResponse response);

        /// <summary>
        /// API: _api/Web/GetFolderByServerRelativePath(decodedurl={0})/ListItemAllFields/Id
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="response"></param>
        /// <returns>is successful (expected result/http status code)</returns>
        bool GetSharePointFolderId(string relativePath, out SharepointResponse response);

        /// <summary>
        /// API: _api/web/lists/GetByTitle({0})?$select=ListItemEntityTypeFullName
        /// </summary>
        /// <param name="title"></param>
        /// <param name="response"></param>
        /// <returns>is successful (expected result/http status code)</returns>
        bool GetListTypeName(string title, out SharepointResponse response);

        /// <summary>
        /// API: _api/web/lists/GetByTitle({0})/items({1})
        /// </summary>
        /// <param name="title"></param>
        /// <param name="itemId"></param>
        /// <param name="listItemEntityTypeFullName"></param>
        /// <param name="columnName"></param>
        /// <param name="columnValue"></param>
        /// <param name="digest"></param>
        /// <param name="response"></param>
        /// <returns>is successful (expected result/http status code)</returns>
        bool UpdateSharePointItem(string title, int itemId, string listItemEntityTypeFullName, string columnName, string columnValue, string digest, out SharepointResponse response);

        /// <summary>
        /// API: _api/web/lists/GetByTitle({0})/items({1})/breakroleinheritance(copyRoleAssignments=false,clearSubscopes=false)
        /// </summary>
        /// <param name="title"></param>
        /// <param name="itemId"></param>
        /// <param name="digest"></param>
        /// <param name="response"></param>
        /// <returns>is successful (expected result/http status code)</returns>
        bool BreakRoleInheritance(string title, int itemId, string digest, out SharepointResponse response);

        /// <summary>
        /// API: _api/web/siteusers(@v)?@v={0}
        /// </summary>
        /// <param name="user"></param>
        /// <param name="response"></param>
        /// <returns>is successful (expected result/http status code)</returns>
        bool GetSiteUser(string user, out SharepointResponse response);

        /// <summary>
        /// API: _api/web/ensureuser
        /// </summary>
        /// <param name="user"></param>
        /// <param name="digest"></param>
        /// <param name="response"></param>
        /// <returns>is successful (expected result/http status code)</returns>
        bool EnsureSiteUser(string user, string digest, out SharepointResponse response);

        /// <summary>
        /// API: _api/web/sitegroups/getbyname({0})
        /// </summary>
        /// <param name="group"></param>
        /// <param name="response"></param>
        /// <returns>is successful (expected result/http status code)</returns>
        bool GetSiteGroup(string group, out SharepointResponse response);

        /// <summary>
        /// API: _api/web/roledefinitions/getbyname({0})
        /// </summary>
        /// <param name="roleDefinition"></param>
        /// <param name="response"></param>
        /// <returns>is successful (expected result/http status code)</returns>
        bool GetRoleDefinition(string roleDefinition, out SharepointResponse response);

        /// <summary>
        /// API: _api/web/lists/GetByTitle({0})/items({1})/roleassignments/addroleassignment(principalid={2},roleDefId={3})
        /// </summary>
        /// <param name="title"></param>
        /// <param name="itemId"></param>
        /// <param name="principalId"></param>
        /// <param name="roleDefId"></param>
        /// <param name="digest"></param>
        /// <param name="response"></param>
        /// <returns>is successful (expected result/http status code)</returns>
        bool RoleAssignment(string title, int itemId, int principalId, int roleDefId, string digest, out SharepointResponse response);
    }

    public class SharepointResponse
    {
        public int StatusCode { get; set; }

        public ISharepointPayload Payload { get; set; }
    }

    public interface ISharepointPayload
    {
    }
}
