using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryQuestionAnswersRepository : IReadOnlyRepository<QuestionOptions>
    {
        private readonly List<QuestionOptions> table = new List<QuestionOptions>();

        public InMemoryQuestionAnswersRepository()
        {
            table = AddQuestionAnswers();
        }

        private List<QuestionOptions> AddQuestionAnswers()
        {
            return new List<QuestionOptions>
                       {
                           new QuestionOptions{ TestId = 1, ProblemNumber = 1, AnswerIdentifiers = "1;2;3"},
                           new QuestionOptions{ TestId = 1, ProblemNumber = 2, AnswerIdentifiers = "1;2;3"},
                           new QuestionOptions{ TestId = 1, ProblemNumber = 3, AnswerIdentifiers = "1;2;3"},
                           new QuestionOptions{ TestId = 1, ProblemNumber = 4, AnswerIdentifiers = "1;2;3"},
                           new QuestionOptions{ TestId = 2, ProblemNumber = 1, AnswerIdentifiers = "1;2;3"},
                           new QuestionOptions{ TestId = 2, ProblemNumber = 2, AnswerIdentifiers = "1;2;3"},
                           new QuestionOptions{ TestId = 2, ProblemNumber = 3, AnswerIdentifiers = "1;2;3"},
                           new QuestionOptions{ TestId = 2, ProblemNumber = 4, AnswerIdentifiers = "1;2;3", IsOpenEndedQuestion = true, PointsPossible = 5}
                       };
        }

        public IQueryable<QuestionOptions> Select()
        {
            return table.AsQueryable();
        }
    }
}
