using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BubbleSheetStudentResultsRepository : IBubbleSheetStudentResultsRepository
    {
        private readonly Table<BubbleSheetStudentResultsView> table;
        private readonly BubbleSheetDataContext _context;

        public BubbleSheetStudentResultsRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = BubbleSheetDataContext.Get(connectionString);
            table = _context.GetTable<BubbleSheetStudentResultsView>();
        }

        public IQueryable<BubbleSheetStudentResults> Select()
        {
            return table.Select(x => new BubbleSheetStudentResults
                                         {
                                             StudentId = x.StudentID,
                                             StudentName = x.StudentName,
                                             Status = x.Status,
                                             Ticket = x.Ticket,
                                             AnsweredCount = x.AnsweredCount,
                                             TotalCount = x.TotalCount,
                                             ClassId = x.ClassID,
                                             PointsEarned = x.PointsEarned,
                                             PointsPossible = x.PointsPossible
                                         });
        }

        public List<BubbleSheetStudentResults> GetStudentsOfBubbleSheetByTicket(string ticket, int classID)
        {
            var data = _context.fnGetStudentsOfBubbleSheetByTicket(ticket, classID).ToList();
            var result = data.Select(o => new BubbleSheetStudentResults
            {
                AnsweredCount = o.AnsweredCount,
                ClassId = o.ClassID,
                PointsEarned = o.PointsEarned.HasValue ? o.PointsEarned.Value : 0,
                PointsPossible = o.PointsPossible.HasValue ? o.PointsPossible.Value : 0,
                ProcessedPage = o.ProcessedPage,
                Status = o.Status,
                StudentId = o.StudentID.HasValue ? o.StudentID.Value : 0,
                StudentName = o.StudentName,
                Ticket = o.Ticket,
                TotalCount = o.TotalCount,
                BubbleSheetFinalStatus = BuildBBSFinalStatus(o.Status)
            }).ToList();

            return result;
        }

        public List<BubbleSheetStudentResults> GetStudentsOfBubbleSheetByTicketNoStatus(string ticket, int classID)
        {
            var data = _context.GetStudentOfBubbleSheetByTicketAndClassID(ticket, classID).ToList();
            var result = data.Select(o => new BubbleSheetStudentResults
            {
                AnsweredCount = o.AnsweredCount,
                ClassId = o.ClassID,
                PointsEarned = o.PointsEarned.HasValue ? o.PointsEarned.Value : 0,
                PointsPossible = o.PointsPossible.HasValue ? o.PointsPossible.Value : 0,
                ProcessedPage = o.ProcessedPage,
                Status = "",
                StudentId = o.StudentID.GetValueOrDefault(),
                StudentName = o.StudentName,
                Ticket = o.Ticket,
                TotalCount = o.TotalCount,
                BubbleSheetId = o.BubbleSheetID,
                RosterPosition = o.RosterPosition.GetValueOrDefault(),
                ArtifactFileName = o.ArtifactFileName,
            }).ToList();

            return result;
        }
        public List<BubbleSheetStudentResults> GetBubbleSheetStudentStatus(string studentIdList, string ticket, int classID)
        {
            var data = _context.GetBubbleSheetStudentStatus(studentIdList, ticket, classID).ToList();
            var result = data.Select(o => new BubbleSheetStudentResults
            {
                Status = o.Status.Length > 0 && o.Status[0] == '|' ? o.Status.Remove(0,1) : o.Status,
                StudentId = o.StudentID.HasValue ? o.StudentID.Value : 0,
                BubbleSheetFinalStatus = BuildBBSFinalStatus(o.Status.Length > 0 && o.Status[0] == '|' ? o.Status.Remove(0, 1) : o.Status)
            }).ToList();

            return result;
        }

        public List<string> GetBubbleSheetStudentStatusV2(string ticket, int classID)
        {
            return _context.GetBubbleSheetStudentStatusV2(ticket, classID)
                .Select(x => x.Status).ToList();
        }

        public BubbleSheetFinalStatus BuildBBSFinalStatus(string status)
        {
            var finalStatus = BubbleSheetFinalStatus.Review;
            var notGradeStatus = new List<string> { "Missing", "Unavailable" };
            var finishedStatus = new List<string> { "Auto", "Absent", "Confirmed", "Complete" };
            if (finishedStatus.Contains(status) || status.Contains("Confirmed") || status.Contains("Absent") || status.Contains("Complete"))
            {
                finalStatus = BubbleSheetFinalStatus.Finished;
            }
            else if (notGradeStatus.Contains(status))
            {
                finalStatus = BubbleSheetFinalStatus.NotGrade;
            }
            else
            {
                finalStatus = BubbleSheetFinalStatus.Review;
            }

            return finalStatus;
        }        
    }
}
