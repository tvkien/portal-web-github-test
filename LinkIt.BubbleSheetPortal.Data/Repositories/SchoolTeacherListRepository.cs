using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SchoolTeacherListRepository : IReadOnlyRepository<SchoolTeacherList>
    {
        private readonly Table<SchoolTeacherListView> table;

        public SchoolTeacherListRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<SchoolTeacherListView>();
        }

        public IQueryable<SchoolTeacherList> Select()
        {
            return table.Select(x => new SchoolTeacherList
                {
                    ClassID = x.ClassID,
                    ClassName = x.ClassName,
                    FirstName = x.NameFirst,
                    LastName = x.NameLast,
                    SchoolID = x.SchoolID,
                    UserID = x.UserID,
                    UserName = x.UserName,
                    Active = x.Active,
                    RoleId = x.RoleID
                });
        }
    }
}