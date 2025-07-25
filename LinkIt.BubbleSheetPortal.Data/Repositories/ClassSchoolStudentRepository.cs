using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ClassSchoolStudentRepository : IReadOnlyRepository<ClassSchoolStudent>
    {
        private readonly Table<ClassSchoolStudentView> table;

        public ClassSchoolStudentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<ClassSchoolStudentView>();            
        }

        public IQueryable<ClassSchoolStudent> Select()
        {
            return table.Select(x => new ClassSchoolStudent()
            {
                ClassID = x.ClassID,
                FirstName = x.FirstName,
                LastName = x.LastName,
                SchoolID = x.SchoolID,
                StudentID = x.StudentID
            });
        }
    }
}