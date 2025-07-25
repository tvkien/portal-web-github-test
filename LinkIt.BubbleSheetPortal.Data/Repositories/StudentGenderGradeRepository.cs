using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class StudentGenderGradeRepository : IReadOnlyRepository<StudentGenderGrade>, IStudentGradeRepository
    {
        private readonly Table<StudentGenderGradeView> table;
        private readonly StudentDataContext dbContext;

        public StudentGenderGradeRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = StudentDataContext.Get(connectionString);
            table = dbContext.GetTable<StudentGenderGradeView>();
        }

        public IQueryable<StudentGenderGrade> Select()
        {
            return table.Select(x => new StudentGenderGrade
            {
                StudentId = x.StudentID,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Status = x.Status,
                DistrictId = x.DistrictID,
                Code = x.Code,
                Gender = x.GenderName,
                Grade = x.GradeName,
                AdminSchoolID = x.AdminSchoolID
            });
        }

        public List<StudentProgram> GetProgramsStudent(int districtId, int userId, int roleId)
        {
            return
                dbContext.ManageClassGetProgramStudent(districtId, userId, roleId)
                    .Select(
                        x =>
                            new StudentProgram()
                            {
                                StudentID = x.StudentID,
                                ProgramID = x.ProgramID,
                                ProgramName = x.NAME
                            })
                    .ToList();
        }

        public List<StudentGrade> GetGradesStudent(int districtId, int userId, int roleId)
        {
            return
                dbContext.ManageClassGetGradesStudent(districtId, userId, roleId)
                    .Select(x => new StudentGrade() { StudentID = x.StudentID, GradeID = x.GradeID, GradeName = x.NAME, Order = x.Order })
                    .ToList();
        }



        public List<StudentGenderGrade> ManageParentGetStudentsAvailableByFilter(int districtId, string programIdList,
            string gradeIdList, bool showInactive, int userId, int roleId)
        {
            return
                dbContext.ManageParentGetStudentsAvailableByFilter(districtId, programIdList, gradeIdList, showInactive, userId,
                    roleId).Select(x => new StudentGenderGrade()
                    {
                        StudentId = x.StudentID ?? 0,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Code = x.Code,
                        Gender = x.Gender,
                        Grade = x.Grade
                    }).ToList();
        }
        public List<StudentGenderGrade> GetStudentsAvailableByFilter(int districtId, string programIdList,
            string gradeIdList, bool showInactive, int userId, int roleId)
        {
            return
                dbContext.GetStudentsAvailableByFilter(districtId, programIdList, gradeIdList, showInactive, userId,
                    roleId).Select(x => new StudentGenderGrade()
                    {
                        StudentId = x.StudentID ?? 0,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Code = x.Code,
                        Gender = x.Gender,
                        Grade = x.Grade
                    }).ToList();
        }

        public void AddManyStudentsToClass(int classId, string studentIdList)
        {
            dbContext.AddManyStudentsToClass(classId, studentIdList);
        }
    }
}
