using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryPreviouslyAnsweredQuestionsRepository : IReadOnlyRepository<PreviouslyAnsweredQuestion>
    {
        private readonly List<PreviouslyAnsweredQuestion> table = new List<PreviouslyAnsweredQuestion>();

        public InMemoryPreviouslyAnsweredQuestionsRepository()
        {
            table = AddPreviouslyAnsweredQuestions();
        }

        private List<PreviouslyAnsweredQuestion> AddPreviouslyAnsweredQuestions()
        {
            return new List<PreviouslyAnsweredQuestion>
                       {
                           new PreviouslyAnsweredQuestion{ AnswerID = 1, AnswerLetter = "A", BubbleSheetId = 1, QuestionOrder = 1, StudentId = 1, VirtualQuestionId = 1, WasAnswered = true},
                           new PreviouslyAnsweredQuestion{ AnswerID = 2, AnswerLetter = "B", BubbleSheetId = 1, QuestionOrder = 2, StudentId = 1, VirtualQuestionId = 2, WasAnswered = true},
                           new PreviouslyAnsweredQuestion{ AnswerID = 3, AnswerLetter = "A", BubbleSheetId = 1, QuestionOrder = 3, StudentId = 1, VirtualQuestionId = 3, WasAnswered = true},
                           new PreviouslyAnsweredQuestion{ AnswerID = 4, AnswerLetter = "C", BubbleSheetId = 1, QuestionOrder = 4, StudentId = 1, VirtualQuestionId = 4, WasAnswered = true},
                           new PreviouslyAnsweredQuestion{ AnswerID = 5, AnswerLetter = "D", BubbleSheetId = 1, QuestionOrder = 5, StudentId = 1, VirtualQuestionId = 5, WasAnswered = true},
                       };
        }

        public IQueryable<PreviouslyAnsweredQuestion> Select()
        {
            return table.AsQueryable();
        }
    }
}
