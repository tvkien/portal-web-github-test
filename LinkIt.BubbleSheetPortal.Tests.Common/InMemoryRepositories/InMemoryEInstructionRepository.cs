using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryEInstructionRepository : IEInstructionRepository
    {
        public InMemoryEInstructionRepository()
        {
        }

        public int RITCreateRequest(int userID, int districtID, string importedFileName, string requestParameter)
        {
            return 1;
        }

        public List<RITCreateRequestTestResponseResult> RITCreateRequestTestResponse(int requestID, string testData)
        {
            return new List<RITCreateRequestTestResponseResult>();
        }

        public int RITCreateVirtualTestQuestion(int requestID, string approveStudentResponse)
        {
            throw new System.NotImplementedException();
        }

        public List<RITGradeTestResponseResult> RITGradeTestResponse(int requestID, int virtualTestID, string aspproveStudentResponse)
        {
            return new List<RITGradeTestResponseResult>();
        }

        public bool UpdateStudentLocalCode(int requestID, string selectedVal, int? foundID)
        {
            return true;
        }

        public IQueryable<SchoolTeacher> GetTeachersBySchoolId(int schooID)
        {
            return new List<SchoolTeacher>().AsQueryable();
        }

        public IQueryable<TeacherDistrictTerm> GetTermsByUserIdAndSchoolId(int userId, int schoolId)
        {
            return new List<TeacherDistrictTerm>().AsQueryable();
        }

        public IQueryable<Class> GetClassesByDistrictTermIdAndUserId(int termId, int userId)
        {
            return new List<Class>().AsQueryable();
        }
    }
}
