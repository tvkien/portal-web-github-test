using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class StudentRosterRepository : IReadOnlyRepository<StudentRoster>
    {
        private readonly Table<TestStudentsWithRosterView> table;

        public StudentRosterRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<TestStudentsWithRosterView>();
        }

        public IQueryable<StudentRoster> Select()
        {
            return table.Select(x => new StudentRoster
                {
                    BubbleSheetId = x.BubblesheetID,
                    Ticket = x.Ticket,
                    RosterPosition = x.RosterPosition,
                    StudentId = x.StudentID,
                    ClassId = x.ClassID
                });
        }
    }
}