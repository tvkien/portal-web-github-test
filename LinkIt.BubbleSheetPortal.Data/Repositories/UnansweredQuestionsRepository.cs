using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using System.Globalization;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class UnansweredQuestionsRepository : IUansweredQuestionsRepository<UnansweredQuestion>
    {
        private readonly Table<UnansweredQuestionsView> testResultsTable;
        private readonly Table<VirtualQuestionAnswerView> questionAnswersTable;
        private readonly TestDataContext testDataContext;

        public UnansweredQuestionsRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            testDataContext = TestDataContext.Get(connectionString);
            testResultsTable = testDataContext.GetTable<UnansweredQuestionsView>();
            questionAnswersTable = testDataContext.GetTable<VirtualQuestionAnswerView>();
        }

        public IQueryable<UnansweredQuestion> SelectQuestionsWithResults()
        {
            return testResultsTable.Select(x => new UnansweredQuestion
                {
                    QuestionId = x.VirtualQuestionID,
                    StudentId = x.StudentID,
                    BubbleSheetId = x.BubbleSheetID,
                    Ticket = x.Ticket,
                    QuestionOrder = x.QuestionOrder,
                    ClassId = x.ClassID.GetValueOrDefault(),
                    QTISchemaId = x.QTISchemaID,
                    PointsPossible = x.PointsPossible,
                    AnswerIdentifiers = x.AnswerIdentifiers,

                    IsMultiMarkQuestion = x.BubbleSheetErrorType != null && x.BubbleSheetErrorType == "M",

                    XmlContent = x.XmlContent,
                    CorrectLetter = DetermineAnswerLetter(x)
                });
        }
        
        private static string DetermineAnswerLetter(UnansweredQuestionsView x)
        {
            return (x.CorrectAnswer[0] < 'A' && x.QTISchemaID != 10 ? ((char)(x.CorrectAnswer[0] + 16)).ToString(CultureInfo.InvariantCulture) : x.CorrectAnswer).ToString(CultureInfo.InvariantCulture);
        }

        public IQueryable<UnansweredQuestion> SelectChoicesForQuestions()
        {
            return questionAnswersTable.Select(x => new UnansweredQuestion
                {
                    QuestionId = x.VirtualQuestionID,
                    StudentId = x.StudentID,
                    BubbleSheetId = x.BubblesheetID,
                    Ticket = x.Ticket,
                    QuestionOrder = x.QuestionOrder,
                    QTISchemaId = x.QTISchemaID,
                    PointsPossible = x.PointsPossible,
                    AnswerIdentifiers = x.AnswerIdentifiers,
                    IsMultiMarkQuestion = false, // always does not have answer
                    XmlContent = x.XmlContent,
                    ClassId = x.ClassID.GetValueOrDefault(),
                    CorrectLetter = DetermineAnswerLetter(x)
                });
        }


        public IQueryable<UnansweredQuestion> GetAllQuestionOfBubbleSheet(int studentID, string ticket, int classId)
        {
            return testDataContext.GetAllQuestionOfVirtualTest(ticket, classId, studentID)
                .Select(x => new UnansweredQuestion
                {
                    QuestionId = x.VirtualQuestionID.GetValueOrDefault(),
                    StudentId = studentID,
                    BubbleSheetId = x.BubblesheetID,
                    Ticket = x.Ticket,
                    QuestionOrder = x.QuestionOrder.GetValueOrDefault(),
                    QTISchemaId = x.QTISchemaID.GetValueOrDefault(),
                    PointsPossible = x.PointsPossible.GetValueOrDefault(),
                    AnswerIdentifiers = x.AnswerIdentifiers,
                    IsMultiMarkQuestion = x.BubbleSheetErrorType != null && x.BubbleSheetErrorType == "M",
                    XmlContent = x.XmlContent,
                    ClassId = x.ClassID.GetValueOrDefault(),
                    CorrectLetter = DetermineAnswerLetter(x)
                }).OrderBy(m=>m.QuestionOrder);
        }

        private static string DetermineAnswerLetter(GetAllQuestionOfVirtualTestResult x)
        {
            return (x.CorrectAnswer[0] < 'A' && x.QTISchemaID != 10 ? ((char)(x.CorrectAnswer[0] + 16)).ToString(CultureInfo.InvariantCulture) : x.CorrectAnswer).ToString(CultureInfo.InvariantCulture);
        }

        private static string DetermineAnswerLetter(VirtualQuestionAnswerView x)
        {
            return (x.CorrectAnswer[0] < 'A' && x.QTISchemaID != 10 ? ((char)(x.CorrectAnswer[0] + 16)).ToString(CultureInfo.InvariantCulture) : x.CorrectAnswer).ToString(CultureInfo.InvariantCulture);
        }
    }
}