using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class LessonSubjectService
    {
        private readonly ILessonSubjectRepository repository;

        public LessonSubjectService(ILessonSubjectRepository repository)
        {
            this.repository = repository;
        }

        public IQueryable<LessonSubject> GetLessonSubjects()
        {
            return repository.Select();
        }
        
    }
}