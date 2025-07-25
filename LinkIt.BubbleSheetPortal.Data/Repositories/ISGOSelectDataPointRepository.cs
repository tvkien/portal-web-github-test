using System;
using System.Collections;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ACTReport;
using LinkIt.BubbleSheetPortal.Models.ACTSummaryReport;
using LinkIt.BubbleSheetPortal.Models.DTOs.SGO;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Models.SGOManagement;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ISGOSelectDataPointRepository
    {
        List<GetStateStandardsForSGOData> GetStateStandardsForSGO(string stateCode, string subjectName, string gradeName, int userId, int virtualTestId);

        List<SGOGetAccessVirtualTestsData> SGOGetAccessVirtualTests(int? subjectId, string subjectName, int districtId,
            int? gradeId, int userId, int userRoleId);

        List<SGOGetTestResultSubScoreNameOfVirtualTestData> SGOGetTestResultSubScoreNameOfVirtualTest(int virtualTestId);

        List<SGOGetStudentDataPointData> SGOGetStudentDataPoint(int sgoId, int sgoDataPointId);

        void SGORemoveDataPointRelevantData(int sgoDataPointId);
        void SGOUpdateStudentDataPointRoster(int sgoId, int sgoDataPointId, int virtualTestId, DateTime resultDate, decimal pointsPossible, int createdBy, int modifiedBy, string studentDataPointXML);

        void SGOSaveStudentDataPointFromVirtualTest(int sgoId, int sgoDataPointId, int virtualTestId);

        List<SGOStudentTestData> SGOGetStudentTestData(int sgoId);
        List<SGOGetExternalVirtualTestData> SGOGetExternalVirtualTest(int userId, string subjectName, int gradeId);
        List<SGOGetExternalVirtualTestData> SGOGetExternalVirtualTestForProgressMonitoring(int sgoId, int userId, string subjectName, int gradeId);
        List<SGOGetVirtualTestCustomScoreData> SGOGetVirtualTestCustomScore(int sgoId,
            int virtualTestCustomScoreSettingId, bool? isTestTaken);

        VirtualTestCustomScore SGOGetAssessmentScoreType(int virtualTestId, int districtID = 0, int virtualTestCustomScoreId = 0, int sGOId = 0, bool? isPostAssignment = false, int sgoDataPointScoreType = 0);

        void SGORemoveTemporaryExternalTest(int virtualTestId);

        SubjectAndGradeDto GetSubjectAndGradeByVirtualTestId(int virtualTestId);
    }
}
