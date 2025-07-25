using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserGroup;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class XLIAreaGroupRepository : IXLIAreaGroupRepository
    {
        private readonly UserDataContext dbContext;
        private readonly Table<XLIAreaGroup> table;

        public XLIAreaGroupRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = UserDataContext.Get(connectionString);
            table = dbContext.GetTable<XLIAreaGroup>();
        }

        public void Save(XLIAreaGroupDto item)
        {
            var entity = table.FirstOrDefault(x => x.XLIGroupID == item.XLIGroupID && x.XLIAreaID == item.XLIAreaID);
            if (entity == null)
            {
                entity = new XLIAreaGroup
                {
                    XLIAreaID = item.XLIAreaID,
                    XLIGroupID = item.XLIGroupID
                };

                table.InsertOnSubmit(entity);
                table.Context.SubmitChanges();
            }

            item.XLIAreaGroupID = entity.XLIAreaGroupID;
        }
    }
}
