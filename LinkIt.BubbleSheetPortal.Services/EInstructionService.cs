using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class EInstructionService
    {
        private readonly IEInstructionRepository _repository;

        public EInstructionService(IEInstructionRepository repository)
        {
            _repository = repository;
        }

        public int RITCreateRequest(int userID, int districtID, string importedFileName, string requestParameter)
        {
            return _repository.RITCreateRequest(userID, districtID, importedFileName, requestParameter);
        }

        public List<RITCreateRequestTestResponseResult> RITCreateRequestTestResponse(int requestID, string testData)
        {
            return _repository.RITCreateRequestTestResponse(requestID, testData);
        }

        public int RITCreateVirtualTestQuestion(int requestID, string approveStudentResponse)
        {
            return _repository.RITCreateVirtualTestQuestion(requestID, approveStudentResponse);
        }

        public List<RITGradeTestResponseResult> RITGradeTestResponse(int requestID, int virtualTestID, string aspproveStudentResponse)
        {
            return _repository.RITGradeTestResponse(requestID, virtualTestID, aspproveStudentResponse);
        }

        public bool UpdateStudentLocalCode(int requestID, string selectedVal, int? foundID)
        {
            return _repository.UpdateStudentLocalCode(requestID, selectedVal, foundID);
        }

        public IQueryable<SchoolTeacher> GetTeachersBySchoolId(int schoolId)
        {
            return _repository.GetTeachersBySchoolId(schoolId);
        }

        public IQueryable<TeacherDistrictTerm> GetTermsByUserIdAndSchoolId(int userId, int schoolId)
        {
            return _repository.GetTermsByUserIdAndSchoolId(userId, schoolId);
        }

        public IQueryable<Class> GetClassesByDistrictTermIdAndUserId(int termId, int userId, int schoolId)
        {
            return _repository.GetClassesByDistrictTermIdAndUserId(termId, userId, schoolId);
        }
    }
}
