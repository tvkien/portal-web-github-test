using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using XLIModuleAccess = LinkIt.BubbleSheetPortal.Models.XLIModuleAccess;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IXLIAreaDistrictModuleRepository {
        bool CheckDistrictRoleAccessModule(int userid, int moduleid);
    }
    public class XLIAreaDistrictModuleRepository : IXLIAreaDistrictModuleRepository
    {
        private readonly DbDataContext _dbDataContext;

        public XLIAreaDistrictModuleRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _dbDataContext = DbDataContext.Get(connectionString);
        }
        public bool CheckDistrictRoleAccessModule(int userid, int moduleid)
        {

            bool result = false;
            result = _dbDataContext.XLICheckDistrictRoleAccessModule(moduleid, userid) ==1?true:false;
            return result;
        }

    }
}
