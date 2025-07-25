using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Enum;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class XLIIconAccessRepository : IXLIIconAccessRepository
    {
        private readonly Table<XLIIconAccessView> table;
        private readonly DbDataContext _dbDataContext;

        public XLIIconAccessRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<XLIIconAccessView>();
            _dbDataContext = DbDataContext.Get(connectionString);
        }

        public IQueryable<XLIIconAccess> Select()
        {
            return table.Select(x => new XLIIconAccess()
            {
                AllModules = x.AllModules,
                IconCode = x.Code,
                AreaSchoolEndDate = x.AreaSchoolEndDate,
                AreaSchoolExprires = x.AreaSchoolExpires,
                AreaSchoolStartDate = x.AreaSchoolStartDate,
                DistrictId = x.DistrictID,
                DistrictSchoolRestrict = x.DistrictSchoolRestrict,
                EndDate = x.EndDate,
                Expires = x.Expires,
                Restrict = x.Restrict,
                SchoolDistrictId = x.SchoolDistrictID,
                SchoolId = x.SchoolID,
                StartDate = x.StartDate,
                XliAreaId = x.XLIAreaID,
                AllRoles = x.AllRoles,
                RoleID = x.RoleID
            });
        }

        public List<XliArea> GetAllXliAreasByUserId(int userId, int districtId, int userRoleId)
        {
            var v = _dbDataContext.XLIIconAccessProc(userId, districtId, userRoleId).ToList();
            if (v.Count > 0)
            {
                var xliAreas = v.Select(o => new XliArea()
                {
                    XliAreaId = o.XLIAreaID,
                    Code = o.Code
                });
                if (userRoleId.In((int)RoleEnum.Student, (int)RoleEnum.Parent))
                {
                    xliAreas = xliAreas.Where(c => c.Code != ContaintUtil.Home);
                }
                return xliAreas.ToList();
            }
                

            return new List<XliArea>();
        }
    }
}
