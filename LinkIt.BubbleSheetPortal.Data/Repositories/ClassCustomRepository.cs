using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ClassCustomRepository : IReadOnlyRepository<ClassCustom>
    {
        private readonly Table<ClassCustomView> table;

        public ClassCustomRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<ClassCustomView>();
        }

        public IQueryable<ClassCustom> Select()
        {
            return table.Select(x => new ClassCustom
                                {
                                    Id   = x.ClassID,
                                    TermId = x.TermID,
                                    UserId = x.UserID,
                                    DistrictTermId = x.DistrictTermID,
                                    SchoolId = x.SchoolID,
                                    Locked = x.Locked,
                                    GradeId = x.GradeID,
                                    SISID = x.SISID,
                                    DistrictId = x.DistrictID,
                                    TeacherUserName = x.TeacherUserName,
                                    TeacherLastName = x.NameLast,
                                    TeacherFirstName = x.NameFirst,
                                    DistrictTermName = x.DistrictTermName,
                                    SchoolName = x.SchoolName,
                                    Period = x.Period,
                                    Course = x.Course,
                                    Section = x.Section,
                                    Code = x.Code,
                                    Name = x.Name,
                                    CourseNumber = x.CourseNumber
                                });
        }
    }
}
