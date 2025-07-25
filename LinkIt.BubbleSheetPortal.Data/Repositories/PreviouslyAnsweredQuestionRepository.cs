using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class PreviouslyAnsweredQuestionRepository : IReadOnlyRepository<PreviouslyAnsweredQuestion>
    {
        private readonly Table<PreviouslyAnsweredQuestionView> table;

        public PreviouslyAnsweredQuestionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = TestDataContext.Get(connectionString);
            table = dataContext.GetTable<PreviouslyAnsweredQuestionView>();
        }

        public IQueryable<PreviouslyAnsweredQuestion> Select()
        {
            return table.Select(x => new PreviouslyAnsweredQuestion
                {
                    AnswerID = x.AnswerID,
                    AnswerLetter = x.AnswerLetter,
                    BubbleSheetId = x.BubbleSheetID,
                    StudentId = x.StudentID,
                    VirtualQuestionId = x.VirtualQuestionID,
                    QuestionOrder = x.QuestionOrder,
                    WasAnswered = x.WasAnswered
                });
        }
    }
}