using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SchoolClassStudentService
    {
        private readonly IReadOnlyRepository<SchoolClassStudent> repository;

        public SchoolClassStudentService(IReadOnlyRepository<SchoolClassStudent> repository)
        {
            this.repository = repository;
        }

        public List<int> GetListStudents(int userAdminSchoolId)
        {
            return repository.Select().Where(o => o.UserSchoolAdminId == userAdminSchoolId).Select(o=>o.StudentId).ToList();
        }
    }
}
