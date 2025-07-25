using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.SGO;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Models.SGOManagement;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGOSelectDataPointRepository : ISGOSelectDataPointRepository
    {
        private readonly SGODataContext _sgoDataContext;

        public SGOSelectDataPointRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _sgoDataContext = SGODataContext.Get(connectionString);
        }

        public List<GetStateStandardsForSGOData> GetStateStandardsForSGO(string stateCode, string subjectName,
            string gradeName, int userId, int virtualTestId)
        {
            return _sgoDataContext.GetStateStandardsForSGO(stateCode, subjectName, gradeName, userId, virtualTestId)
                .Select(x => new GetStateStandardsForSGOData
                {
                    Children = x.Children,
                    Description = x.Description,
                    GUID = x.GUID,
                    GradeId = x.GradeID,
                    GradeName = x.GradeName,
                    HighGradeId = x.HighGradeID,
                    Level = x.Level,
                    LowGradeId = x.LowGradeID,
                    MasterStandardId = x.MasterStandardID,
                    Number = x.Number,
                    ParentGUID = x.ParentGUID,
                    StateCode = x.StateCode,
                    StateId = x.StateID,
                    StateName = x.StateName,
                    SubjectName = x.SubjectName
                }).ToList();
        }

        public List<SGOGetAccessVirtualTestsData> SGOGetAccessVirtualTests(int? subjectId, string subjectName,
            int districtId,
            int? gradeId, int userId, int userRoleId)
        {
            return
                _sgoDataContext.SGOGetAccessVirtualTests(subjectId, subjectName, districtId, gradeId, userId, userRoleId)
                    .Select(x => new SGOGetAccessVirtualTestsData
                    {
                        VirtualTestId = x.VirtualTestID,
                        Name = x.Name
                    }).ToList();
        }

        public List<SGOGetTestResultSubScoreNameOfVirtualTestData> SGOGetTestResultSubScoreNameOfVirtualTest(int virtualTestId)
        {
            var i = 0;
            return
                _sgoDataContext.SGOGetTestResultSubScoreNameOfVirtualTest(virtualTestId)
                    .OrderBy(x => x.Name)
                    .Select(x => new SGOGetTestResultSubScoreNameOfVirtualTestData
                    {
                        TestResultSubScoreId = i++,
                        Name = x.Name
                    }).ToList();
        }

        public List<SGOGetStudentDataPointData> SGOGetStudentDataPoint(int sgoId, int virtualTestId)
        {
            return
                _sgoDataContext.SGOGetStudentDataPoint(sgoId, virtualTestId)
                    .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                    .Select(x => new SGOGetStudentDataPointData
                    {
                        ClassId = x.ClassID,
                        Code = x.Code,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        StudentName = x.LastName + ", " + x.FirstName,
                        ScoreRaw = x.ScoreRaw,
                        SgoId = x.SGOID,
                        SgoStudentId = x.SGOStudentID,
                        StudentId = x.StudentID,
                        TestResultId = x.TestResultID,
                        ResultDate = x.ResultDate,
                        PointsPossible = x.PointsPossible,
                        SGOStudentType = x.SGOStudentType
                    }).ToList();
        }

        public List<SGOStudentTestData> SGOGetStudentTestData(int sgoId)
        {
            return
                _sgoDataContext.SGOGetStudentTestData(sgoId)
                    .Select(x => new SGOStudentTestData
                    {
                        DataSetCategoryID = x.DataSetCategoryID,
                        DataSetCategoryName = x.DataSetCategoryName,
                        //AchievementLevelSettingId = x.AchievementLevelSettingID,
                        //AchievementLevelSettingName = x.AchievementLevelSettingName,
                        BankId = x.BankID.GetValueOrDefault(),
                        BankName = x.BankName,
                        BankAuthorId = x.BankAuthorID,
                        GradeId = x.GradeID.GetValueOrDefault(),
                        GradeName = x.GradeName,
                        GradeOrder = x.GradeOrder.GetValueOrDefault(),
                        StateId = x.StateID.GetValueOrDefault(),
                        StateName = x.StateName,
                        StudentId = x.StudentID.GetValueOrDefault(),
                        SubjectId = x.SubjectID.GetValueOrDefault(),
                        SubjectName = x.SubjectName,
                        TestResultId = x.TestResultID.GetValueOrDefault(),
                        VirtualTestId = x.VirtualTestID.GetValueOrDefault(),
                        VirtualTestName = x.VirtualTestName,
                        VirtualTestSourceId = x.VirtualTestSourceID,
                        VirtualTestType = x.VirtualTestType,
                        ResultDate = x.ResultDate,
                        DataSetOriginID = x.DataSetOriginID.GetValueOrDefault()
                    }).ToList();
        }

        public void SGORemoveDataPointRelevantData(int sgoDataPointId)
        {
            _sgoDataContext.SGORemoveDataPointRelevantData(sgoDataPointId);
        }

        public void SGORemoveTemporaryExternalTest(int virtualTestId)
        {
            _sgoDataContext.SGORemoveTemporaryExternalTest(virtualTestId);
        }

        public void SGOUpdateStudentDataPointRoster(int sgoId, int sgoDataPointId, int virtualTestId,
            DateTime resultDate, decimal pointsPossible, int createdBy, int modifiedBy, string studentDataPointXML)
        {
            _sgoDataContext.SGOUpdateStudentDataPointRoster(sgoId, sgoDataPointId, virtualTestId, resultDate, pointsPossible, createdBy, modifiedBy, GetElement(studentDataPointXML));
        }

        public void SGOSaveStudentDataPointFromVirtualTest(int sgoId, int sgoDataPointId, int virtualTestId)
        {
            _sgoDataContext.SGOSaveStudentDataPointFromVirtualTest(sgoId, sgoDataPointId, virtualTestId);
        }

        public List<SGOGetExternalVirtualTestData> SGOGetExternalVirtualTest(int userId, string subjectName, int gradeId)
        {
            return
                _sgoDataContext.SGOGetExternalVirtualTest(subjectName, gradeId, userId)
                    .Select(x => new SGOGetExternalVirtualTestData
                    {
                        Name = x.Name,
                        VirtualTestId = x.VirtualTestID
                    }).ToList();
        }

        public List<SGOGetExternalVirtualTestData> SGOGetExternalVirtualTestForProgressMonitoring(int sgoId, int userId, string subjectName, int gradeId)
        {
            return
                _sgoDataContext.SGOGetExternalVirtualTestForProgressMonitoring(sgoId, subjectName, gradeId, userId)
                    .Select(x => new SGOGetExternalVirtualTestData
                    {
                        Name = x.Name,
                        VirtualTestId = x.VirtualTestID
                    }).ToList();
        }

        public List<SGOGetVirtualTestCustomScoreData> SGOGetVirtualTestCustomScore(int sgoId, int virtualTestCustomScoreSettingId, bool? isTestTaken)
        {
            int? isTakenTest = isTestTaken.HasValue ? (isTestTaken.Value ? 1 : 0) : default(int?);

            return
                _sgoDataContext.SGOGetVirtualTestCustomScore(sgoId, virtualTestCustomScoreSettingId, isTakenTest)
                    .Select(x => new SGOGetVirtualTestCustomScoreData
                    {
                        BankId = x.BankID,
                        BankName = x.BankName,
                        GradeId = x.GradeID,
                        GradeName = x.GradeName,
                        GradeOrder = x.GradeOrder,
                        SubjectId = x.SubjectID,
                        SubjectName = x.SubjectName,
                        VirtualTestCustomScoreId = x.VirtualTestCustomScoreID,
                        VirtualTestId = x.VirtualTestID,
                        VirtualTestName = x.VirtualTestName
                    }).ToList();
        }

        public VirtualTestCustomScore SGOGetAssessmentScoreType(int virtualTestId, int districtID = 0, int virtualTestCustomScoreId = 0, int sGOId = 0, bool? isPostAssignment = false, int sgoDataPointScoreType = 0)
        {
            return _sgoDataContext.SGOGetAssessmentScoreType(virtualTestId, districtID, virtualTestCustomScoreId, sGOId, isPostAssignment, sgoDataPointScoreType)
                .Select(x => new VirtualTestCustomScore
                {
                    VirtualTestCustomScoreId = x.VirtualTestCustomScoreID,
                    UsePercent = x.UsePercent.GetValueOrDefault(),
                    UsePercentile = x.UsePercentile.GetValueOrDefault(),
                    UseRaw = x.UseRaw.GetValueOrDefault(),
                    UseScaled = x.UseScaled.GetValueOrDefault(),
                    UseIndex = x.UseIndex.GetValueOrDefault(),
                    UseLexile = x.UseLexile.GetValueOrDefault(),
                    UseCustomN1 = x.UseCustomN1.GetValueOrDefault(),
                    UseCustomN2 = x.UseCustomN2.GetValueOrDefault(),
                    UseCustomN3 = x.UseCustomN3.GetValueOrDefault(),
                    UseCustomN4 = x.UseCustomN4.GetValueOrDefault(),
                    UseCustomA1 = x.UseCustomA1.GetValueOrDefault(),
                    UseCustomA2 = x.UseCustomA2.GetValueOrDefault(),
                    UseCustomA3 = x.UseCustomA3.GetValueOrDefault(),
                    UseCustomA4 = x.UseCustomA4.GetValueOrDefault(),
                    CustomN1Label = x.CustomN1Label,
                    CustomN2Label = x.CustomN2Label,
                    CustomN3Label = x.CustomN3Label,
                    CustomN4Label = x.CustomN4Label,
                    CustomA1Label = x.CustomA1Label,
                    CustomA2Label = x.CustomA2Label,
                    CustomA3Label = x.CustomA3Label,
                    CustomA4Label = x.CustomA4Label
                }).FirstOrDefault();
        }

        private XElement GetElement(string xml)
        {
            return XElement.Parse(xml);
        }

        public SubjectAndGradeDto GetSubjectAndGradeByVirtualTestId(int virtualTestId)
        {
            return _sgoDataContext.GetSubjectAndGradeByVirtualTestId(virtualTestId)
                .Select(o => new SubjectAndGradeDto
                {
                    SubjectId = o.SubjectID,
                    SubjectName = o.SubjectName,
                    GradeId = o.GradeID,
                    GradeName = o.GradeName
                }).FirstOrDefault();
        }
    }
}
