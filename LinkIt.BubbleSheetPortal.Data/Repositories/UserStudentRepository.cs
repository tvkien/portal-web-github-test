using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class UserStudentRepository : IUserStudentRepository
    {
        private readonly Table<UserStudentView> table;
        private readonly UserDataContext _userDataContext;
        public UserStudentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _userDataContext = UserDataContext.Get(connectionString);
            table = UserDataContext.Get(connectionString).GetTable<UserStudentView>();
        }

        public IQueryable<UserStudent> Select()
        {
            return table.Select(x => new UserStudent
            {
                UserID = x.UserID,
                SchoolID = x.SchoolID,
                ClassID = x.ClassID,
                TeacherID = x.TeacherID,
                StudentID = x.StudentID,
                ClassName = x.ClassName,
                SchoolName = x.SchoolName,
                TermName = x.TermName,
                TeacherName = x.TeacherName,
                ModifiedBy = x.ModifiedBy
            });
        }

        public List<GetAvailableClassesBySchoolAndStudentIdResult> GetAvailableClassesBySchoolAndStudentId(int schoolId, int studentId, int? userId, int offSetRowCount, int fetchRowCount, string searchStr, string sortBy, string sortDirection)
        {
            return _userDataContext.GetAvailableClassesBySchoolAndStudentId(schoolId, studentId, userId, offSetRowCount, fetchRowCount, searchStr, sortBy, sortDirection).ToList();
        }
    }
}
