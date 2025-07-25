using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class LessonFileTypeService
    {
        private readonly IReadOnlyRepository<LessonFileType> repository;

        public LessonFileTypeService(IReadOnlyRepository<LessonFileType> repository)
        {
            this.repository = repository;
        }

        public IQueryable<LessonFileType> GetLessonFileType()
        {
            return repository.Select();
        }
        
    }
}