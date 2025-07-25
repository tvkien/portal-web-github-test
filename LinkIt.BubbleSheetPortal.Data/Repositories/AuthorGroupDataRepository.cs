using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AuthorGroupDataRepository : IReadOnlyRepository<AuthorGroupList>
    {
        private readonly Table<AuthorGroupListView> table;

        public AuthorGroupDataRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<AuthorGroupListView>();
        }

        public IQueryable<AuthorGroupList> Select()
        {
            return table.Select(x => new AuthorGroupList
                                     {
                                         Name = x.Name,
                                         Level = x.Level,
                                         SchoolId = x.SchoolID,
                                         StateId = x.StateID,
                                         DistrictId = x.DistrictID,
                                         AuthorGroupId = x.AuthorGroupID,
                                         DistrictName = x.Districts,
                                         SchoolName = x.Schools,
                                         UserId = x.UserID,
                                         UserNameList = x.Users
                                     });
        }
    }
}