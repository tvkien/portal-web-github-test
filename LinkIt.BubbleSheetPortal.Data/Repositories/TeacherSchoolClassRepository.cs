using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TeacherSchoolClassRepository : IReadOnlyRepository<TeacherSchoolClass>
    {
        private readonly Table<TeacherSchoolClassView> table;

        public TeacherSchoolClassRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<TeacherSchoolClassView>();
        }

        public IQueryable<TeacherSchoolClass> Select()
        {
            return table.Select(x => new TeacherSchoolClass
                {
                    ClassId = x.ClassID,
                    SchoolId = x.SchoolID,
                    TeacherFirstName = x.NameFirst,
                    TeacherLastName = x.NameLast,
                    UserId = x.UserID,
                    UserName = x.UserName,
                    UserStatusId = x.UserStatusID
                    
                });
        }
    }
}