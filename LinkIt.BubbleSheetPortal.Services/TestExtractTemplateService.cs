using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TestExtractTemplateService
    {
        private readonly ITestExtractTemplateRepository _repository;

        public TestExtractTemplateService(ITestExtractTemplateRepository repository)
        {
            _repository = repository;
        }

        public List<TestExtractTemplate> GetAllTemplates()
        {
            return _repository.Select().OrderBy(o => o.DisplayOrder).ToList();
        }

        public TestExtractTemplate GetExtractTemplateById(int id)
        {
            return _repository.Select().FirstOrDefault(o => o.ID == id);
        }

        public List<ListItem> GetGradeHaveTestResult(ExtractLocalCustom filter)
        {
            return _repository.GetGradeHaveTestResult(filter);
        }

        public List<ListSubjectItem> GetSubjectHaveTestResult(ExtractLocalCustom filter)
        {
            return _repository.GetSubjectHaveTestResult(filter);
        }

        public List<ListItem> GetBankHaveTestResult(ExtractLocalCustom filter)
        {
            return _repository.GetBankHaveTestResult(filter);
        }

        public List<ListItem> GetTestHaveTestResult(ExtractLocalCustom filter)
        {
            return _repository.GetTestHaveTestResult(filter);
        }

        public List<ListItem> GetGradeHaveTest(ExtractLocalCustom filter)
        {
            return _repository.GetGradeHaveTest(filter);
        }

        public List<ListSubjectItem> GetSubjectHaveTest(ExtractLocalCustom filter)
        {
            return _repository.GetSubjectHaveTest(filter);
        }

        public List<ListItem> GetBankHaveTest(ExtractLocalCustom filter)
        {
            return _repository.GetBankHaveTest(filter);
        }

        public List<ListItem> GetVirtualTests(ExtractLocalCustom filter)
        {
            return _repository.GetVirtualTests(filter);
        }

        public List<ListItem> GetSchoolHaveTestResult(ExtractLocalCustom filter)
        {
            return _repository.GetSchoolHaveTestResult(filter);
        }

        public List<ListItem> GetSchoolHaveUser(int districtId, int userId, int roleId)
        {
            return _repository.GetSchoolHaveUser(districtId, userId, roleId);
        }

        public List<ListItem> GetTeacherHaveTestResult(ExtractLocalCustom filter)
        {
            return _repository.GetTeacherHaveTestResult(filter);
        }

        public List<ListItem> GetClassHaveTestResult(ExtractLocalCustom filter)
        {
            return _repository.GetClassHaveTestResult(filter);
        }

        public List<StudentMeta> GetStudentHaveTestResult(ExtractLocalCustom filter)
        {
            return _repository.GetStudentHaveTestResult(filter);
        }

        public List<ExportUserInformation> ExportUserInformation(string lstTestResultIds, int districtId)
        {
            return _repository.ExportUserInformation(lstTestResultIds, districtId);
        }

        public List<ExportTest> ExportTest(string lstTestResultIds, int districtId)
        {
            return _repository.ExportTest(lstTestResultIds, districtId);
        }

        public List<ExportQuestionTemplate> ExportQuestionTemplate(string lstTestResultIds, int districtId)
        {
            return _repository.ExportQuestionTemplate(lstTestResultIds, districtId);
        }

        public List<ExportTestResultTemplate> ExportTestResultTemplate(string lstTestResultIds, int districtId)
        {
            return _repository.ExportTestResultTemplate(lstTestResultIds, districtId);
        }

        public List<ExportPointsEarnedData> ExportPointsEarned(string listTestResultIDs, int districtID)
        {
            return _repository.ExportPointsEarned(listTestResultIDs, districtID);
        }

        public List<ExportStudentResponseData> ExportStudentResponse(string listTestResultIDs, int districtID,
            string guid)
        {
            return _repository.ExportStudentResponse(listTestResultIDs, districtID, guid);
        }

        public List<ExportClassTestAssignmentData> ExportClassTestAssignment(string listTestResultIDs, int districtID)
        {
            return _repository.ExportClassTestAssignment(listTestResultIDs, districtID);
        }

        public List<ExportRosterData> ExportRoster(string listTestResultIDs, int districtID)
        {
            return _repository.ExportRoster(listTestResultIDs, districtID);
        }

        public List<ListItem> GetGradeHaveTestAssignment(ExtractLocalCustom filter)
        {
            return _repository.GetGradeHaveTestAssignment(filter);
        }

        public List<ListSubjectItem> GetSubjectHaveTestAssignment(ExtractLocalCustom filter)
        {
            return _repository.GetSubjectHaveTestAssignment(filter);
        }

        public List<ListItem> GetBankHaveTestAssignment(ExtractLocalCustom filter)
        {
            return _repository.GetBankHaveTestAssignment(filter);
        }

        public List<ListItem> GetVirtualTestsHaveTestAssignment(ExtractLocalCustom filter)
        {
            return _repository.GetVirtualTestsHaveTestAssignment(filter);
        }

        public List<ListItem> GetSchoolHaveTestAssignment(ExtractLocalCustom filter)
        {
            return _repository.GetSchoolHaveTestAssignment(filter);
        }

        public List<ListItem> GetTeacherHaveTestAssignment(ExtractLocalCustom filter)
        {
            return _repository.GetTeacherHaveTestAssignment(filter);
        }

        public List<ListItem> GetClassHaveTestAssignment(ExtractLocalCustom filter)
        {
            return _repository.GetClassHaveTestAssignment(filter);
        }

        public List<ListItem> GetTermHaveStudent(int districtId, int userId, int roleId)
        {
            return _repository.GetTermHaveStudent(districtId, userId, roleId);
        }

        public List<ListItem> GetSchoolHaveStudent(ExtractRosterCustom filter)
        {
            return _repository.GetSchoolHaveStudent(filter);
        }

        public List<ListItem> GetTeacherHaveStudent(ExtractRosterCustom filter)
        {
            return _repository.GetTeacherHaveStudent(filter);
        }

        public List<ListItem> GetClassHaveStudent(ExtractRosterCustom filter)
        {
            return _repository.GetClassHaveStudent(filter);
        }
    }
}
