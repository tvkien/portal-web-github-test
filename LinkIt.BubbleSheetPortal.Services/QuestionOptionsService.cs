using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QuestionOptionsService
    {
        private readonly IReadOnlyRepository<QuestionOptions> repository; 

        public QuestionOptionsService(IReadOnlyRepository<QuestionOptions> repository)
        {
            this.repository = repository;
        }

        public IQueryable<QuestionOptions> GetQuestionOptionsByTestId(int testId)
        {
            return repository.Select()
                .Where(x => x.TestId.Equals(testId))
                .OrderBy(x => x.ProblemNumber);
        }
    }
}
