using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryPasswordResetQuestionRepository : IReadOnlyRepository<PasswordResetQuestion>
    {
        private List<PasswordResetQuestion> table;

        public InMemoryPasswordResetQuestionRepository()
        {
            table = AddPasswordResetQuestions();
        }

        private List<PasswordResetQuestion> AddPasswordResetQuestions()
        {
            return new List<PasswordResetQuestion>
                       {
                           new PasswordResetQuestion{ Id = 1, Question = "Question 1" },
                           new PasswordResetQuestion{ Id = 2, Question = "Question 2" },
                           new PasswordResetQuestion{ Id = 3, Question = "Question 3" },
                           new PasswordResetQuestion{ Id = 4, Question = "Question 4" },
                           new PasswordResetQuestion{ Id = 5, Question = "Question 5" },
                       };
        }

        public IQueryable<PasswordResetQuestion> Select()
        {
            return table.AsQueryable();
        }
    }
}