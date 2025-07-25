using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class DistrictTermClassRepository : IReadOnlyRepository<DistrictTermClass>
    {
        private readonly Table<DistrictTermClassView> table;

        public DistrictTermClassRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<DistrictTermClassView>();
        }

        public IQueryable<DistrictTermClass> Select()
        {
            return table.Select(x => new DistrictTermClass
            {
                DistrictId = x.DistrictID,
                DateStart = x.DateStart ?? DateTime.Now.AddYears(-20),
                DateEnd = x.DateEnd ?? DateTime.Now.AddYears(-20),
                ClassId = x.ClassID,
                DistrictTermId = x.DistrictTermID,
                SchoolId = x.SchoolID ?? 0,
                UserId = x.UserID ?? 0,
                TermId = x.TermID,
                TeacherId = x.TeacherID
            });
        }
    }
}