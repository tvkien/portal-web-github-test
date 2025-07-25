using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryUnansweredQuestionsRepository : IUansweredQuestionsRepository<UnansweredQuestion>
    {
        private readonly List<UnansweredQuestion> testResultsTable;
        private readonly List<UnansweredQuestion> questionsAnswersTable;

        public InMemoryUnansweredQuestionsRepository()
        {
            testResultsTable = AddTestResults();
            questionsAnswersTable = AddQuestionAnswers();
        }

        private List<UnansweredQuestion> AddTestResults()
        {
            return new List<UnansweredQuestion>
                       {
                           new UnansweredQuestion { StudentId = 10, BubbleSheetId = 123, Ticket = "Ticket2", QuestionId = 1, QuestionOrder = 1, AnswerIdentifiers = "A;B;C;D" },
                           new UnansweredQuestion { StudentId = 10, BubbleSheetId = 123, Ticket = "Ticket2", QuestionId = 2, QuestionOrder = 2, AnswerIdentifiers = "A;B;C;D" },
                           new UnansweredQuestion { StudentId = 10, BubbleSheetId = 123, Ticket = "Ticket2", QuestionId = 3, QuestionOrder = 7, AnswerIdentifiers = "A;B;C;D" },
                           new UnansweredQuestion { StudentId = 10, BubbleSheetId = 123, Ticket = "Ticket2", QuestionId = 4, QuestionOrder = 4, AnswerIdentifiers = "A;B;C;D" },
                           new UnansweredQuestion { StudentId = 10, BubbleSheetId = 123, Ticket = "Ticket2", QuestionId = 5, QuestionOrder = 8, AnswerIdentifiers = "A;B;C;D" },
                           new UnansweredQuestion { StudentId = 10, BubbleSheetId = 123, Ticket = "Ticket1", QuestionId = 6, QuestionOrder = 1, AnswerIdentifiers = "A;B;C;D" },
                           new UnansweredQuestion { StudentId = 10, BubbleSheetId = 123, Ticket = "Ticket1", QuestionId = 7, QuestionOrder = 6, AnswerIdentifiers = "A;B;C;D" },
                           new UnansweredQuestion { StudentId = 10, BubbleSheetId = 123, Ticket = "Ticket1", QuestionId = 8, QuestionOrder = 3, AnswerIdentifiers = "A;B;C;D" },
                           new UnansweredQuestion { StudentId = 10, BubbleSheetId = 123, Ticket = "Ticket1", QuestionId = 9, QuestionOrder = 7, AnswerIdentifiers = "A;B;C;D" },
                           new UnansweredQuestion { StudentId = 10, BubbleSheetId = 123, Ticket = "Ticket1", QuestionId = 9, QuestionOrder = 7, AnswerIdentifiers = "A;B;C;D" },
                           new UnansweredQuestion{ StudentId = 250, BubbleSheetId = 250, Ticket = "open-ended-ticket", ClassId = 250, QTISchemaId = 10, PointsPossible = 8, QuestionOrder = 1, QuestionId = 10},
                           new UnansweredQuestion{ StudentId = 250, BubbleSheetId = 250, Ticket = "open-ended-ticket", ClassId = 250, QTISchemaId = 10, PointsPossible = 8, QuestionOrder = 2, QuestionId = 11},
                           new UnansweredQuestion{ StudentId = 250, BubbleSheetId = 250, Ticket = "open-ended-ticket", ClassId = 250, QTISchemaId = 10, PointsPossible = 8, QuestionOrder = 3, QuestionId = 12},
                           new UnansweredQuestion{ StudentId = 250, BubbleSheetId = 250, Ticket = "open-ended-ticket", ClassId = 250, QTISchemaId = 10, PointsPossible = 8, QuestionOrder = 4, QuestionId = 13},
                       };
        }

        private List<UnansweredQuestion> AddQuestionAnswers()
        {
            return new List<UnansweredQuestion>
                       {
                           new UnansweredQuestion { StudentId = 10, Ticket = "Ticket1", QuestionOrder = 1, AnswerIdentifiers = "A;B;C;D", PointsPossible = 1, BubbleSheetId = 123, QuestionId = 5},
                           new UnansweredQuestion { StudentId = 10, Ticket = "Ticket1", QuestionOrder = 1, AnswerIdentifiers = "A;B;C;D", PointsPossible = 1, BubbleSheetId = 123, QuestionId = 7},
                           new UnansweredQuestion { StudentId = 10, Ticket = "Ticket1", QuestionOrder = 1, AnswerIdentifiers = "A;B;C;D", PointsPossible = 1, BubbleSheetId = 123, QuestionId = 9},
                           new UnansweredQuestion { StudentId = 10, Ticket = "Ticket1", QuestionOrder = 1, AnswerIdentifiers = "A;B;C;D", PointsPossible = 1, BubbleSheetId = 123, QuestionId = 23},
                           new UnansweredQuestion { StudentId = 10, Ticket = "Ticket2", QuestionOrder = 1, AnswerIdentifiers = "A;B;C;D", PointsPossible = 1, BubbleSheetId = 123, QuestionId = 26},
                           new UnansweredQuestion { StudentId = 10, Ticket = "Ticket2", QuestionOrder = 2, AnswerIdentifiers = "A;B;C;D", PointsPossible = 1, BubbleSheetId = 123, QuestionId = 74},
                           new UnansweredQuestion { StudentId = 10, Ticket = "Ticket2", QuestionOrder = 3, AnswerIdentifiers = "A;B;C;D", PointsPossible = 1, BubbleSheetId = 123, QuestionId = 85},
                           new UnansweredQuestion { StudentId = 10, Ticket = "Ticket2", QuestionOrder = 4, AnswerIdentifiers = "A;B;C;D", PointsPossible = 1, BubbleSheetId = 123, QuestionId = 123},
                           new UnansweredQuestion { StudentId = 9, BubbleSheetId = 123, Ticket = "ticket", ClassId = 123, QuestionId = 10, QuestionOrder = 8, AnswerIdentifiers = "A;B;C;D" },
                           new UnansweredQuestion { StudentId = 9, BubbleSheetId = 123, Ticket = "ticket", ClassId = 123, QuestionId = 11, QuestionOrder = 9, AnswerIdentifiers = "A;B;C;D" },
                           new UnansweredQuestion { StudentId = 9, BubbleSheetId = 123, Ticket = "ticket", ClassId = 123, QuestionId = 12, QuestionOrder = 10, AnswerIdentifiers = "A;B;C;D" },
                           new UnansweredQuestion { StudentId = 9, BubbleSheetId = 123, Ticket = "ticket", ClassId = 123, QuestionId = 13, QuestionOrder = 11, AnswerIdentifiers = "A;B;C;D" }
                       };
        }

        public IQueryable<UnansweredQuestion> SelectQuestionsWithResults()
        {
            return testResultsTable.AsQueryable();
        }

        public IQueryable<UnansweredQuestion> SelectChoicesForQuestions()
        {
            return questionsAnswersTable.AsQueryable();
        }
    }
}