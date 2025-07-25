using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserGroup;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class XLIAreaGMRoleRepository : IXLIAreaGMRoleRepository
    {
        private readonly UserDataContext dbContext;
        private readonly Table<XLIAreaGMRole> table;

        public XLIAreaGMRoleRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = UserDataContext.Get(connectionString);
            table = dbContext.GetTable<XLIAreaGMRole>();
        }

        public void Add(params XLIAreaGMRoleDto[] items)
        {
            var entities = items.Select(x => new XLIAreaGMRole
            {
                RoleID = x.RoleID,
                XLIAreaGroupModuleID = x.XLIAreaGroupModuleID
            });

            table.InsertAllOnSubmit(entities);
            table.Context.SubmitChanges();
        }

        public bool Delete(int xliGroupId, int xliModuleId)
        {
            var entities = table.Where(x => x.XLIAreaGroupModule.XLIAreaGroup.XLIGroupID == xliGroupId && x.XLIAreaGroupModule.XLIModuleID == xliModuleId).ToArray();

            if (entities.Length > 0)
            {
                table.DeleteAllOnSubmit(entities);
                table.Context.SubmitChanges();
            }

            return true;
        }
    }
}
