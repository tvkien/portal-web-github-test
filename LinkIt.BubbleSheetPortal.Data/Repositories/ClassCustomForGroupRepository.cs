using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ClassCustomForGroupRepository : IReadOnlyRepository<ClassCustomForGroup>
    {
        private readonly Table<ClassCustomForGroupView> table;

        public ClassCustomForGroupRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<ClassCustomForGroupView>();
        }

        public IQueryable<ClassCustomForGroup> Select()
        {
            return table.Select(x => new ClassCustomForGroup
            {
                                    Id   = x.ClassID,
                                    UserId = x.UserID,
                                    DistrictTermId = x.DistrictTermID,
                                    SchoolId = x.SchoolID,
                                    TeacherLastName = x.NameLast,
                                    TeacherFirstName = x.NameFirst,
                                    DistrictTermName = x.DistrictTermName,
                                    Name = x.Name,
                                    SchoolName = x.SchoolName
                                });
        }
    }
}
