using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TeacherDistrictTermRepository : ITeacherDistrictTermRepository
    {
        private readonly Table<TeacherDistrictTermView> table;
        private readonly TestDataContext _dbContext;

        public TeacherDistrictTermRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<TeacherDistrictTermView>();
            _dbContext = TestDataContext.Get(conn.GetLinkItConnectionString());
        }

        public IQueryable<TeacherDistrictTerm> Select()
        {
            return table.Select(x => new TeacherDistrictTerm
            {
                DistrictID = x.DistrictID,
                UserId = x.UserID,
                DistrictTermId = x.DistrictTermID,
                DistrictName = x.DistrictName,
                SchoolId = x.SchoolID ?? 0,
                UserStatusId = x.UserStatusID ?? 0
            });
        }

        public IEnumerable<TeacherDistrictTerm> GetTermBySchool(int schoolId, int userId, int roleId)
        {
            return _dbContext.GetTermBySchool(schoolId, userId, roleId)
                .Select(o => new TeacherDistrictTerm
                {
                    DistrictID = o.DistrictID.GetValueOrDefault(),
                    UserId = o.UserID.GetValueOrDefault(),
                    DistrictTermId = o.DistrictTermID.GetValueOrDefault(),
                    DistrictName = o.DistrictTermName,
                });
        }
    }
}
