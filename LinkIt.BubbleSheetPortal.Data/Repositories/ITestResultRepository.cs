using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ITestResultRepository : IRepository<TestResult>
    {
        List<AssessmentItem> GetAssessmentItem(int districtId, string strTestResultIds);
        List<AssessmentAchievedDetail> GetAssessmentAchievedDetail(int districtId, string strTestResultIds);
        List<AssessmentItemResponse> GetAssessmentItemResponse(int districtId, string strTestResultIds);
        IEnumerable<string> GetAssessmentItemResponseString(int districtId, string strTestResultIds);
        List<AsmntItemrAcademicStds> GetAsmntItemrAcademicStds(int districtId, string strTestResultIds);
        IEnumerable<string> GetAsmntItemrAcademicStdsString(int districtId, string strTestResultIds);
        List<AsmntSubtestAcademicStds> GetAsmntSubtestAcademicStds(int districtId, string strTestResultIds);
        List<AssessmentAccModFact> GetAssessmentAccModFact(int districtId, string strTestResultIds);
        List<AssessmentFact> GetAssessmentFact(int districtId, string strTestResultIds);
        List<AssessmentResponse> GetAssessmentResponse(int districtId, string strTestResultIds);
        IEnumerable<string> GetAssessmentResponseString(int districtId, string strTestResultIds);

        List<int> GetExtractTestResultIDs(ExtractLocalCustom obj);
        List<ExtractTestResult> GetExtractTestResults(ExtractLocalCustom obj, int pageIndex, int pageSize, ref int? totalRecords, string sortColumns);
        
        bool TransferTestResultByClass(int schoolId, int districtTermId, int teacherId, int classId, int testResultId);
        List<ExtractUser> GetExtractUser(ExtractUserCustom obj);
        List<ExportTest> GetExtractTest(ExtractLocalCustom obj);
        List<QTITestClassAssignment> GetExtractTestAssignment(ExtractLocalCustom obj);
        List<ExportRosterData> GetExtractRoster(ExtractRosterCustom obj);
        List<int> GetAllClassIds(ExtractRosterCustom obj);
        void SaveExtractTestResultToQueue(ExtractLocalCustom obj, int type, string lstTemplates, int timezoneOffset, string baseHostURL, string lstIdsUncheck);
        void ReEvaluateBadge(int testResultID);
        void ReEvaluateBadgeV2(string testResultIds);
        bool RetagTestResults(RetagTestResult obj);
        void DeleteTestResultAndSubItemsV2(string testResultIds);
    }
}
