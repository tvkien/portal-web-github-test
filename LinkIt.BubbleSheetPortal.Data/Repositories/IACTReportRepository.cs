using System.Collections;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ACTReport;
using LinkIt.BubbleSheetPortal.Models.ACTSummaryReport;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IACTReportRepository
    {
        IList<ACTAnswerSectionData> ACTGetAnswerSectionData(int testResultID);
        IList<ACTSectionTagData> ACTGetSectionTagData(int studentID, int virtualTestSubTypeId);
        IList<ACTTestHistoryData> ACTGetTestHistoryData(int studentID, int virtualTestSubTypeId);
        ACTStudentInformation ACTGetStudentInformation(int studentID, int testResultID);

        IList<ACTSummaryClassLevelData> ACTSummaryGetClassLevelData(int classID, int virtualTestID, int virtualTestSubTypeId);
        IList<ACTSummaryClassLevelData> ACTSummaryGetClassLevelImprovementData(int classId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId);
        IList<ACTSummaryClassLevelData> ACTSummaryGetClassLevelBaselineData(int classId, List<int> virtualTestIds, int virtualTestSubTypeId);

        //IList<ACTSummaryDistrictLevelData> ACTSummaryGetDistrictLevelData(int districtID, int virtualTestID);

        IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetTeacherLevelData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetTeacherLevelAverageData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetTeacherLevelBaselineData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetTeacherLevelImprovementData(int userId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId);

        IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetSchoolLevelData(int schoolId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetSchoolLevelAverageData(int schoolId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetSchoolLevelBaselineData(int schoolId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetSchoolLevelImprovementData(int schoolId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId);

        IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetDistrictLevelData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetDistrictLevelAverageData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetDistrictLevelBaselineData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> ACTSummaryGetDistrictLevelImprovementData(int districtId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId);

        IList<ACTSummaryClassLevelData> SATSummaryGetClassLevelData(int classID, int virtualTestID, int virtualTestSubTypeId);
        IList<ACTSummaryClassLevelData> SATSummaryGetClassLevelBaselineData(int classId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummaryClassLevelData> SATSummaryGetClassLevelImprovementData(int classId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId);

        IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetTeacherLevelData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetTeacherLevelAverageData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetTeacherLevelBaselineData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetTeacherLevelImprovementData(int userId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId);

        IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetSchoolLevelData(int schoolId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetSchoolLevelAverageData(int schoolId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetSchoolLevelBaselineData(int schoolId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetSchoolLevelImprovementData(int schoolId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId);

        IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetDistrictLevelData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetDistrictLevelAverageData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetDistrictLevelBaselineData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId);
        IList<ACTSummarySchoolOrTeacherLevelData> SATSummaryGetDistrictLevelImprovementData(int districtId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId);

        IList<ACTAnswerSectionData> SATGetAnswerSectionData(int testResultID);
        IList<ACTSectionTagData> SATGetSectionTagData(int studentID, int virtualTestSubTypeID);
        IList<ACTTestHistoryData> SATGetTestHistoryData(int studentID, int virtualTestSubTypeID);
        ACTStudentInformation SATGetStudentInformation(int studentID, int testResultID);
        IList<ReportType> GetReportTypes(List<int> reportTypeIds);
        IList<ReportType> GetAllReportTypes();
    }
}