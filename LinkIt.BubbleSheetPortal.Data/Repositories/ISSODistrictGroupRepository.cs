using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.SSO;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ISSODistrictGroupRepository: IRepository<SSODistrictGroup>
    {
        SSODistrictGroup GetByDistrictID(int districtID, string type = "auth0");
        SSODistrictGroup GetByTenantID(int tenantID, string type = "auth0");
        List<Models.District> GetListDistrictSameSSO(string subdomain);
    }
}
