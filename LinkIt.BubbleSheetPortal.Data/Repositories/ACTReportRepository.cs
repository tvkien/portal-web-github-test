using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ACTReport;
using LinkIt.BubbleSheetPortal.Models.ACTSummaryReport;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ACTReportRepository : IACTReportRepository
    {
        private readonly BubbleSheetDataContext _dbContext;
        private readonly Table<ReportTypeEntity> table;
        private readonly TestDataContext _testContext;

        public ACTReportRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = BubbleSheetDataContext.Get(connectionString).GetTable<ReportTypeEntity>();
            _dbContext = BubbleSheetDataContext.Get(connectionString);            
            _dbContext.CommandTimeout = 300; //increase timeout value for dbcontext in report module

            _testContext = TestDataContext.Get(connectionString);
            _testContext.CommandTimeout = 300; //increase timeout value for dbcontext in report module
        }

        public IList<ACTAnswerSectionData> ACTGetAnswerSectionData(int testResultID)
        {
            return _dbContext.ACTGetAnswerSectionData(testResultID)
                .Select(x => new ACTAnswerSectionData
                             {
                                 AnswerID = x.answerID,
                                 AnswerLetter = x.AnswerLetter,
                                 PointsEarned = x.PointsEarned,
                                 PointsPossible = x.PointsPossible,
                                 QuestionOrder = x.QuestionOrder,
                                 WasAnswered = x.WasAnswered,
                                 SectionID = x.VirtualSectionID,
                                 SectionName = x.SectionName,
                                 SectionOrder = x.SectionOrder,
                                 CategoryID = x.CategoryID??0,
                                 CategoryName = x.CategoryName,
                                 TagName = x.TagName,
                                 TagID = x.TagID??0,
                                 CorrectAnswer = x.CorrectAnswer                                 
                             }).ToList();
        }

        public IList<ACTSectionTagData> ACTGetSectionTagData(int studentID, int virtualTestSubTypeId)
        {
            return _dbContext.ACTGetSectionTagData(studentID, virtualTestSubTypeId)
                .Select(x => new ACTSectionTagData
                             {
                                 TotalAnswer = x.TotalAnswer ?? 0,
                                 CorrectAnswer = x.CorrectAnswer ?? 0,
                                 CategoryID = x.ItemTagCategoryID ?? 0,
                                 CategoryName = x.TagCategoryName ?? string.Empty,
                                 SectionID = x.VirtualSectionID ?? 0,
                                 TagName = x.TagName ?? string.Empty,
                                 TagNameForOrder = x.TagNameForOrder ?? string.Empty,
                                 BlankAnswer = x.BlankAnswer ?? 0,
                                 HistoricalAvg = x.HistoricalAvg ?? 0,
                                 Percentage = x.Percentage ?? 0,
                                 SectionName = x.SectionName ?? string.Empty,
                                 TagID = x.ItemTagID ?? 0,
                                 UpdatedDate = x.UpdatedDate ?? DateTime.Now,
                                 VirtualTestID = x.VirtualTestID ?? 0,
                                 IncorrectAnswer = x.IncorrectAnswer ?? 0,
                                 TestResultID = x.TestResultID ?? 0,
                                 CategoryDescription = x.TagCategoryDescription ?? string.Empty,
                                 MinQuestionOrder = x.MinQuestionOrder.GetValueOrDefault(),
                                 ItemTagCategoryOrder = x.ItemTagCategoryOrder,
                                 ItemTagOrder = x.ItemTagOrder,
                                 PresentationType = x.PresentationType,
                                 SectionOrder = x.SectionOrder
                             }).ToList();
        }

        public IList<ACTTestHistoryData> ACTGetTestHistoryData(int studentID, int virtualTestSubTypeId)
        {
            return _dbContext.ACTGetTestHistory(studentID, virtualTestSubTypeId)
                .Select(x => new ACTTestHistoryData
                             {
                                 SectionName = x.SectionName,
                                 UpdatedDate = x.UpdatedDate,
                                 SectionScore = x.ScoreScaled ?? 0,
                                 SectionScoreRaw = x.ScoreRaw?? 0,
                                 CompositeScore = x.ScoreScaled_Composite?? 0,
                                 StudentID = x.StudentID,
                                 TeacherID = x.TeacherID ?? 0,
                                 TestResultID = x.TestResultID,
                                 VirtualTestID = x.VirtualTestID,
                                 TestResultSubScoreID = x.TestResultSubScoreID ?? 0,
                                 TestName = x.TestName,
                                 VirtualTestSubTypeID = x.VirtualTestSubTypeID ?? 2
                             }).ToList();
        }

        public ACTStudentInformation ACTGetStudentInformation(int studentID, int testResultID)
        {
            return _dbContext.ACTGetStudentInformation(studentID, testResultID)
                .Select(x => new ACTStudentInformation
                             {
                                 ClassName = x.ClassName,
                                 DistrictTermName = x.TermName,
                                 StudentCode = x.StudentCode,
                                 StudentFirstName = x.StudentFirstName,
                                 StudentLastName = x.StudentLastName,
                                 TeacherName = x.TeacherName,
                                 TestName = x.TestName,
                                 TestDate = x.TestDate
                             }).FirstOrDefault();
        }

        public IList<ACTSummaryClassLevelData> ACTSummaryGetClassLevelData(int classID, int virtualTestID, int virtualTestSubTypeId)
        {
            return _dbContext.ACTSummaryReportGetClassLevelData(classID, virtualTestID, virtualTestSubTypeId)
                .Select(x => new ACTSummaryClassLevelData
                             {
                                 ClassID = x.ClassID,
                                 ScoreRaw = x.ScoreRaw ?? 0,
                                 ScoreScaled = x.ScoreScaled ?? 0,
                                 SectionName = string.IsNullOrEmpty(x.NAME) ? string.Empty : x.NAME,
                                 StudentCode = x.Code,
                                 StudentFirstName = x.FirstName,
                                 StudentID = x.StudentID,
                                 StudentLastName = x.LastName,
                                 TestResultID = x.TestResultID ?? 0,
                                 ResultDate = x.ResultDate
                             }).ToList();
        }

        //public IList<ACTSummaryDistrictLevelData> ACTSummaryGetDistrictLevelData(int districtID, int virtualTestID)
        //{
        //    return _dbContext.ACTSummaryReportGetDistrictLevelData(districtID, virtualTestID)
        //        .Select(x => new ACTSummaryDistrictLevelData
        //                     {
        //                         SchoolID = x.SchoolID,
        //                         SchoolName = x.SchoolName,
        //                         SectionName = x.NAME,
        //                         StudentID = x.StudentID,
        //                         ScoreRaw = x.ScoreRaw ?? 0,
        //                         ScoreScaled = x.ScoreScaled ?? 0,
        //                         ResultDate = x.ResultDate,
        //                         TestResultID = x.TestResultID
        //                     }).ToList();
        //}

        public IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetTeacherLevelData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.ACTSummaryReportGetTeacherLevelData(userId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.ClassID,
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherName,
                    DistrictTermName = x.DistrictTermName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                    TestName = x.TestName,
                    VirtualTestId = x.VirtualTestID
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetTeacherLevelAverageData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.ACTSummaryReportGetTeacherLevelAverageData(userId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    VirtualTestId = x.VirtualTestID,
                    SectionName = x.SectionName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,                    
                    StudentNo = x.StudentNo
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetTeacherLevelBaselineData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.ACTSummaryReportGetTeacherLevelBaselineData(userId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.ClassID,
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherName,
                    DistrictTermName = x.DistrictTermName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetTeacherLevelImprovementData(int userId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.ACTSummaryReportGetTeacherLevelImprovementData(userId, districtTermId, virtualTestIdList, improvementOption, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.ClassID,
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherName,
                    DistrictTermName = x.DistrictTermName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetSchoolLevelData(int schoolID, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.ACTSummaryReportGetSchoolLevelData(schoolID, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.ClassID,
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherName,
                    DistrictTermName = x.DistrictTermName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                    TestName = x.TestName,
                    VirtualTestId = x.VirtualTestID
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetSchoolLevelAverageData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.ACTSummaryReportGetSchoolLevelAverageData(userId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    VirtualTestId = x.VirtualTestID,
                    SectionName = x.SectionName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    StudentNo = x.StudentNo
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetSchoolLevelBaselineData(int schoolId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.ACTSummaryReportGetSchoolLevelBaselineData(schoolId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.ClassID,
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherName,
                    DistrictTermName = x.DistrictTermName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetSchoolLevelImprovementData(int schoolId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.ACTSummaryReportGetSchoolLevelImprovementData(schoolId, districtTermId, virtualTestIdList, improvementOption, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.ClassID,
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherName,
                    DistrictTermName = x.DistrictTermName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                }).ToList();
        }


        public IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetDistrictLevelData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            _dbContext.CommandTimeout = 180;
            var data = _dbContext.ACTSummaryReportGetDistrictLevelData(districtId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.SchoolID,
                    ClassName = x.SchoolName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                    TestName = x.TestName,
                    VirtualTestId = x.VirtualTestID
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetDistrictLevelAverageData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            _dbContext.CommandTimeout = 180;
            var data = _dbContext.ACTSummaryReportGetDistrictLevelAverageData(districtId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    VirtualTestId = x.VirtualTestID,
                    SectionName = x.SectionName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    StudentNo = x.StudentNo
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetDistrictLevelBaselineData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            _dbContext.CommandTimeout = 180;
            var data = _dbContext.ACTSummaryReportGetDistrictLevelBaselineData(districtId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.SchoolID,
                    ClassName = x.SchoolName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetDistrictLevelImprovementData(int districtId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            _dbContext.CommandTimeout = 180;
            var data = _dbContext.ACTSummaryReportGetDistrictLevelImprovementData(districtId, districtTermId, virtualTestIdList, improvementOption, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.SchoolID,
                    ClassName = x.SchoolName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                }).ToList();
        }

        public IList<ACTSummaryClassLevelData> SATSummaryGetClassLevelData(int classID, int virtualTestID, int virtualTestSubTypeId)
        {
            return _dbContext.SATSummaryReportGetClassLevelData(classID, virtualTestID, virtualTestSubTypeId)
                .Select(x => new ACTSummaryClassLevelData
                {
                    ClassID = x.ClassID,
                    ScoreRaw = x.ScoreRaw ?? 0,
                    ScoreScaled = x.ScoreScaled ?? 0,
                    SectionName = string.IsNullOrEmpty(x.NAME) ? string.Empty : x.NAME,
                    StudentCode = x.Code,
                    StudentFirstName = x.FirstName,
                    StudentID = x.StudentID,
                    StudentLastName = x.LastName,
                    TestResultID = x.TestResultID ?? 0,
                    ResultDate = x.ResultDate
                }).ToList();
        }

        public IList<ACTSummaryClassLevelData> SATSummaryGetClassLevelImprovementData(int classId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.SATSummaryReportGetClassLevelImprovementData(classId, virtualTestIdList, improvementOption, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummaryClassLevelData
                {
                    ClassID = x.ClassID,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.NAME,
                    StudentCode = x.Code,
                    StudentFirstName = x.FirstName,
                    StudentID = x.StudentID,
                    StudentLastName = x.LastName,
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetTeacherLevelData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.SATSummaryReportGetTeacherLevelData(userId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.ClassID,
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherName,
                    DistrictTermName = x.DistrictTermName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                    TestName = x.TestName,
                    VirtualTestId = x.VirtualTestID
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetTeacherLevelAverageData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.SATSummaryReportGetTeacherLevelAverageData(userId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    VirtualTestId = x.VirtualTestID,
                    SectionName = x.SectionName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    StudentNo = x.StudentNo
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetTeacherLevelBaselineData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.SATSummaryReportGetTeacherLevelBaselineData(userId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.ClassID,
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherName,
                    DistrictTermName = x.DistrictTermName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetTeacherLevelImprovementData(int userId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.SATSummaryReportGetTeacherLevelImprovementData(userId, districtTermId, virtualTestIdList, improvementOption, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.ClassID,
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherName,
                    DistrictTermName = x.DistrictTermName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetSchoolLevelData(int schoolId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.SATSummaryReportGetSchoolLevelData(schoolId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.ClassID,
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherName,
                    DistrictTermName = x.DistrictTermName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                    TestName = x.TestName,
                    VirtualTestId = x.VirtualTestID
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetSchoolLevelAverageData(int schoolId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.SATSummaryReportGetSchoolLevelAverageData(schoolId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    VirtualTestId = x.VirtualTestID,
                    SectionName = x.SectionName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    StudentNo = x.StudentNo
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetSchoolLevelBaselineData(int schoolId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.SATSummaryReportGetSchoolLevelBaselineData(schoolId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.ClassID,
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherName,
                    DistrictTermName = x.DistrictTermName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetSchoolLevelImprovementData(int schoolId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.SATSummaryReportGetSchoolLevelImprovementData(schoolId, districtTermId, virtualTestIdList, improvementOption, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.ClassID,
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherName,
                    DistrictTermName = x.DistrictTermName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetDistrictLevelData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            _dbContext.CommandTimeout = 180;
            var data = _dbContext.SATSummaryReportGetDistrictLevelData(districtId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.SchoolID,
                    ClassName = x.SchoolName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                    TestName = x.TestName,
                    VirtualTestId = x.VirtualTestID
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetDistrictLevelAverageData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            _dbContext.CommandTimeout = 180;
            var data = _dbContext.SATSummaryReportGetDistrictLevelAverageData(districtId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    VirtualTestId = x.VirtualTestID,
                    SectionName = x.SectionName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    StudentNo = x.StudentNo
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetDistrictLevelBaselineData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            _dbContext.CommandTimeout = 180;
            var data = _dbContext.SATSummaryReportGetDistrictLevelBaselineData(districtId, districtTermId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.SchoolID,
                    ClassName = x.SchoolName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                }).ToList();
        }

        public IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetDistrictLevelImprovementData(int districtId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            _dbContext.CommandTimeout = 180;
            var data = _dbContext.SATSummaryReportGetDistrictLevelImprovementData(districtId, districtTermId, virtualTestIdList, improvementOption, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummarySchoolOrTeacherLevelData
                {
                    ClassId = x.SchoolID,
                    ClassName = x.SchoolName,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.SectionName,
                    StudentNo = x.StudentNo,
                }).ToList();
        }


        public IList<ACTSummaryClassLevelData> ACTSummaryGetClassLevelImprovementData(int classId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.ACTSummaryReportGetClassLevelImprovementData(classId, virtualTestIdList, improvementOption, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummaryClassLevelData
                {
                    ClassID = x.ClassID,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.NAME,
                    StudentCode = x.Code,
                    StudentFirstName = x.FirstName,
                    StudentID = x.StudentID,
                    StudentLastName = x.LastName,                                        
                }).ToList();
        }

        public IList<ACTSummaryClassLevelData> ACTSummaryGetClassLevelBaselineData(int classId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.ACTSummaryReportGetClassLevelBaselineData(classId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummaryClassLevelData
                {
                    ClassID = x.ClassID,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.NAME,
                    StudentCode = x.Code,
                    StudentFirstName = x.FirstName,
                    StudentID = x.StudentID,
                    StudentLastName = x.LastName,
                }).ToList();
        }

        public IList<ACTSummaryClassLevelData> SATSummaryGetClassLevelBaselineData(int classId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            var virtualTestIdList = ",";
            foreach (var virtualTestId in virtualTestIds)
            {
                virtualTestIdList += virtualTestId + ",";
            }

            var data = _dbContext.SATSummaryReportGetClassLevelBaselineData(classId, virtualTestIdList, virtualTestSubTypeId);

            return data
                .Select(x => new ACTSummaryClassLevelData
                {
                    ClassID = x.ClassID,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    SectionName = x.NAME,
                    StudentCode = x.Code,
                    StudentFirstName = x.FirstName,
                    StudentID = x.StudentID,
                    StudentLastName = x.LastName,
                }).ToList();
        }

        public IList<ACTAnswerSectionData> SATGetAnswerSectionData(int testResultID)
        {
            return _dbContext.SATGetAnswerSectionData(testResultID)
                .Select(x => new ACTAnswerSectionData
                {
                    AnswerID = x.answerID ?? 0,
                    AnswerLetter = x.AnswerLetter,
                    PointsEarned = x.PointsEarned  ?? 0,
                    PointsPossible = x.PointsPossible ?? 0,
                    QuestionOrder = x.QuestionOrder,
                    WasAnswered = x.WasAnswered ?? false,
                    SectionID = x.VirtualSectionID,
                    SectionName = x.SectionTitle,
                    SectionOrder = x.SectionOrder,
                    CategoryID = x.CategoryID ?? 0,
                    CategoryName = x.CategoryName,
                    TagName = x.TagName,
                    TagID = x.TagID ?? 0,
                    CorrectAnswer = x.CorrectAnswer,
                    QTISchemaID = x.QTISchemaID,
                    SubjectID = x.SubjectID,
                    SubjectName = x.SubjectName,
                    VirtualQuestionID = x.VirtualQuestionID,
                    PassageName = x.PassageName,
                    PassageID = x.QTIRefObjectID
                }).ToList();
        }

        public IList<ACTSectionTagData> SATGetSectionTagData(int studentID, int virtualTestSubTypeID)
        {
            return _dbContext.SATGetSectionTagData(studentID, virtualTestSubTypeID)
                .Select(x => new ACTSectionTagData
                {
                    TotalAnswer = x.TotalAnswer ?? 0,
                    CorrectAnswer = x.CorrectAnswer ?? 0,
                    CategoryID = x.ItemTagCategoryID ?? 0,
                    CategoryName = x.TagCategoryName ?? string.Empty,
                    SubjectID = x.SubjectID ?? 0,
                    TagName = x.TagName ?? string.Empty,
                    TagNameForOrder = x.TagNameForOrder ?? string.Empty,
                    BlankAnswer = x.BlankAnswer ?? 0,
                    HistoricalAvg = x.HistoricalAvg ?? 0,
                    Percentage = x.Percentage ?? 0,
                    SubjectName = x.SubjectName ?? string.Empty,
                    TagID = x.ItemTagID ?? 0,
                    UpdatedDate = x.UpdatedDate ?? DateTime.Now,
                    VirtualTestID = x.VirtualTestID ?? 0,
                    IncorrectAnswer = x.IncorrectAnswer ?? 0,
                    TestResultID = x.TestResultID ?? 0,
                    CategoryDescription = x.TagCategoryDescription ?? string.Empty,
                    MinQuestionOrder = x.MinQuestionOrder ?? 0,
                    PresentationType = x.PresentationType,
                    ItemTagCategoryOrder = x.ItemTagCategoryOrder,
                    ItemTagOrder = x.ItemTagOrder

                }).ToList();
        }

        public IList<ACTTestHistoryData> SATGetTestHistoryData(int studentID, int virtualTestSubTypeID)
        {
            return _dbContext.SATGetTestHistory(studentID, virtualTestSubTypeID)
                .Select(x => new ACTTestHistoryData
                {
                    SectionName = x.SectionName,
                    UpdatedDate = x.UpdatedDate,
                    SectionScore = x.ScoreScaled ?? 0,                    
                    CompositeScore = x.ScoreScaled_Composite ?? 0,
                    SectionScoreRaw = x.ScoreRaw??0,
                    StudentID = x.StudentID,
                    TeacherID = x.TeacherID ?? 0,
                    TestResultID = x.TestResultID,
                    VirtualTestID = x.VirtualTestID,
                    TestResultSubScoreID = x.TestResultSubScoreID ?? 0,
                    TestName = x.TestName,
                    ClassID = x.ClassID ?? 0
                }).ToList();
        }

        public ACTStudentInformation SATGetStudentInformation(int studentID, int testResultID)
        {
            return _dbContext.SATGetStudentInformation(studentID, testResultID)
                .Select(x => new ACTStudentInformation
                {
                    ClassName = x.ClassName,
                    DistrictTermName = x.TermName,
                    StudentCode = x.StudentCode,
                    StudentFirstName = x.StudentFirstName,
                    StudentLastName = x.StudentLastName,
                    TeacherName = x.TeacherName,
                    TestName = x.TestName,
                    TestDate = x.TestDate
                }).FirstOrDefault();
        }

        public IList<ReportType> GetReportTypes(List<int> reportTypeIds)
        {
            return table.Where(x => reportTypeIds.Contains(x.ReportTypeID)).Select(r => new ReportType(){ReportTypeId = r.ReportTypeID, Name = r.Name, ReportOrder = r.ReportOrder, DisplayName = r.DisplayName}).ToList();
        }

        public IList<ReportType> GetAllReportTypes()
        {
            return table.Select(r => new ReportType() { ReportTypeId = r.ReportTypeID, Name = r.Name, ReportOrder = r.ReportOrder, DisplayName = r.DisplayName}).ToList();
        }
    }
}