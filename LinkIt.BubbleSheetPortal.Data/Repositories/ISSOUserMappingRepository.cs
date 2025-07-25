using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.SSO;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ISSOUserMappingRepository: IRepository<SSOUserMapping>
    {
        SSOUserMappingEntity GetLinkitUserFromMapping(string adUsername, int districtId, string type);
        SSOUserMappingEntity GetLinkitUserFromMapping(string adUsername, int districtId, string type, IEnumerable<int> roleIds);
    }
}
