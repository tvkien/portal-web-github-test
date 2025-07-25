using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

using LinkIt.BubbleSheetPortal.Models.DTOs.SGO;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Models.SGOManagement;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SGOSelectDataPointService
    {
        private readonly ISGOSelectDataPointRepository _sGOSelectDataPointRepository;
        private readonly ISGODataPointRepository _sGODataPointRepository;

        public SGOSelectDataPointService(
            ISGOSelectDataPointRepository sGOSelectDataPointRepository,
            ISGODataPointRepository sGODataPointRepository)
        {
            _sGOSelectDataPointRepository = sGOSelectDataPointRepository;
            _sGODataPointRepository = sGODataPointRepository;
        }

        public IQueryable<SGOStudentTestData> GetSGOStudentTestData(int sgoId)
        {
            return _sGOSelectDataPointRepository.SGOGetStudentTestData(sgoId).AsQueryable();
        }

        public List<GetStateStandardsForSGOData> GetStateStandardsForSGO(string stateCode, string subjectName,
            string gradeName, int userId, int virtualTestId)

        {
            return  _sGOSelectDataPointRepository.GetStateStandardsForSGO(stateCode, subjectName, gradeName, userId,
                virtualTestId);            
        }

        public IQueryable<SGOGetAccessVirtualTestsData> GetAccessVirtualTests(int? subjectId, string subjectName,
            int districtId,
            int? gradeId, int userId, int userRoleId)
        {
            return
                _sGOSelectDataPointRepository.SGOGetAccessVirtualTests(subjectId, subjectName, districtId, gradeId,
                    userId, userRoleId).AsQueryable();
        }

        public IQueryable<SGOGetTestResultSubScoreNameOfVirtualTestData> GetTestResultSubScoreNameOfVirtualTest(int virtualTestId)
        {
            return
                _sGOSelectDataPointRepository.SGOGetTestResultSubScoreNameOfVirtualTest(virtualTestId).AsQueryable();
        }

        public IQueryable<SGOGetStudentDataPointData> GetStudentDataPoint(int sgoId, int virtualTestId)
        {
            return
                _sGOSelectDataPointRepository.SGOGetStudentDataPoint(sgoId, virtualTestId).AsQueryable();
        }

        public void RemoveDataPointRelevantData(int sgoDataPointId)
        {
            _sGOSelectDataPointRepository.SGORemoveDataPointRelevantData(sgoDataPointId);
        }

        public void SGORemoveTemporaryExternalTest(int virtualTestId)
        {
            _sGOSelectDataPointRepository.SGORemoveTemporaryExternalTest(virtualTestId);
        }

        public void UpdateStudentDataPointRoster(int sgoId, int sgoDataPointId, int virtualTestId, DateTime resultDate, decimal pointsPossible, int createdBy, int modifiedBy, string studentDataPointXML)
        {
            _sGOSelectDataPointRepository.SGOUpdateStudentDataPointRoster(sgoId, sgoDataPointId, virtualTestId, resultDate, pointsPossible, createdBy, modifiedBy, studentDataPointXML);
        }

        public void SaveStudentDataPointFromVirtualTest(int sgoId, int sgoDataPointId, int virtualTestId)
        {
            _sGOSelectDataPointRepository.SGOSaveStudentDataPointFromVirtualTest(sgoId, sgoDataPointId, virtualTestId);
        }

        public List<SGOGetExternalVirtualTestData> GetExternalVirtualTest(int userId, string subjectName, int gradeId)
        {
            return _sGOSelectDataPointRepository.SGOGetExternalVirtualTest(userId, subjectName, gradeId);
        }

        public List<SGOGetExternalVirtualTestData> GetExternalVirtualTestForProgressMonitoring(int sgoId, int userId, string subjectName, int gradeId)
        {
            return _sGOSelectDataPointRepository.SGOGetExternalVirtualTestForProgressMonitoring(sgoId, userId, subjectName, gradeId);
        }

        public List<SGOGetVirtualTestCustomScoreData> GetVirtualTestCustomScore(int sgoId, int virtualTestCustomScoreSettingId, bool? isTestTaken)
        {
            return _sGOSelectDataPointRepository.SGOGetVirtualTestCustomScore(sgoId, virtualTestCustomScoreSettingId, isTestTaken);
        }

        public List<ListItem> GetTestPreAssessmentHistorical(int sgoId, int dataSetCategoryID, string subjectName, int gradeId, int previewTestTeacherId)
        {
            var data = GetSGOStudentTestData(sgoId)
              .Where(x =>
                      x.DataSetCategoryID == dataSetCategoryID && x.SubjectName == subjectName &&
                      x.GradeId == gradeId && x.VirtualTestSourceId == 3
                      && x.BankAuthorId != previewTestTeacherId // Include External virtual test (created in preview test teacher banks)
                      )
              .Select(x => new { x.VirtualTestId, x.VirtualTestName }).Distinct().OrderBy(x => x.VirtualTestName)
              .Select(x => new ListItem
              {
                  Id = x.VirtualTestId,
                  Name = x.VirtualTestName
              });
            return data.ToList();
        }

        public List<ListItem> GetTestPostAssessmentHistorical(int sgoId, int dataSetCategoryID, string subjectName, int gradeId, int previewTestTeacherId)
        {
            var data = GetSGOStudentTestData(sgoId)
                .Where(x =>
                    x.DataSetCategoryID == dataSetCategoryID && x.SubjectName == subjectName &&
                    x.GradeId == gradeId && x.VirtualTestSourceId == 3
                    && x.BankAuthorId != previewTestTeacherId // Include External virtual test (created in preview test teacher banks)
                    )
                .Select(x => new {x.VirtualTestId, x.VirtualTestName}).Distinct().OrderBy(x => x.VirtualTestName)
                .Select(x => new ListItem
                {
                    Id = x.VirtualTestId,
                    Name = x.VirtualTestName
                });
            return data.ToList();
        }

        public VirtualTestCustomScore SGOGetAssessmentScoreType(int virtualTestId, int districtID = 0, int? virtualTestCustomScoreId = null, int sGOId = 0, bool? isPostAssignment = false, int sgoDataPointScoreType = 0)
        {
            return _sGOSelectDataPointRepository.SGOGetAssessmentScoreType(virtualTestId, districtID, virtualTestCustomScoreId ?? 0, sGOId, isPostAssignment, sgoDataPointScoreType);
        }

        public SubjectAndGradeDto GetSubjectAndGradeByVirtualTestId(int virtualTestId)
        {
            return _sGOSelectDataPointRepository.GetSubjectAndGradeByVirtualTestId(virtualTestId);
        }
    }}
