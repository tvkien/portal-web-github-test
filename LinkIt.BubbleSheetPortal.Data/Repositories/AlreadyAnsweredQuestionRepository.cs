using System;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AlreadyAnsweredQuestionRepository : IReadOnlyRepository<AlreadyAnsweredQuestion>
    {
        private readonly Table<AlreadyAnsweredQuestionView> table;

        public AlreadyAnsweredQuestionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<AlreadyAnsweredQuestionView>();
        }

        public IQueryable<AlreadyAnsweredQuestion> Select()
        {
            return table.Select(x => new AlreadyAnsweredQuestion
                                         {
                                             QuestionId = x.VirtualQuestionID,
                                             PointsPossible = x.PointsPossible,
                                             BubbleSheetId = x.BubbleSheetID,
                                             StudentId = x.StudentID,
                                             AnswerLetter = DetermineAnswerLetter(x),
                                             AnswerIdentifiers = x.AnswerIdentifiers,
                                             Ticket = x.Ticket,
                                             QuestionOrder = x.QuestionOrder,
                                             QTISchemaId = x.QTISchemaID,
                                             XmlContent = x.XmlContent
                                         });
        }

        private static string DetermineAnswerLetter(AlreadyAnsweredQuestionView x)
        {
            return (x.AnswerLetter[0] < 'A' && x.QTISchemaID != 10 ? ((char)(x.AnswerLetter[0] + 16)).ToString(CultureInfo.InvariantCulture) : x.AnswerLetter).ToString(CultureInfo.InvariantCulture);
        }
    }
}
