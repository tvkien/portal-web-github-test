using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SchoolTeacherListService
    {
        private readonly IReadOnlyRepository<SchoolTeacherList> repository;

        public SchoolTeacherListService(IReadOnlyRepository<SchoolTeacherList> repository)
        {
            this.repository = repository;
        }

        public IQueryable<SchoolTeacherList> GetSchoolTeacherListBySchoolId(int schoolId)
        {
            return repository.Select().Where(x => x.SchoolID.Equals(schoolId));
        }

        public IQueryable<SchoolTeacherList> GetSchoolTeacherListByTeacherId(int userId)
        {
            return repository.Select().Where(x => x.UserID.Equals(userId));
        }
    }
}