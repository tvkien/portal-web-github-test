using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BubbleSheetReviewDetailsRepository : IReadOnlyRepository<BubbleSheetReviewDetails>, IBubbleSheetReviewDetailsRepository
    {
        private readonly Table<BubbleSheetReviewDetailsView> table;
        private readonly BubbleSheetDataContext _context;
        public BubbleSheetReviewDetailsRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = BubbleSheetDataContext.Get(connectionString);
            table = _context.GetTable<BubbleSheetReviewDetailsView>();
        }

        public IQueryable<BubbleSheetReviewDetails> Select()
        {
            return table.Select(x => new BubbleSheetReviewDetails
                                    {
                                        BubbleSheetFileId = x.BubbleSheetFileID,
                                        BubbleSheetId = x.BubblesheetID,
                                        Ticket = x.Ticket,
                                        StudentId = x.StudentID,
                                        StudentName = x.StudentName,
                                        ClassId = x.ClassID,
                                        RosterPosition = x.RosterPosition,
                                        ClassName = x.ClassName,
                                        TeacherId = x.TeacherID,
                                        TeacherName = x.TeacherName,
                                        SchoolId = x.SchoolID,
                                        SchoolName = x.SchoolName,
                                        FileDisposition = x.FileDisposition,
                                        PageNumber = x.PageNumber,
                                        InputFileName = x.InputFileName,
                                        OutputFileName = x.OutputFileName,
                                        ResultCount = x.ResultCount,
                                        UploadedBy = x.UploadedBy,
                                        UploadedDate = x.UploadDate,
                                        CreatedByUserId = x.CreatedByUserID,
                                        IsManualEntry = x.IsManualEntry,
                                        VirtualTestId = x.VirtualTestID,
                                        ResultDate = x.ResultDate
                                    });
        }

        public List<BubbleSheetClassViewAnswer> GetBubbleSheetClassViewAnswerData(string studentIdList, string ticket, int classId)
        {
            return _context.BubbleSheetClassViewAnswerData(studentIdList, ticket, classId)
                .Select(x => new BubbleSheetClassViewAnswer()
                {
                    BubbleSheetId = x.BubblesheetID,
                    StudentId = x.StudentID,
                    VirtualQuestionId = x.VirtualQuestionID,
                    AnswerIdentifiers = x.AnswerIdentifiers,
                    CorrectAnswer = x.CorrectAnswer,
                    PointsPossible = x.PointsPossible,
                    QTISchemaId = x.QTISchemaID,
                    QuestionOrder = x.QuestionOrder,
                    Status = x.Status,
                    XmlContent = x.XmlContent,
                    AnswerLetter = x.AnswerLetter,
                    WasAnswered = x.WasAnswered ?? false
                }).ToList();
        }
    }
}
