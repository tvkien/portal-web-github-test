using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ISGORepository : IRepository<SGOObject>
    {
        void SGOSaveStudentPopulate(SGOPopulateStudentFilter objFilter);
        List<ListItemExtra> GetStudentSelectedBySogId(int sgoId);

        void SGODeleteSGOGroupWithOut9988BySGOId(int sgoId);
        
        IQueryable<SGOCustomNew> GetSGOCustom(int districtId, int userId, int pageIndex, int pageSize, ref int? totalRecords,
            string sortColumns, string searchbox, int userRoleId, bool? isArchived, bool? isActive, int? schoolId, int? teacherId
            , int? reviewerId, int? districtTermId, string sgoStatusIds, DateTime? InstructionPeriodFrom, DateTime? InstructionPeriodTo);

        void SGOSaveStudent(int sgoId, string studentDataPointXML);
        List<SGOStudentScoreInDataPoint> GetStudentScorePreAssessmentLinkIt(int dataPointID);
        List<SGOStudentScoreInDataPoint> GetStudentScorePreAssessmentExternal(int dataPointID);
        List<SGOStudentScoreInDataPoint> GetStudentScorePreAssessmentLegacy(int dataPointID);
        void MoveStudentHasNoGroupToDefaultGroup(int SGOID);
        void AssignStudentToDataPointBand(int sgoDataPointBandID, string studentIDs);
        void AssignStudentToGroup(int sgoID, int sgoGroupID, string studentIDs);
        List<SGOAutoGroupStudentData> AutoAssignStudentToGroup(int sgoID, bool includeUpdate, int scoreType);
        void UpdateGroup(int sgoID, string groupNames);
        void DeleteDataPointBand(string dataPointBandIDs);

        IQueryable<ListItem> SGOGetDistictTerm(SGOPopulateStudentFilter obj);
        IQueryable<ListItem> SGOGetGender(SGOPopulateStudentFilter obj);
        IQueryable<ListItem> SGOGetRace(SGOPopulateStudentFilter obj);
        IQueryable<ListItem> SGOGetProgram(SGOPopulateStudentFilter obj);

        IQueryable<ListItem> SGOGetClasses(SGOPopulateStudentFilter obj);
        IQueryable<ListItemExtra> SGOGetStudents(SGOPopulateStudentFilter obj);

        void PopulateSchoolIdsAndClassIdsBySgoId(int sgoId);

        IQueryable<SGOStepObject> GetCompletedList(int sgoId);
        List<SGOScoreAchievmentData> GetScoreToCreateDefaultBandForHistoricalTest(int dataPointId);

        int SGOAuthorizeRevision(int sgoId, int userId, int statusId);
        void SGORelatedDataPoint(int oldDataPointId, int newSgoId);
        List<User> SGOGetAdminsOfUser(int districtID, int userID, int roleID);

        List<SGOCalculateScoreResult> GetSGOCalculateScoreResult(int sgoId, int sgoDataPointId);

        bool CheckScoreSGOResults(int sgoId, int datapointId);
        List<SGODataPoint> GetDataPointHasNoBand(int sgoId);
        List<SGOScoringDetail> GetSgoScoringDetail(int sgoId, int? sgoDataPointId);
        void PopulateDefaultAttainmentGroup(int sgoId);
        List<ListItemExtra> GetAllStudentsBySogId(int sgoId);

        List<SGOStudentScoreInDataPoint> GetStudentScorePreCustomAssessmentLinkIt(int dataPointID, int? virtualTestCustomSubScoreId);

        SGOCustomReport GetSGOCustomById(int sgoId);

        List<SGOReportDataPoint> GetSGOReportDataPoint(int sgoId);
        List<SGOReportDataPointFilter> GetSGOReportDataPointFilter(int sgoId);
        IQueryable<SGOExportData> SGOGetFinalAdministrativeSignoffSGO(int userId, DateTime? from, DateTime? to, bool? isArchire, bool? isActive, string sgoStatusIDs);
        bool GetAccessPermission(int districtId, int userId, int sgoId);
        List<SGOGetCandidateClass> GetCandidateClassForReplacement(int sgoId);
        List<SGOLoggingData> GetFullDataForLogging(int sgoId);
        void ApplyCandidateClassForReplacement(int sgoId, int removedClassId, int candidateClassId);
        bool CheckScoreTestResultStudent(int sgoId, int datapointId, int virtualTestId);
        bool IsExistStudentHasScoreNull(int sgoId, int datapointId);
    }
}
