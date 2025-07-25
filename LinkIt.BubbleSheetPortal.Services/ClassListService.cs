using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ClassListService
    {
        private readonly IReadOnlyRepository<ClassList> repository;

        public ClassListService(IReadOnlyRepository<ClassList> repository)
        {
            this.repository = repository;
        }

        public IQueryable<ClassList> GetClassListByPrimaryTeacherID(int teacherID)
        {
            return repository.Select().Where(c => c.UserId.Equals(teacherID));
        }

        public IQueryable<ClassList> GetClassListBySchoolID(int schoolID)
        {
            return repository.Select().Where(c => c.SchoolID.Equals(schoolID));
        }

        public IQueryable<ClassList> Select()
        {
            return repository.Select();
        }

        public IQueryable<ClassList> GetClassListByTeacherID(int teacherID)
        {
            return repository.Select().Where(c => c.UserId.Equals(teacherID));
        }
    }
}
