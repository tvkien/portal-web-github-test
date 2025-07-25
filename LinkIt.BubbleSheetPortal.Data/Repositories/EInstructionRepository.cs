using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class EInstructionRepository : IEInstructionRepository
    {
        private string _connectionString = null;

        public EInstructionRepository(IConnectionString conn)
        {
            _connectionString = conn.GetLinkItConnectionString();
        }

        public int RITCreateRequest(int userID, int districtID, string importedFileName, string requestParameter)
        {
            var result = DbDataContext.Get(_connectionString)
                                      .RITCreateRequest(userID, districtID, importedFileName, requestParameter)
                                      .FirstOrDefault();
            return result == null ? 0 : result.Column1;
        }

        public List<RITCreateRequestTestResponseResult> RITCreateRequestTestResponse(int requestID, string testData)
        {
            var result = DbDataContext.Get(_connectionString)
                                      .RITCreateRequestTestResponse(requestID, testData);
            return result.ToList();
        }

        public int RITCreateVirtualTestQuestion(int requestID, string approveStudentResponse)
        {
            var result = DbDataContext.Get(_connectionString)
                                     .RITCreateVirtualTestQuestion(requestID, approveStudentResponse).FirstOrDefault();
            return result == null ? 0 : result.VirtualTestID;
        }

        public List<RITGradeTestResponseResult> RITGradeTestResponse(int requestID, int virtualTestID, string aspproveStudentResponse)
        {
            var result = DbDataContext.Get(_connectionString)
                                      .RITGradeTestResponse(requestID, virtualTestID, aspproveStudentResponse);
            return result.ToList();
        }

        public bool UpdateStudentLocalCode(int requestID, string selectedVal, int? foundID)
        {
            var dbContext = DbDataContext.Get(_connectionString);
            var updateRequest = dbContext.RequestStudentResponses.Where(x => x.RequestStudentResponseID == requestID).FirstOrDefault();
            if (updateRequest != null)
            {
                updateRequest.StudentLocalCode = selectedVal;
                updateRequest.StudentExist = true;
            }
            if (foundID != null)
            {
                var inactiveRequest = dbContext.RequestStudentResponses.Where(x => x.RequestStudentResponseID == foundID).FirstOrDefault();
                if (inactiveRequest != null)
                {
                    inactiveRequest.StudentLocalCode = string.Empty;
                    inactiveRequest.StudentExist = false;
                }
            }
            dbContext.SubmitChanges();
            return true;
        }

        public IQueryable<SchoolTeacher> GetTeachersBySchoolId(int schooID)
        {
            return ClassSchoolTeacherStudentQuery()
                    .Where(x => x.SchoolID.Equals(schooID))
                    .Select(x => new SchoolTeacher()
                        {
                            UserId = x.UserID,
                            SchoolId = x.SchoolID,
                            FirstName = x.TeacherFirstName,
                            LastName = x.TeacherLastName,
                            TeacherName = x.TeacherName,
                            UserName = x.TeacherName,
                            RoleId = x.RoleID
                        }).Distinct();
        }

        public IQueryable<TeacherDistrictTerm> GetTermsByUserIdAndSchoolId(int userId, int schoolId)
        {
            return ClassSchoolTeacherStudentQuery()
                    .Where(x => x.UserID.Equals(userId) && x.SchoolID.Equals(schoolId))
                    .Select(x => new TeacherDistrictTerm()
                    {
                        UserId = x.UserID,
                        SchoolId = x.SchoolID,
                        DistrictTermId = x.DistrictTermID,
                        DistrictName = x.TermName                        
                    }).Distinct();
        }

        public IQueryable<Class> GetClassesByDistrictTermIdAndUserId(int termId, int userId, int schoolId)
        {
            return ClassSchoolTeacherStudentQuery()
                    .Where(x => x.UserID.Equals(userId) && x.DistrictTermID.Equals(termId) && x.SchoolID.Equals(schoolId))
                    .Select(x => new Class()
                    {
                        Id = x.ClassID,
                        Name = x.ClassName,
                        DistrictTermId = x.DistrictTermID,
                        SchoolId = x.SchoolID,                        
                    }).Distinct();
        }

        private System.Data.Linq.Table<ClassSchoolTeacherStudentView> ClassSchoolTeacherStudentQuery()
        {
            var dbContext = DbDataContext.Get(_connectionString);
            return dbContext.ClassSchoolTeacherStudentViews;
        }
    }
}
