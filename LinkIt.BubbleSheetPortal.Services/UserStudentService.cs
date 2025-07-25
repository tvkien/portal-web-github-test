using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class UserStudentService
    {
        private readonly IUserStudentRepository repository;

        public UserStudentService(IUserStudentRepository repository)
        {
            this.repository = repository;
        }

        public bool HasAccessStudentByLoginUser(int userId, int studentId)
        {
            return repository.Select().Any(x => x.UserID.Equals(userId) && x.StudentID.Equals(studentId));
        }

        public bool HasAccessStudentByUserAsTeacher(int userId, int studentId)
        {
            return repository.Select().Any(x => x.UserID.Equals(userId) && x.StudentID.Equals(studentId));
        }

        public IQueryable<UserStudent> GetUserStudentsBySchool(int schoolId)
        {
            return repository.Select().Where(u => u.SchoolID.Equals(schoolId));
        }

        public List<UserStudent> GetAvailableClassesBySchoolAndStudentId(int schoolId, int studentId, int? userId, int offSetRowCount, int fetchRowCount, string searchStr, string sortBy, string sortDirection)
        {
            return repository.GetAvailableClassesBySchoolAndStudentId(schoolId, studentId, userId, offSetRowCount, fetchRowCount, searchStr, sortBy, sortDirection).Select(x => new UserStudent
            {
                ClassID = x.ClassID,
                ClassName = x.ClassName,
                SchoolName = x.SchoolName,
                TeacherName = x.TeacherName,
                TermName = x.TermName,
                TotalCount = x.TotalCount
            }).ToList();
        }

        public IQueryable<UserStudent> GetAvailableClassesBySchoolAndStudentId(int studentId, int schoolId)
        {
            return repository.Select().Where(u => u.SchoolID == schoolId && u.StudentID != studentId && u.TeacherName != "legacy data");
        }

        public IQueryable<UserStudent> GetStudentsByStudentAndSchool(int studentId, int schoolId)
        {
            return repository.Select().Where(x => x.SchoolID.Equals(schoolId) && x.StudentID.Equals(studentId));
        }

        public IQueryable<UserStudent> GetStudentsByStudent(int studentId)
        {
            return repository.Select().Where(x => x.StudentID.Equals(studentId));
        }
    }
}
