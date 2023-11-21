using D365.Extension.DataAccess;
using D365.Extension.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace D365.Extension.Core
{
  public partial class Executor
    {
        protected DataAccessor DataAccessor(IOrganizationService organizationService,
            MergeOption mergeOption = MergeOption.NoTracking)
        {
            var dataContext = new DataContext(organizationService)
            {
                MergeOption = mergeOption
            };
            return new DataAccessor(dataContext);
        }
    }
}