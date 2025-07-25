using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class StudentInClassRepository : IReadOnlyRepository<StudentInClass>
    {
        private readonly Table<StudentInClassView> table;

        public StudentInClassRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<StudentInClassView>();
        }

        public IQueryable<StudentInClass> Select()
        {
            return table.Select(x => new StudentInClass
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    MiddleName = x.MiddleName,
                    Gender = x.Gender,
                    Race = x.Race,
                    Code = x.Code,
                    StudentID = x.StudentID,
                    Active = x.Active.GetValueOrDefault(),
                    ClassID = x.ClassID,
                    ID = x.ClassStudentID,
                    GradeName = x.GradeName
                });
        }
    }
}