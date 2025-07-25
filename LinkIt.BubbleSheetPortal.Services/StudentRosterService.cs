using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class StudentRosterService
    {
        private readonly IReadOnlyRepository<StudentRoster> repository;

        public StudentRosterService(IReadOnlyRepository<StudentRoster> repository)
        {
            this.repository = repository;
        }

        public int GetStudentRosterPositionByTicketAndStudentId(string ticket, int studentId, int classId)
        {
            var studentRoster = repository.Select().FirstOrDefault(x => x.Ticket.Equals(ticket) && x.StudentId.Equals(studentId) && x.ClassId.Equals(classId));

            return studentRoster.IsNull() ? 0 : studentRoster.RosterPosition;
        }
    }
}