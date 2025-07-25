using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class LessonContentTypeService
    {
        private readonly ILessonContentTypeRepository repository;

        public LessonContentTypeService(ILessonContentTypeRepository repository)
        {
            this.repository = repository;
        }

        public IQueryable<LessonContentType> GetLessonContentType()
        {
            return repository.Select();
        }
    }
}