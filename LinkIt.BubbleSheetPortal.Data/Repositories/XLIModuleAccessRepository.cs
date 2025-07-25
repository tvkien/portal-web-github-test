using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserGroup;
using LinkIt.BubbleSheetPortal.Models.Old.UserGroup;
using XLIModuleAccess = LinkIt.BubbleSheetPortal.Models.XLIModuleAccess;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class XLIModuleAccessRepository : IXLIModuleAccessRepository
    {
        private readonly Table<XLIModuleAccessView> table;
        private readonly DbDataContext _dbDataContext;

        public XLIModuleAccessRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<XLIModuleAccessView>();
            _dbDataContext = DbDataContext.Get(connectionString);
        }

        public IQueryable<XLIModuleAccess> Select()
        {
            return table.Select(x => new XLIModuleAccess()
            {
                XliModuleId = x.XLIModuleID,
                AreaCode = x.AreaCode,
                DMAllRoles = x.DMAllRole,
                DMDistrictId = x.DMDistrictID,
                DMEndDate = x.DMEndDate,
                DMExpires = x.DMExprires,
                DMRoleId = x.DMRoleID,
                DMStartDate = x.DMStartDate,
                DSMDistrictId = x.DSMDistrictID,
                DSMRestrict = x.DSMRestrict,
                SchoolId = x.SchoolID,
                ModuleAllRoles = x.ModuleAllRoles,
                ModuleCode = x.ModuleCode,
                ModuleRestrict = x.ModuleRestrict,
                SMAllRole = x.SMAllRoles,
                SMEndDate = x.SMEndDate,
                SMExprires = x.SMExpires,
                SMRoleId = x.SMRoleID,
                SMStartDate = x.SMStartDate
            });
        }

        public List<XliModule> GetAllModulesAccessByUser(int userId, int districtid, int userRoleId, string lstXliAreaIds)
        {
            var v = _dbDataContext.XLIModuleAccessProc(userId, districtid, userRoleId, lstXliAreaIds).ToList();
            if (v.Count > 0)
                return v.Select(o => new XliModule()
                {
                    XliAreaId = o.XLIAreaID,
                    XliModuleId = o.XLIModuleID,
                    Code = o.Code
                }).ToList();
            return new List<XliModule>();
        }

        public IEnumerable<XLIModuleAccessDto> GetXLIModuleAccessesByGroupID(int groupId, string xliModuleIds)
        {
            return _dbDataContext.GetModuleAccessByGroupID(groupId, xliModuleIds)
                    .Select(x => new XLIModuleAccessDto
                    {
                        AreaID = x.AreaID.Value,
                        AreaName = x.AreaName,
                        ModuleID = x.ModuleID.Value,
                        ModuleCode = x.ModuleCode,
                        ModuleName = x.ModuleName,
                        DistrictAccess = x.DistrictAccess,
                        SchoolAccess = x.SchoolAccess,
                        UserGroupAccess = x.UserGroupAccess,
                        CurrentAccess = x.CurrentAccess
                    });
        }

        public IEnumerable<SchoolAccess> GetSchoolAccessByModuleID(int moduleID, int districtID)
        {
            return _dbDataContext.GetSchoolAccessByModuleID(moduleID, districtID)
                .Select(x => new SchoolAccess
                {
                    SchoolName = x.SchoolName,
                    RoleAccess = x.RoleAccess
                }).ToList();
        }

        public IEnumerable<XLIModuleAccessSummaryDto> GetModuleSumaryAccess(int districtID, string xliModuleIds)
        {
            return _dbDataContext.GetModuleAccessSumary(districtID, xliModuleIds)
                .Select(x => new XLIModuleAccessSummaryDto
                {
                    AreaID = x.AreaID.Value,
                    AreaCode = x.AreaCode,
                    AreaName = x.AreaName,
                    ModuleID = x.ModuleID.Value,
                    ModuleCode = x.ModuleCode,
                    ModuleName = x.ModuleName,
                    DistrictAccess = x.DistrictAccess,
                    SchoolAccess = x.SchoolAccess,
                    UserGroupWithAccess = x.UserGroupWithAccess,
                    UserGroupWithoutAccess = x.UserGroupWithoutAccess
                });
        }

        public IEnumerable<GetReportingModulesDto> GetReportingModules(string env)
        {
            return _dbDataContext.fnGetUserGroupReportingModules(env)
                .Select(x => new GetReportingModulesDto
                {
                    Order = x.Order,
                    ModuleID = x.ModuleID,
                    ModuleCode = x.ModuleCode
                });
        }
    }
}
