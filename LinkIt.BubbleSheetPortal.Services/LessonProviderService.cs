using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class LessonProviderService
    {
        private readonly IReadOnlyRepository<LessonProvider> repository;

        public LessonProviderService(IReadOnlyRepository<LessonProvider> repository)
        {
            this.repository = repository;
        }

        public IQueryable<LessonProvider> GetLessonProviders()
        {
            return repository.Select().OrderBy(x => x.Name);
        }

    }
}