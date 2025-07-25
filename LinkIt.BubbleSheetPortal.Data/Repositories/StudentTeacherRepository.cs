using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class StudentTeacherRepository : IReadOnlyRepository<StudentTeacher>
    {
        private readonly Table<StudentTeacherView> table;

        public StudentTeacherRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<StudentTeacherView>();
        }

        public IQueryable<StudentTeacher> Select()
        {
            return table.Select(x => new StudentTeacher
                {
                    StudentId = x.StudentID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Status = x.Status,
                    DistrictId = x.DistrictID,
                    Code = x.Code,
                    Gender = x.GenderName,
                    Grade = x.GradeName,
                    ClassId = x.ClassID,
                    UserId = x.UserID,
                    AdminSchoolId = x.AdminSchoolID
                });
        }
    }
}
