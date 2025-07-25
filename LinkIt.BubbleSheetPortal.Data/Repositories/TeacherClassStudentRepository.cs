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
    public class TeacherClassStudentRepository : IReadOnlyRepository<TeacherClassStudent>
    {
        private readonly Table<TeacherClassStudentView> table;

        public TeacherClassStudentRepository(string connectionString)
        {
            table = TestDataContext.Get(connectionString).GetTable<TeacherClassStudentView>();
        }

        public IQueryable<TeacherClassStudent> Select()
        {
            return table.Select(x => new TeacherClassStudent
                                {
                                    ClassId = x.ClassID,
                                    ClassUserLOEId = x.ClassUserLOEID,
                                    StudentId = x.StudentID,
                                    UserId = x.UserID
                                });
        }
    }
}
