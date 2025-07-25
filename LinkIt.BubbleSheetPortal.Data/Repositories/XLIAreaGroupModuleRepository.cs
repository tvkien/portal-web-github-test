using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserGroup;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class XLIAreaGroupModuleRepository : IXLIAreaGroupModuleRepository
    {
        private readonly UserDataContext dbContext;
        private readonly Table<XLIAreaGroupModule> table;
        private readonly Table<XLIAreaGroup> xliAreaGroupTable;
        
        public XLIAreaGroupModuleRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = UserDataContext.Get(connectionString);
            table = dbContext.GetTable<XLIAreaGroupModule>();
            xliAreaGroupTable = dbContext.GetTable<XLIAreaGroup>();
        }

        public void Save(XLIAreaGroupModuleDto item)
        {
            var entity = table.FirstOrDefault(x => x.XLIAreaGroupID == item.XLIAreaGroupID && x.XLIModuleID == item.XLIModuleID);
            if (entity == null)
            {
                entity = new XLIAreaGroupModule
                {
                    XLIAreaGroupID = item.XLIAreaGroupID,
                    XLIModuleID = item.XLIModuleID,
                    AllRoles = item.AllRoles
                };

                table.InsertOnSubmit(entity);
                table.Context.SubmitChanges();
            }

            item.XLIAreaGroupModuleID = entity.XLIAreaGroupModuleID;
        }

        public bool Delete(int xliGroupId, int xliModuleId)
        {
            var entity = table.FirstOrDefault(x => x.XLIAreaGroup.XLIGroupID == xliGroupId && x.XLIModuleID == xliModuleId);

            int xliAreaGroupID = 0;

            if (entity != null)
            {
                xliAreaGroupID = entity.XLIAreaGroupID;

                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();

                var isExistAnyEntities = table.Any(x => x.XLIAreaGroupID == xliAreaGroupID);

                if (isExistAnyEntities)
                {
                    return true;
                }

                var xliAreaGroup = xliAreaGroupTable.FirstOrDefault(x => x.XLIAreaGroupID == xliAreaGroupID);
                if (xliAreaGroup != null)
                {
                    xliAreaGroupTable.DeleteOnSubmit(xliAreaGroup);
                    xliAreaGroupTable.Context.SubmitChanges();
                }
            }

            return true;
        }
    }
}
