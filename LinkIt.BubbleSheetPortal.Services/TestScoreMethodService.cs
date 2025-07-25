using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TestScoreMethodService
    {
        private readonly IReadOnlyRepository<TestScoreMethod> repository;

        public TestScoreMethodService(IReadOnlyRepository<TestScoreMethod> repository)
        {
            this.repository = repository;
        }

        public IQueryable<TestScoreMethod> GetAll()
        {
            return repository.Select();
        }

        
    }
}