using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryAlreadyAnsweredQuestionsRepository : IReadOnlyRepository<AlreadyAnsweredQuestion>
    {
        private readonly List<AlreadyAnsweredQuestion> table;

        public InMemoryAlreadyAnsweredQuestionsRepository()
        {
            table = AddAlreadyAnsweredQuestions();
        }

        private List<AlreadyAnsweredQuestion> AddAlreadyAnsweredQuestions()
        {
            return new List<AlreadyAnsweredQuestion>
                       {
                           new AlreadyAnsweredQuestion{StudentId = 10, BubbleSheetId = 123},
                           new AlreadyAnsweredQuestion{StudentId = 10, BubbleSheetId = 123},
                           new AlreadyAnsweredQuestion{StudentId = 10, BubbleSheetId = 123},
                           new AlreadyAnsweredQuestion{StudentId = 10, BubbleSheetId = 123},
                           new AlreadyAnsweredQuestion{StudentId = 250, BubbleSheetId = 250, QTISchemaId = 10, PointsPossible = 6, AnswerLetter = "3"},
                       };
        }

        public IQueryable<AlreadyAnsweredQuestion> Select()
        {
            return table.AsQueryable();
        }
    }
}