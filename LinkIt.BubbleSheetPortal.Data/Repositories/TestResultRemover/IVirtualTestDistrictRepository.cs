using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.GenesisGradeBook;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover
{
    public interface IVirtualTestDistrictRepository : IReadOnlyRepository<VirtualTestDistrict>
    {
        IQueryable<VirtualTestDistrict> GetVirtualTestDistricts(int districtId, int userId, int roleId, int? schoolId,
            int? teacherId, int? classId, int? studentId, int isRegrader);
        IQueryable<VirtualTestDistrict> GetVirtualTestDistrictsForRegrader(int districtId, int userId, int roleId, int schoolId, int? termId);
        IQueryable<ClassDistrict> GetClassDistrictByRole(int districtId, int userId, int roleId, int? virtualtestId, int? studentId, int? SchoolId, int? teacherId, int isRegrader);
        IQueryable<ClassDistrict> GetClassDistrictByRoleHasTestResult(int districtId, int userId, int roleId, int schoolId, int termId);
        IQueryable<TermDistrict> GetTermDistrictByRoleForRegrader(int districtId, int userId, int roleId, int? classId, int schoolId);
        IQueryable<StudentTestResultDistrict> GetStudentDistrictByRole(int districtId, int userId, int roleId,
            int? schoolId, int? teacherId, int? classId, int? virtualTestId, int isRegrader);
        IQueryable<SchoolTestResultDistrict> GetSchoolDistrictByRole(int districtId, int userId, int roleId,
          int? teacherId, int? classId, int? studentId, int? virtualTestId, int isRegrader);

        IQueryable<SchoolTestResultDistrict> GetSchoolDistrictByRoleReGrader(int districtId, int userId, int roleId);

        IQueryable<TeacherTestResultDistrict> GetPrimaryTeacherDistrictByRole(int districtId, int userId, int roleId,
            int? schoolid, int? classId, int? studentId, int? virtualTestId, int isRegrader);

        IQueryable<DisplayTestResultDistrict> GetTestResutDistrictProcByRole(int districtId, int userId, int roleId,
            int? schoolid, string teacherName, int? classId, string studentName, int? virtualTestId, int termId,
            string testPeriod ,int pageIndex, int pageSize, ref int? totalRecords, string sortColumns, int isRegrader, string generalSearch);

        IQueryable<TermDistrict> GetTermDistrictByRole(int districtId, int userId, int roleId, int virtualTestId, int studentId, int classId, int schoolId, int teacherId, int isRegrader);
        IQueryable<TestResultLog> GetTestResultDetails(string testresultIds);
        IQueryable<TestResultScoreLog> GetTestResultScores(string testresultIds);
        IQueryable<TestResultSubScoreLog> GetTestResultSubScores(string listTestResultScoreId);
        IQueryable<TestResultProgramLog> GetTestResultProgram(string testresultIds);
        IQueryable<AnswerLog> GetAnswersByTestResultId(string testresultIds);
        IQueryable<AnswerSubLog> GetAnswerSubsByAnswerId(string listAnswerId);

        IQueryable<DisplayTestResultDistrictCustom> GetTestResutRetaggedProcByRole(TestResultDataModel model, ref int? totalRecords);

        IQueryable<TermDistrict> GetDistrictTermValidFilterByRole(int districtId, int userId, int roleId,
            int virtualTestId, int studentId, int classId, int schoolId, int teacherId);

        IQueryable<TermDistrict> GetFullDistrictTermValidFilterByRole(int districtId, int userId, int roleId,
            int virtualTestId, int studentId, int classId, int schoolId, int teacherId);
        IQueryable<TermDistrict> GetFullDistrictTermValidFilterByRoleV2(int districtId, int userId, int roleId,
            int virtualTestId, int studentId, int classId, string schoolIds, int teacherId);

        IQueryable<DisplayTestResultDistrictCustom> GetRemoveTestResutProcByRole(RemoveTestResultsDetail input, ref int? totalRecords);
        IQueryable<DisplayTestResultDistrictCustomV2> GetRemoveTestResutProcByRoleV2(RemoveTestResultsDetailV2 input);
        IQueryable<DisplayVirtualTestDistrictCustomV2> GetRemoveVirtualTestProcByRoleV2(RemoveVirtualTestDetailV2 input, ref int? totalRecords, ref int? totalStudents, ref int? totalTestResults);

        IQueryable<ListItem> GetTestBySchoolAndDistrictTerm(int districtId, int schoolId, int districtTermId, int userId,
            int roleId);

        IQueryable<ListItem> GetAllSubjects();

        #region ValidTerm

        IQueryable<VirtualTestDistrict> GetVirtualTestValidTermByDistrict(int districtId, int userId, int roleId,
            int? schoolId, int? teacherId, int? classId, int? studentId);

        IQueryable<TeacherTestResultDistrict> GetPrimaryTeacherValidTermByDistrictAndRole(int districtId, int userId, int roleId,
            int? schoolid, int? classId, int? studentId, int? virtualTestId);

        IQueryable<ClassDistrict> GetClassValidTermByDistrictRole(int districtId, int userId, int roleId,
            int? virtualtestId, int? studentId, int? schoolId, int? teacherId);
        IQueryable<StudentTestResultDistrict> GetStudentValidTermByDistrictAndRole(int districtId, int userId, int roleId,
            int? schoolId, int? teacherId, int? classId, int? virtualTestId);

        IQueryable<SchoolTestResultDistrict> GetSchoolValidTermByDistrictAndRole(int districtId, int userId, int roleId,
             int? teacherId, int? classId, int? studentId, int? virtualTestId);

        IQueryable<SchoolTestResultDistrict> GetSchoolValidTermByDistrictForRemoveTestResults(int districtId, int userId, int roleId,
             int? teacherId, int? classId, int? studentId, int? virtualTestId);

        bool? IsExistAutoGradingQueueBeingGraded(string testResultIds);
      

        #endregion
    }
}
