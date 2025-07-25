using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Threading;
using AutoMapper;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories.Loaders;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TestResultRepository : ITestResultRepository
    {
        private readonly Table<TestResultEntity> table;
        private readonly TestDataContext dbContext;
        private readonly ExtractTestDataContext extractDbContext;
        private string ConnectionString;
        private const int MaximumThreads = 4;

        public TestResultRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<TestResultEntity>();
            dbContext = TestDataContext.Get(connectionString);
            extractDbContext = ExtractTestDataContext.Get(connectionString);
            Mapper.CreateMap<TestResult, TestResultEntity>();
            ConnectionString = connectionString;
        }

        public IQueryable<TestResult> Select()
        {
            return table.Select(x => new TestResult
            {
                TestResultId = x.TestResultID,
                VirtualTestId = x.VirtualTestID,
                StudentId = x.StudentID,
                TeacherId = x.TeacherID ?? 0,
                SchoolId = x.SchoolID ?? 0,
                ResultDate = x.ResultDate,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                TermId = x.TermID ?? 0,
                ClassId = x.ClassID ?? 0,
                GradedById = x.GradedByID,
                ScoreType = x.ScoreType,
                ScoreValue = x.ScoreValue ?? 0,
                TRData = x.TRData,
                SubmitType = x.SubmitType ?? 0,
                DistrictTermId = x.DistrictTermID ?? 0,
                UserId = x.UserID ?? 0,
                OriginalUserId = x.OriginalUserID ?? 0,
                UIN = x.UIN,
                LegacyBatchId = x.LegacyBatchID ?? 0,
                BubbleSheetId = x.BubbleSheetID ?? 0,
                QTIOnlineTestSessionID = x.QTIOnlineTestSessionID
            });
        }

        public void Save(TestResult item)
        {
            var entity = table.FirstOrDefault(x => x.TestResultID.Equals(item.TestResultId));

            if (entity.IsNull())
            {
                entity = new TestResultEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.TestResultId = entity.TestResultID;
        }

        public void Delete(TestResult item)
        {
            dbContext.DeleteTestResultAndSubItems(item.TestResultId);
        }

        public void DeleteTestResultAndSubItemsV2(string testResultIds)
        {
            dbContext.DeleteTestResultAndSubItemsV2(testResultIds);
        }

        public List<AssessmentItem> GetAssessmentItem(int districtId, string strTestResultIds)
        {
            var lst = new List<AssessmentItem>();
            var lstExportAssessmentItemResult = extractDbContext.ExportAssessmentItem_New(strTestResultIds, districtId).ToList();
            if (lstExportAssessmentItemResult.Count > 0)
            {
                lst.AddRange(lstExportAssessmentItemResult.Select(itemResult => new AssessmentItem()
                {
                    DistrictCode = itemResult.Code,
                    Year = itemResult.Year ?? 0,
                    TestName = itemResult.TestName,
                    SubjectName = itemResult.SubjectName,
                    BankName = itemResult.BankName,
                    GradeName = itemResult.GradeName,
                    DistrictTermName = itemResult.DistrictTermName,
                    SubtestHighestGradeLevel = itemResult.SubtestHighestGradeLevel,
                    SubtestLowestGradeLevel = itemResult.SubtestLowestGradeLevel
                }));
            }
            return lst;
        }

        public List<AssessmentAchievedDetail> GetAssessmentAchievedDetail(int districtId, string strTestResultIds)
        {
            var lst = new List<AssessmentAchievedDetail>();
            var lstExportAssessmentAchievedDetailResult = extractDbContext.ExportAssessmentAchievedDetail_New(strTestResultIds, districtId).ToList();
            if (lstExportAssessmentAchievedDetailResult.Count > 0)
            {
                lst.AddRange(lstExportAssessmentAchievedDetailResult.Select(itemResult => new AssessmentAchievedDetail()
                {
                    AchievelevelName = itemResult.AchieveLevelName,
                    AchievementLevel = itemResult.AchievementLevel.ToString(),
                    DistrictCode = itemResult.Code,
                    Year = itemResult.Year ?? 0,
                    TestName = itemResult.TestName
                }));
            }
            return lst;
        }

        public List<AssessmentItemResponse> GetAssessmentItemResponse(int districtId, string strTestResultIds)
        {
            var lst = new List<AssessmentItemResponse>();
            var lstExportAssessmentItemResponseResult = extractDbContext.ExportAssessmentItemResponse_New(strTestResultIds, districtId).ToList();
            if (lstExportAssessmentItemResponseResult.Count > 0)
            {
                lst.AddRange(lstExportAssessmentItemResponseResult.Select(itemResult => new AssessmentItemResponse()
                {
                    TestName = itemResult.TestName,
                    CorrectAnswer = itemResult.CorrectAnswer,
                    DistrictCode = itemResult.Code,
                    Number = itemResult.Number,
                    PointsPossible = itemResult.PointsPossible,
                    QtiSchemaId = itemResult.QtiSchemaId,
                    QuestionOrder = itemResult.QuestionOrder,
                    Year = itemResult.Year ?? 0,
                    ResponseLongType = itemResult.ResponseLongType
                }));
            }
            return lst;
        }

        public IEnumerable<string> GetAssessmentItemResponseString(int districtId, string strTestResultIds)
        {
            var testResultIds = strTestResultIds.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            var totalRecords = testResultIds.Length;
            var pageSize = totalRecords / MaximumThreads;
            var doneEvents = new ManualResetEvent[MaximumThreads];
            var services = new List<AssessmentItemResponseLoader>();
            for (var i = 0; i < MaximumThreads; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                var takeRecords = i == MaximumThreads - 1 ? int.MaxValue : pageSize;
                var partialTestResultIDs = testResultIds.Skip(i * pageSize).Take(takeRecords);
                var partialStrTestResultIDs = String.Join(";", partialTestResultIDs);
                var testExtractService = new AssessmentItemResponseLoader(ConnectionString, districtId, partialStrTestResultIDs, doneEvents[i]);
                services.Add(testExtractService);
                ThreadPool.QueueUserWorkItem(testExtractService.ThreadPoolCallback, i);
            }

            WaitHandle.WaitAll(doneEvents);

            IEnumerable<ExportAssessmentItemResponse_NewResult> testResults = new List<ExportAssessmentItemResponse_NewResult>();
            foreach (var service in services)
            {
                testResults = testResults.Union(service.Results);
            }

            const string template = "{0}\tLinkIt\t{1}\t{2}\t{3}\t{4}\t\t{5}\t\t{6}\t\t{7}\t\t\t\t\t\t\t\t\t\t\t{8}\t\t\t\t\t\t\t\t\t";
            var result = testResults.Select(item => string.Format(template,
                                        ValidData(item.Code, 8)
                                        , string.Format("{0}-06-30", item.Year)
                                        , ValidData(item.TestName, 200)
                                        , item.QuestionOrder <= 9 ? string.Format("0{0}", item.QuestionOrder) : item.QuestionOrder.ToString()
                                        , ValidData(item.Number, 50)
                                        , item.QtiSchemaId
                                        , item.PointsPossible >= 10 ? item.PointsPossible.ToString() : string.Format("0{0}", item.PointsPossible)
                                        , item.CorrectAnswer
                                        , item.ResponseLongType
                                        )).Distinct();

            return result;
        }

        public List<AsmntItemrAcademicStds> GetAsmntItemrAcademicStds(int districtId, string strTestResultIds)
        {
            var lst = new List<AsmntItemrAcademicStds>();
            var lstExportAssessmentItemResponseResult = extractDbContext.Export_ASMNT_ITEMR_ACADEMIC_STDS_New(strTestResultIds, districtId).ToList();
            if (lstExportAssessmentItemResponseResult.Count > 0)
            {
                lst.AddRange(lstExportAssessmentItemResponseResult.Select(itemResult => new AsmntItemrAcademicStds()
                {
                    DistrictCode = itemResult.Code,
                    Document = itemResult.Document,
                    Number = itemResult.Number,
                    QuestionOrder = itemResult.QuestionOrder,
                    YearResultDate = itemResult.YearResultDate.HasValue ? itemResult.YearResultDate.Value : 0,
                    TestName = itemResult.TestName,
                    Year = itemResult.Year
                }));
            }
            return lst;
        }

        public IEnumerable<string> GetAsmntItemrAcademicStdsString(int districtId, string strTestResultIds)
        {
            var testResultIds = strTestResultIds.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            var totalRecords = testResultIds.Length;
            var pageSize = totalRecords / MaximumThreads;
            var doneEvents = new ManualResetEvent[MaximumThreads];
            var services = new List<AssessmentAcademicLoader>();
            for (var i = 0; i < MaximumThreads; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                var takeRecords = i == MaximumThreads - 1 ? int.MaxValue : pageSize;
                var partialTestResultIDs = testResultIds.Skip(i * pageSize).Take(takeRecords);
                var partialStrTestResultIDs = String.Join(";", partialTestResultIDs);
                var testExtractService = new AssessmentAcademicLoader(ConnectionString, districtId, partialStrTestResultIDs, doneEvents[i]);
                services.Add(testExtractService);
                ThreadPool.QueueUserWorkItem(testExtractService.ThreadPoolCallback, i);
            }

            WaitHandle.WaitAll(doneEvents);

            IEnumerable<Export_ASMNT_ITEMR_ACADEMIC_STDS_NewResult> testResults = new List<Export_ASMNT_ITEMR_ACADEMIC_STDS_NewResult>();
            foreach (var service in services)
            {
                testResults = testResults.Union(service.Results);
            }

            const string template = "{0}\tLinkIt\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}";
            var result = testResults.Select(item => string.Format(template,
                                        ValidData(item.Code, 8)
                                        , string.Format("{0}-06-30", item.YearResultDate)
                                        , ValidData(item.TestName, 200)
                                        , item.QuestionOrder <= 9 ? string.Format("0{0}", item.QuestionOrder) : item.QuestionOrder.ToString()
                                        , item.Document
                                        , item.Year
                                        , item.Number
                                        )).Distinct();

            return result;
        }

        public List<AsmntSubtestAcademicStds> GetAsmntSubtestAcademicStds(int districtId, string strTestResultIds)
        {
            var lst = new List<AsmntSubtestAcademicStds>();
            var lstExportAssessmentItemResponseResult = extractDbContext.Export_ASMNT_SUBTEST_ACADEMIC_STDS_New(strTestResultIds, districtId).ToList();
            if (lstExportAssessmentItemResponseResult.Count > 0)
            {
                lst.AddRange(lstExportAssessmentItemResponseResult.Select(itemResult => new AsmntSubtestAcademicStds()
                {
                    DistrictCode = itemResult.Code,
                    Year = itemResult.Year ?? 0,
                    TestName = itemResult.TestName
                }));
            }
            return lst;
        }

        public List<AssessmentAccModFact> GetAssessmentAccModFact(int districtId, string strTestResultIds)
        {
            var lst = new List<AssessmentAccModFact>();
            var lstExportAssessmentItemResponseResult = extractDbContext.Export_ASSESSMENT_ACC_MOD_FACT_New(strTestResultIds, districtId).ToList();
            if (lstExportAssessmentItemResponseResult.Count > 0)
            {
                lst.AddRange(lstExportAssessmentItemResponseResult.Select(itemResult => new AssessmentAccModFact()
                {
                    DistrictCode = itemResult.Code,
                    ResultDate = DateTime.Parse(itemResult.ResultDate),
                    StudentCode = itemResult.StudentCode,
                    TestName = itemResult.TestName,
                    Year = itemResult.Year ?? 0
                }));
            }
            return lst;
        }

        public List<AssessmentFact> GetAssessmentFact(int districtId, string strTestResultIds)
        {
            var lst = new List<AssessmentFact>();
            var lstExportAssessmentItemResponseResult = extractDbContext.Export_ASSESSMENT_FACT_New(strTestResultIds, districtId).ToList();
            if (lstExportAssessmentItemResponseResult.Count > 0)
            {
                lst.AddRange(lstExportAssessmentItemResponseResult.Select(itemResult => new AssessmentFact()
                {
                    DistrictCode = itemResult.Code,
                    ResultDate = DateTime.Parse(itemResult.ResultDate),
                    StudentCode = itemResult.StudentCode,
                    TestName = itemResult.TestName,
                    TotalPointsEarned = itemResult.TotalPointsEarned ?? 0,
                    TotalPointsPossible = itemResult.TotalPointsPossible ?? 0,
                    Year = itemResult.Year ?? 0
                }));
            }
            return lst;
        }

        public List<AssessmentResponse> GetAssessmentResponse(int districtId, string strTestResultIds)
        {
            var testResultIds = strTestResultIds.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            var totalRecords = testResultIds.Length;
            var pageSize = totalRecords / MaximumThreads;
            var doneEvents = new ManualResetEvent[MaximumThreads];
            var services = new List<AssessmentResponseLoader>();
            for (var i = 0; i < MaximumThreads; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                var takeRecords = i == MaximumThreads - 1 ? int.MaxValue : pageSize;
                var partialTestResultIDs = testResultIds.Skip(i * pageSize).Take(takeRecords);
                var partialStrTestResultIDs = String.Join(";", partialTestResultIDs);
                var testExtractService = new AssessmentResponseLoader(ConnectionString, districtId, partialStrTestResultIDs, doneEvents[i]);
                services.Add(testExtractService);
                ThreadPool.QueueUserWorkItem(testExtractService.ThreadPoolCallback, i);
            }

            WaitHandle.WaitAll(doneEvents);

            IEnumerable<Export_ASSESSMENT_RESPONSE_NewResult> testResults = new List<Export_ASSESSMENT_RESPONSE_NewResult>();
            foreach (var service in services)
            {
                testResults = testResults.Union(service.Results);
            }

            var result = testResults.Select(itemResult => new AssessmentResponse
            {
                AnswerText = itemResult.AnswerText,
                DistrictCode = itemResult.Code,
                PointsEarned = itemResult.PointsEarned,
                QuestionOrder = itemResult.QuestionOrder,
                ResultDate = itemResult.ResultDate.Date,
                StudentCode = itemResult.StudentCode,
                TestName = itemResult.TestName,
                Year = itemResult.ResultDate.Month > 6 ? itemResult.ResultDate.Year + 1 : itemResult.ResultDate.Year
            }).Distinct(new MyObjEqualityComparer()).ToList();
            return result;
        }

        public IEnumerable<string> GetAssessmentResponseString(int districtId, string strTestResultIds)
        {
            var testResultIds = strTestResultIds.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            var totalRecords = testResultIds.Length;
            var pageSize = totalRecords / MaximumThreads;
            var doneEvents = new ManualResetEvent[MaximumThreads];
            var services = new List<AssessmentResponseLoader>();
            for (var i = 0; i < MaximumThreads; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                var takeRecords = i == MaximumThreads - 1 ? int.MaxValue : pageSize;
                var partialTestResultIDs = testResultIds.Skip(i * pageSize).Take(takeRecords);
                var partialStrTestResultIDs = String.Join(";", partialTestResultIDs);
                var testExtractService = new AssessmentResponseLoader(ConnectionString, districtId, partialStrTestResultIDs, doneEvents[i]);
                services.Add(testExtractService);
                ThreadPool.QueueUserWorkItem(testExtractService.ThreadPoolCallback, i);
            }

            WaitHandle.WaitAll(doneEvents);

            IEnumerable<Export_ASSESSMENT_RESPONSE_NewResult> testResults = new List<Export_ASSESSMENT_RESPONSE_NewResult>();
            foreach (var service in services)
            {
                testResults = testResults.Union(service.Results);
            }

            const string assessmentTemplate = "{0}\tLinkIt\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t\t\t\t\t\t\t\t{7}\t\t\t\t\t\t";
            var result = testResults.Select(item => string.Format(assessmentTemplate,
                                        ValidData(item.Code, 8)
                                        , string.Format("{0}-06-30", item.ResultDate.Month > 6 ? item.ResultDate.Year + 1 : item.ResultDate.Year)
                                        , ValidData(item.TestName, 200)
                                        , String.Format("{0:yyyy-MM-dd}", item.ResultDate)
                                        , BuildStudentCode(item.StudentCode)
                                        , item.QuestionOrder <= 9 ? string.Format("0{0}", item.QuestionOrder) : item.QuestionOrder.ToString()
                                        , item.AnswerText
                                        , item.PointsEarned.ToString())).Distinct();

            return result;
        }

        public List<int> GetExtractTestResultIDs(ExtractLocalCustom obj)
        {
            return extractDbContext.extrGetAllTestResultIdByFilter(obj.DistrictId, obj.StartDate, obj.EndDate.AddDays(1), obj.BankdId, obj.StudentId, obj.ListTestIDs, obj.SchoolId, obj.ClassId, obj.TeacherId, obj.UserId, obj.UserRoleId)
                .ToList().Select(o => o.TestResultID).ToList();
        }

        public List<ExtractTestResult> GetExtractTestResults(ExtractLocalCustom obj, int pageIndex, int pageSize, ref int? totalRecords, string sortColumns)
        {
            var data = extractDbContext.extrGetTestResultByFilter(obj.DistrictId, obj.StartDate,
                obj.EndDate.AddDays(1), obj.BankdId, obj.StudentId, obj.ListTestIDs, obj.SchoolId,
                obj.ClassId, obj.TeacherId, obj.UserId, obj.UserRoleId, pageIndex, pageSize, sortColumns, obj.GradeId,
                obj.SubjectName, obj.SSearch, obj.IsHideStudentName)
                .Select(o => new ExtractTestResult
                {
                    TestResultId = o.TestResultID,
                    TestNameCustom = o.TestNameCustom,
                    ResultDate = o.ResultDate,
                    SchoolName = o.SchoolName,
                    TeacherCustom = o.TeacherCustom,
                    ClassNameCustom = o.ClassNameCustom,
                    StudentCustom = o.StudentCustom,
                    StudentCodeCustom = string.Format("{0} ({1})", o.StudentID, o.StudentCode),
                    TotalRows = o.TotalRows
                }).ToList();
            if (data != null)
            {
                if (data.Count > 0)
                {
                    totalRecords = data[0].TotalRows;
                }
            }
            return data;
        }
        public List<ExtractUser> GetExtractUser(ExtractUserCustom obj)
        {
            return extractDbContext.extrGetUserByFilter(obj.DistrictId, obj.StartDate.ToShortDateString(),
                obj.EndDate.AddDays(1).ToShortDateString(), obj.ListSchoolIDs, obj.UserId, obj.UserRoleId)
                .ToList().Select(o => new ExtractUser
                {
                    UserId = o.UserID,
                    SchoolId = o.SchoolID ?? 0,
                    UserName = o.UserName,
                    FirstName = o.NameFirst,
                    LastName = o.NameLast,
                    SchoolName = o.Name,
                    CreatedDate = o.CreatedDate,
                    ModifiedDate = o.ModifiedDate
                }).ToList();
        }

        public List<ExportTest> GetExtractTest(ExtractLocalCustom obj)
        {
            return extractDbContext.extrGetVirtualTestByFilter(obj.DistrictId, obj.StartDate.ToShortDateString(),
                obj.EndDate.AddDays(1).ToShortDateString(), obj.UserId, obj.UserRoleId, obj.GradeId, obj.SubjectName, obj.BankdId, obj.ListTestIDs)
                .ToList().Select(o => new ExportTest
                {
                    VirtualTestId = o.VirtualTestID,
                    BankName = o.BankName,
                    TestName = o.Name,
                    Grade = o.Grade,
                    Subject = o.Subject
                }).ToList();
        }

        public List<QTITestClassAssignment> GetExtractTestAssignment(ExtractLocalCustom obj)
        {
            return extractDbContext.extrGetTestAssignmentByFilter(obj.DistrictId, obj.StartDate.ToShortDateString(),
                obj.EndDate.AddDays(1).ToShortDateString(), obj.GradeId, obj.SubjectName, obj.BankdId, obj.ListTestIDs, obj.SchoolId, obj.ClassId, obj.TeacherId, obj.UserId, obj.UserRoleId)
                .ToList().Select(x => new QTITestClassAssignment
                {
                    AssignmentDate = x.AssignmentDate,
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherCustom,
                    QTITestClassAssignmentID = x.QTITestClassAssignmentID,
                    TestName = x.TestNameCustom,
                    Code = x.Code
                }).ToList();
        }

        public List<ExportRosterData> GetExtractRoster(ExtractRosterCustom obj)
        {
            return extractDbContext.extrGetRosterByFilter(obj.DistrictId, obj.StartDate.ToShortDateString(),
                obj.EndDate.AddDays(1).ToShortDateString(), obj.SchoolId, obj.TeacherId, obj.ListClassIDs, obj.UserId, obj.UserRoleId)
                .ToList().Select(x => new ExportRosterData
                {
                    ClassID = x.ClassID,
                    ClassName = x.ClassName,
                    SchoolName = x.SchoolName,
                    Username = x.UserName,
                    Term = x.DistrictTerm,
                    StudentCode = x.StudentCode,
                    StudentNameCustom = x.StudentCustom,
                    ClassStudentID = x.ClassStudentID
                }).ToList();
        }

        public List<int> GetAllClassIds(ExtractRosterCustom obj)
        {
            return
                extractDbContext.extrGetClassIDsByFilter(obj.DistrictId, obj.StartDate.ToShortDateString(),
                obj.EndDate.AddDays(1).ToShortDateString(), obj.SchoolId, obj.TeacherId,
                    obj.ListClassIDs, obj.UserId, obj.UserRoleId).Select(o => o.ClassID).ToList();
        }

        public void ReEvaluateBadge(int testResultID)
        {
            dbContext.TestResult_ReEvaluateBadge(testResultID);
        }

        public void ReEvaluateBadgeV2(string testResultIds)
        {
            dbContext.TestResult_ReEvaluateBadgeV2(testResultIds);
        }

        private string ValidData(string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            var tmp = str.Replace("\n", string.Empty).Replace("\r", string.Empty);
            if (tmp.Length > maxLength)
            {
                return tmp.Substring(0, maxLength);
            }
            return tmp;
        }

        private string BuildStudentCode(string code)
        {
            var studentCode = string.Empty;
            if (code.Length < 9)
            {
                for (int i = 1; i + code.Length < 10; i++)
                {
                    studentCode += "0";
                }
                studentCode += code;
            }
            else
            {
                studentCode = code;
            }
            return studentCode;
        }

        /// <summary>
        /// Update TestResult by Class
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="districtTermId"></param>
        /// <param name="teacherId"></param>
        /// <param name="classId"></param>
        /// <param name="testResultId"></param>
        /// <returns></returns>
        public bool TransferTestResultByClass(int schoolId, int districtTermId, int teacherId, int classId,
            int testResultId)
        {
            try
            {
                var vTestResult = table.FirstOrDefault(o => o.TestResultID == testResultId);
                if (vTestResult != null)
                {
                    vTestResult.SchoolID = schoolId;
                    vTestResult.DistrictTermID = districtTermId;
                    vTestResult.ClassID = classId;
                    vTestResult.UserID = teacherId;
                    vTestResult.UpdatedDate = DateTime.UtcNow;
                    table.Context.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                //TODO: log here
            }
            return false;
        }

        public void SaveExtractTestResultToQueue(ExtractLocalCustom obj, int type, string lstTemplates, int timezoneOffset, string baseHostURL, string lstIdsUncheck)
        {
            extractDbContext.extrAddExtrTestResultToQueue(obj.DistrictId, obj.StartDate.ToShortDateString(),
                obj.EndDate.AddDays(1).ToShortDateString(), obj.BankdId, obj.StudentId, obj.ListTestIDs, obj.SchoolId, obj.ClassId,
                obj.TeacherId, obj.UserId, obj.UserRoleId, timezoneOffset, type, DateTime.UtcNow, baseHostURL, lstTemplates, lstIdsUncheck);
        }

        public bool RetagTestResults(RetagTestResult obj)
        {
            try
            {
                dbContext.AddRetagTestResults(obj.ListTestResultIds, obj.DistrictId, obj.UserId, obj.Gradebook, obj.StudentRecord, obj.CleverApi, obj.IsExportRawScore);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class MyObjEqualityComparer : IEqualityComparer<AssessmentResponse>
    {
        public bool Equals(AssessmentResponse x, AssessmentResponse y)
        {
            return true;

            //return string.CompareOrdinal(x.AnswerText, y.AnswerText) == 0
            //       && string.CompareOrdinal(x.DistrictCode, y.DistrictCode) == 0
            //       && x.PointsEarned == y.PointsEarned
            //       && x.QuestionOrder == y.QuestionOrder
            //       && x.ResultDate.Date == y.ResultDate.Date
            //       && string.CompareOrdinal(x.StudentCode, y.StudentCode) == 0
            //       && string.CompareOrdinal(x.TestName, y.TestName) == 0;
        }

        public int GetHashCode(AssessmentResponse obj)
        {
            return obj.StudentCode.GetHashCode();
        }
    }
}
