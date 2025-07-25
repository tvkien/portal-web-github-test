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
    public class SchoolClassStudentRepository : IReadOnlyRepository<SchoolClassStudent>
    {
        private readonly Table<SchoolClassStudentView> table;

        public SchoolClassStudentRepository(string connectionString)
        {
            table = TestDataContext.Get(connectionString).GetTable<SchoolClassStudentView>();
        }

        public IQueryable<SchoolClassStudent> Select()
        {
            return table.Select(x => new SchoolClassStudent
                                {
                                    ClassId = x.ClassID,
                                    StudentId = x.StudentID,
                                    SchoolId = x.SchoolID,
                                    UserSchoolAdminId = x.UserSchoolAdminID
                                });
        }
    }
}
