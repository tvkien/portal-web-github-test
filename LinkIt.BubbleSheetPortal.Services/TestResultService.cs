using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TestResultService
    {
        private readonly ITestResultRepository repository;

        public TestResultService(ITestResultRepository repository)
        {
            this.repository = repository;
        }
        public List<AssessmentItem> GetAssessmentItem(int districtId, string strTestResultIds)
        {
            return repository.GetAssessmentItem(districtId, strTestResultIds);
        }

        public List<AssessmentAchievedDetail> GetAssessmentAchievedDetail(int districtId, string strTestResultIds)
        {
            return repository.GetAssessmentAchievedDetail(districtId, strTestResultIds);
        }

        public List<AssessmentItemResponse> GetAssessmentItemResponse(int districtId, string strTestResultIds)
        {
            return repository.GetAssessmentItemResponse(districtId, strTestResultIds);
        }

        public IEnumerable<string> GetAssessmentItemResponseString(int districtId, string strTestResultIds)
        {
            return repository.GetAssessmentItemResponseString(districtId, strTestResultIds);
        }

        public List<int> GetExtractTestResultStudentIDs(ExtractLocalCustom obj)
        {
            return repository.GetExtractTestResultIDs(obj);
        }

        public List<ExtractTestResult> GetExtractTestResults(ExtractLocalCustom obj, int pageIndex, int pageSize,
            ref int? totalRecords, string sortColumns)
        {
            return repository.GetExtractTestResults(obj, pageIndex, pageSize, ref totalRecords, sortColumns);
        }

        public List<AsmntItemrAcademicStds> GetAsmntItemrAcademicStds(int districtId, string strTestResultIds)
        {
            return repository.GetAsmntItemrAcademicStds(districtId, strTestResultIds);
        }

        public IEnumerable<string> GetAsmntItemrAcademicStdsString(int districtId, string strTestResultIds)
        {
            return repository.GetAsmntItemrAcademicStdsString(districtId, strTestResultIds);
        }

        public List<AsmntSubtestAcademicStds> GetAsmntSubtestAcademicStds(int districtId, string strTestResultIds)
        {
            return repository.GetAsmntSubtestAcademicStds(districtId, strTestResultIds);
        }

        public List<AssessmentFact> GetAssessmentFact(int districtId, string strTestResultIds)
        {
            return repository.GetAssessmentFact(districtId, strTestResultIds);
        }

        public List<AssessmentResponse> GetAssessmentResponse(int districtId, string strTestResultIds)
        {
            return repository.GetAssessmentResponse(districtId, strTestResultIds);
        }

        public IEnumerable<string> GetAssessmentResponseString(int districtId, string strTestResultIds)
        {
            return repository.GetAssessmentResponseString(districtId, strTestResultIds);
        }

        public List<AssessmentAccModFact> GetAssessmentAccModFact(int districtId, string strTestResultIds)
        {
            return repository.GetAssessmentAccModFact(districtId, strTestResultIds);
        }

        /// <summary>
        /// UpdateTestResultByClass
        /// </summary>
        /// <param name="objClass"></param>
        /// <param name="lstTestResultIds"></param>
        /// <returns></returns>
        public bool UpdateTestResultByClass(Class objClass, List<int> lstTestResultIds)
        {
            if (lstTestResultIds.Count > 0)
            {
                foreach (int testResultId in lstTestResultIds)
                {
                    if (objClass != null && objClass.SchoolId.HasValue && objClass.UserId.HasValue &&
                        objClass.DistrictTermId.HasValue)
                    {
                        repository.TransferTestResultByClass(objClass.SchoolId.Value, objClass.DistrictTermId.Value,
                            objClass.UserId.Value, objClass.Id, testResultId);
                    }
                }
                return true;
            }
            return false;
        }

        public IQueryable<TestResult> GetTestResultByStudentId(int studentId)
        {
            return repository.Select().Where(en => en.StudentId == studentId);
        }

        public IQueryable<TestResult> GetTestResultByStudentIdList(int classId, List<int> studentIdList)
        {
            return repository.Select().Where(en => en.ClassId == classId && studentIdList.Contains(en.StudentId));
        }

        public IQueryable<TestResult> GetTestResultsByVirtualTestId(int virtualTestId)
        {
            return repository.Select().Where(o => o.VirtualTestId == virtualTestId);
        }

        public IQueryable<TestResult> GetTestResultsByVirtualTestIds(List<int> virtualTestIds, DateTime? resultDateFrom, DateTime? resultDateTo)
        {
            var data = repository.Select().Where(o => virtualTestIds.Contains(o.VirtualTestId));
            if (resultDateFrom.HasValue)
                data = data.Where(x => x.ResultDate >= resultDateFrom);

            if (resultDateTo.HasValue)
            {
                resultDateTo = Convert.ToDateTime(resultDateTo.Value.ToShortDateString()).AddDays(1).AddSeconds(-1);
                data = data.Where(x => x.ResultDate <= resultDateTo);
            }

            return data;
        }

        public TestResult GetTestResultById(int testResultId)
        {
            return repository.Select().FirstOrDefault(en => en.TestResultId == testResultId);
        }
        public TestResult GetTestResultByTestSessionId(int qtiOnlineTestSessionID)
        {
            return repository.Select().FirstOrDefault(en => en.QTIOnlineTestSessionID == qtiOnlineTestSessionID);
        }
        public List<ExtractUser> GetExtractUser(ExtractUserCustom obj)
        {
            return repository.GetExtractUser(obj);
        }

        public List<ExportTest> GetExtractTest(ExtractLocalCustom obj)
        {
            return repository.GetExtractTest(obj);
        }

        public List<QTITestClassAssignment> GetExtractTestAssignment(ExtractLocalCustom obj)
        {
            return repository.GetExtractTestAssignment(obj);
        }

        public List<ExportRosterData> GetExtractRoster(ExtractRosterCustom obj)
        {
            return repository.GetExtractRoster(obj);
        }

        public List<int> GetAllClassIds(ExtractRosterCustom obj)
        {
            return repository.GetAllClassIds(obj);
        }

        public IQueryable<TestResult> GetTestResults(int virtualTestId, int studentId)
        {
            return repository.Select().Where(o => o.VirtualTestId == virtualTestId && o.StudentId == studentId);
        }

        public void Save(TestResult item)
        {
            repository.Save(item);
        }

        public void SaveExtractTestResultToQueue(ExtractLocalCustom obj, int type, string lstTemplates, int timezoneOffset, string baseHostURL, string lstIdsUncheck)
        {
            repository.SaveExtractTestResultToQueue(obj, type, lstTemplates, timezoneOffset, baseHostURL, lstIdsUncheck);
        }

        public bool RetagTestResults(RetagTestResult obj)
        {
            return repository.RetagTestResults(obj);
        }

        public IQueryable<DateTime> GetResultDates(int virtualTestId, int classId)
        {
            return repository.Select().Where(o => o.VirtualTestId == virtualTestId && o.ClassId == classId).Select(x => x.ResultDate).Distinct();
        }
        public IQueryable<TestResult> GetStudentResultDates(int virtualTestId, int classId)
        {
           return repository.Select().Where(o => o.VirtualTestId == virtualTestId && o.ClassId == classId);
        }

        public IQueryable<DateTime> GetDateHasResultStudent(int virtualTestId, int studentId)
        {
            return repository.Select().Where(d => d.VirtualTestId == virtualTestId && d.StudentId == studentId).Select(d => d.ResultDate).Distinct();
        }
    }
}
