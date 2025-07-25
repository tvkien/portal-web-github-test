using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserGroup;
using LinkIt.BubbleSheetPortal.Models.Old.UserGroup;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IXLIModuleAccessRepository : IReadOnlyRepository<XLIModuleAccess>
    {
        List<XliModule> GetAllModulesAccessByUser(int userId, int districtid, int userRoleId, string lstXliAreaIds);

        IEnumerable<XLIModuleAccessDto> GetXLIModuleAccessesByGroupID(int groupId, string xliModuleIds);

        IEnumerable<SchoolAccess> GetSchoolAccessByModuleID(int moduleID, int districtID);

        IEnumerable<XLIModuleAccessSummaryDto> GetModuleSumaryAccess(int districtID, string xliModuleIds);

        IEnumerable<GetReportingModulesDto> GetReportingModules(string env);
    }
}
