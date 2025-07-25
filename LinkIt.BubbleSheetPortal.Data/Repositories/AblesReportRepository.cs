using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AblesReportRepository : IAblesReportRepository
    {
        private readonly Table<AblesReportTypeEntity> table;
        private readonly TestDataContext _testContext;

        public AblesReportRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _testContext = TestDataContext.Get(connectionString);
            _testContext.CommandTimeout = 300; //increase timeout value for dbcontext in report module

            table = _testContext.GetTable<AblesReportTypeEntity>();
        }

        public List<ReportType> GetAllReportTypes()
        {
            return
                table.Select(
                    r => new ReportType() { ReportTypeId = r.ReportTypeID, Name = r.Name, ReportOrder = r.ReportOrder })
                    .ToList();
        }

        public List<AblesPointsEarnedResponseData> GetPointsEarnedResponsed(int districtId, string studentList, int schoolId,
            string termIdList, string testIdList, bool isASD)
        {
            return
                _testContext.GetPointEarnedForAblesReport(districtId, studentList, schoolId, termIdList, testIdList, isASD)
                    .Select(x => new AblesPointsEarnedResponseData
                    {
                        TestResultID = x.TestResultID,
                        VirtualTestID = x.VirtualTestID,
                        AssessmentRoundID = x.AssessmentRoundID,
                        ResultDate = x.ResultDate,
                        StudentID = x.StudentID,
                        StudentName = x.StudentName,
                        StudentCode = x.StudentCode,
                        StartDate = x.DateStart,
                        EndDate = x.DateEnd,
                        Answers = x.Answers,
                        ValueMapping = x.ValueMapping,
                        AblesTestName = x.AblesTestName,
                        ClassName = x.ClassName,
                        StateCode = x.StateCode
                    }).ToList();
        }

        public List<AblesPointsEarnedResponseData> GetAblesDataForAdminReporting(int studentId, int testresultId)
        {
            return
              _testContext.AR_GetAblesData(studentId, testresultId)
                    .Select(x => new AblesPointsEarnedResponseData
                    {
                        TestResultID = x.TestResultID,
                        AssessmentRoundID = x.AssessmentRoundID,
                        ResultDate = x.ResultDate,
                        StudentCode = x.StudentCode,
                        Answers = x.Answers,
                        ValueMapping = x.ValueMapping,
                        AblesTestName = x.AblesTestName,
                        StateCode = x.StateCode,
                        ClassName = x.ClassName,
                        SchoolCode = x.SchoolCode,
                        StudentName = x.StudentName,
                    }).ToList();
        }

        public List<AblesDataDropDown> GetAblesDataDropDown(int? districtId, int? schoolId, int? teacherId, int userId, int roleId,
            string districtTermIdStr, string testIdList, bool isASD, int year)
        {
            return
                _testContext.GetClassForAblesDataDropDown(districtId, userId, roleId, schoolId, teacherId, districtTermIdStr, testIdList, isASD, year)
                    .Select(x => new AblesDataDropDown()
                    {
                        Id = x.ID,
                        Name = x.Name
                    }).ToList();
        }

        public List<AblesReportJobData> GetAblesReportJobs(int districtId, int userId, DateTime fromDate, DateTime toDate)
        {
            return _testContext.GetAblesReportJob(districtId, userId, fromDate, toDate).Select(x => new AblesReportJobData()
            {
                AblesReportJobId = x.AblesReportJobID,
                ReportTypeId = x.ReportTypeID,
                ReportName = x.ReportName,
                SchoolName = x.SchoolName,
                TeacherName = string.Format("{0} {1}", x.NameLast, x.NameFirst),
                ClassName = x.ClassName,
                AssessmentRound = x.AssessmentRound,
                CreatedDate = x.CreatedDate,
                DownloadUrl = x.DownloadUrl,
                LearningArea = x.LearningArea
            }).OrderByDescending(x => x.CreatedDate).ToList();
        }

        public List<AblesStudent> GetStudentHasData(int? districtId, int? classId,
            string districtTermIdStr, string testIdList, bool isASD)
        {
            return
                _testContext.AblesGetStudentHasData(districtId, classId, districtTermIdStr, testIdList, isASD)
                    .Select(x => new AblesStudent()
                    {
                        StudentId = x.StudentID,
                        FullName = x.FullName,
                        HasTestResult = x.HasTestResult ?? false
                    }).ToList();
        }

        public int? GetMinResultYearBySchool(int? schoolId)
        {
            return _testContext.GetMinResultYearBySchool(schoolId).Select(x=>x.MinYear).FirstOrDefault();
        }
    }
}
