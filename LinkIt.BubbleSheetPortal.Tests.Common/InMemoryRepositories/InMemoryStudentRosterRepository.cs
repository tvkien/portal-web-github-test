using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryStudentRosterRepository : IReadOnlyRepository<StudentRoster>
    {
        private List<StudentRoster> table;

        public InMemoryStudentRosterRepository()
        {
            table = AddStudentRosters();
        }

        private List<StudentRoster> AddStudentRosters()
        {
            return new List<StudentRoster>
                       {
                           new StudentRoster{ BubbleSheetId = 1, RosterPosition = 1, StudentId = 1, Ticket = "ticket"},
                           new StudentRoster{ BubbleSheetId = 1, RosterPosition = 2, StudentId = 2, Ticket = "ticket"},
                           new StudentRoster{ BubbleSheetId = 1, RosterPosition = 3, StudentId = 3, Ticket = "ticket"},
                           new StudentRoster{ BubbleSheetId = 1, RosterPosition = 4, StudentId = 4, Ticket = "ticket"}
                       };
        }

        public IQueryable<StudentRoster> Select()
        {
            return table.AsQueryable();
        }
    }
}
