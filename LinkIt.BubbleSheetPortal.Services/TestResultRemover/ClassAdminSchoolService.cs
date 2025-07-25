using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Services.TestResultRemover
{
    public class ClassAdminSchoolService
    {
        private readonly IReadOnlyRepository<ClassAdminSchool> repository;

        public ClassAdminSchoolService(IReadOnlyRepository<ClassAdminSchool> repository)
        {
            this.repository = repository;
        }

        public List<int> GetClassIdsByAdminSchoolUserId(int userId)
        {
            return repository.Select().Where(o => o.UserId == userId).Select(o=>o.ClassId).ToList();
        } 
    }
}
