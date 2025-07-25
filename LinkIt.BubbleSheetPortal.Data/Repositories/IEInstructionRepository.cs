using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IEInstructionRepository
    {
        int RITCreateRequest(int userID, int districtID, string importedFileName, string requestParameter);
        List<RITCreateRequestTestResponseResult> RITCreateRequestTestResponse(int requestID, string testData);
        int RITCreateVirtualTestQuestion(int requestID, string approveStudentResponse);
        List<RITGradeTestResponseResult> RITGradeTestResponse(int requestID, int virtualTestID, string aspproveStudentResponse);
        bool UpdateStudentLocalCode(int requestID, string selectedVal, int? foundID);
        IQueryable<SchoolTeacher> GetTeachersBySchoolId(int schooID);
        IQueryable<TeacherDistrictTerm> GetTermsByUserIdAndSchoolId(int userId, int schoolId);
        IQueryable<Class> GetClassesByDistrictTermIdAndUserId(int termId, int userId, int schoolId);
    }
}
