using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using XLIGroupUser = LinkIt.BubbleSheetPortal.Models.XLIGroupUser;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class XLIGroupUserRepository : IXLIGroupUserRepository
    {
        private readonly Table<Entities.XLIGroupUser> table;
        private readonly UserDataContext dbContext;

        public XLIGroupUserRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = UserDataContext.Get(connectionString);
            table = dbContext.GetTable<Entities.XLIGroupUser>();
        }

        public IQueryable<XLIGroupUser> Select()
        {
            return table.Select(x => new XLIGroupUser
            {
                UserID = x.UserID,
                XLIGroupID = x.XLIGroupID,
                XLIGroupUserID = x.XLIGroupUserID
            });
        }

        public void RemoveUsersFromGroup(List<int> userIds)
        {
            var xliGroupUsers = table.Where(x => userIds.Contains(x.UserID)).ToList();
            if (xliGroupUsers.Count > 0)
            {
                table.DeleteAllOnSubmit(xliGroupUsers);
                table.Context.SubmitChanges();
            }
        }
    }
}
