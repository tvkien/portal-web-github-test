using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ClassTypeService
    {
        private readonly IReadOnlyRepository<ClassType> repository;

        public ClassTypeService(IReadOnlyRepository<ClassType> repository)
        {
            this.repository = repository;
        }

        public IQueryable<ClassType> GetClassTypes()
        {
            return repository.Select();
        }
    }
}
