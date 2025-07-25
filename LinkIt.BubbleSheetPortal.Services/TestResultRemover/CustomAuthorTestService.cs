using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Services.TestResultRemover
{
    public class CustomAuthorTestService
    {    
        private readonly ICustomAuthorTestRepository testResultRepository;

        public CustomAuthorTestService(ICustomAuthorTestRepository testResultRepository)
        {   
            this.testResultRepository = testResultRepository;
        }  

        public List<GetAuthorBySchoolAdminResult> GetAuthorBySchoolAdmin(int schoolAdminId)
        {
            var result = testResultRepository.GetAuthorBySchoolAdmin(schoolAdminId);
            return result;
        }

        public List<AuthorTestProcResult> GetAuthorTestByDistrictId(int districtId, bool isRegrader)
        {
            var vResult = testResultRepository.GetAuthorTestByDistrictId(districtId);
            if(isRegrader)
            {
                return vResult.Where(o => o.VirtualTestSourceID != 3).ToList();
            }
            return vResult;
        }
    }
}
