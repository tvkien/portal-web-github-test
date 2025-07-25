using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class PasswordResetQuestionService
    {
        private readonly IReadOnlyRepository<PasswordResetQuestion> repository;

        public PasswordResetQuestionService(IReadOnlyRepository<PasswordResetQuestion> repository)
        {
            this.repository = repository;
        }

        public IQueryable<PasswordResetQuestion> GetPasswordResetQuestions(int roleId)
        {
            if (roleId == (int)Permissions.Student)
            {
                return repository.Select().Where(x => x.Type != null && x.Type == 2);
            }

            return repository.Select().Where(x => x.Type != null && x.Type == 1);
        }
    }
}