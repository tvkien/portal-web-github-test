using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System.Data.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ClassListRepository : IReadOnlyRepository<ClassList>
    {
        private readonly Table<ClassListView> table;

        public ClassListRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<ClassListView>();
        }

        public IQueryable<ClassList> Select()
        {
            return table.Select(x => new ClassList
                {
                    ClassId = x.ClassID,
                    IsLocked = x.IsLocked,
                    ClassName = x.ClassName,
                    TermName = x.TermName,
                    TermStartDate = x.TermStartDate,
                    TermEndDate = x.TermEndDate,
                    SchoolID = x.SchoolID,
                    UserId = x.UserID,
                    PrimaryTeacher = x.PrimaryTeacher,
                    SchoolName = x.SchoolName,
                    Students = x.Students,
                    Teachers = x.Teachers,
                    ModifiedBy = x.ModifiedBy,
                    ClassType = x.ClassType,
                    Subjects = x.Subjects
            });
        }
    }
}
