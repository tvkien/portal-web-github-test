using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ITestExtractTemplateRepository : IRepository<TestExtractTemplate>
    {
        List<ListItem> GetGradeHaveTestResult(ExtractLocalCustom filter);
        List<ListSubjectItem> GetSubjectHaveTestResult(ExtractLocalCustom filter);
        List<ListItem> GetBankHaveTestResult(ExtractLocalCustom filter);
        List<ListItem> GetTestHaveTestResult(ExtractLocalCustom filter);

        List<ListItem> GetGradeHaveTest(ExtractLocalCustom filter);
        List<ListSubjectItem> GetSubjectHaveTest(ExtractLocalCustom filter);
        List<ListItem> GetBankHaveTest(ExtractLocalCustom filter);
        List<ListItem> GetVirtualTests(ExtractLocalCustom filter);

        List<ListItem> GetSchoolHaveTestResult(ExtractLocalCustom filter);
        List<ListItem> GetSchoolHaveUser(int districtId, int userId, int roleId);
        List<ListItem> GetTeacherHaveTestResult(ExtractLocalCustom filter);
        List<ListItem> GetClassHaveTestResult(ExtractLocalCustom filter);
        List<StudentMeta> GetStudentHaveTestResult(ExtractLocalCustom filter);

        List<ExportUserInformation> ExportUserInformation(string lstTestResultIds, int districtId);
        List<ExportTest> ExportTest(string lstTestResultIds, int districtId);
        List<ExportQuestionTemplate> ExportQuestionTemplate(string lstTestResultIds, int districtId);
        List<ExportTestResultTemplate> ExportTestResultTemplate(string lstTestResultIds, int districtId);
        List<ExportPointsEarnedData> ExportPointsEarned(string listTestResultIDs, int districtID);
        List<ExportStudentResponseData> ExportStudentResponse(string listTestResultIDs, int districtID, string guid);
        List<ExportClassTestAssignmentData> ExportClassTestAssignment(string listTestResultIDs, int districtID);
        List<ExportRosterData> ExportRoster(string listTestResultIDs, int districtID);

        List<ListItem> GetGradeHaveTestAssignment(ExtractLocalCustom filter);
        List<ListSubjectItem> GetSubjectHaveTestAssignment(ExtractLocalCustom filter);
        List<ListItem> GetBankHaveTestAssignment(ExtractLocalCustom filter);
        List<ListItem> GetVirtualTestsHaveTestAssignment(ExtractLocalCustom filter);
        List<ListItem> GetSchoolHaveTestAssignment(ExtractLocalCustom filter);
        List<ListItem> GetTeacherHaveTestAssignment(ExtractLocalCustom filter);
        List<ListItem> GetClassHaveTestAssignment(ExtractLocalCustom filter);
        List<ListItem> GetTermHaveStudent(int districtId, int userId, int roleId);
        List<ListItem> GetSchoolHaveStudent(ExtractRosterCustom filter);
        List<ListItem> GetTeacherHaveStudent(ExtractRosterCustom filter);
        List<ListItem> GetClassHaveStudent(ExtractRosterCustom filter);

    }
}
