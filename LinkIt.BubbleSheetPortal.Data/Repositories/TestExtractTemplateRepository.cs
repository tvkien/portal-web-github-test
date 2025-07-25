using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TestExtractTemplateRepository : ITestExtractTemplateRepository
    {
        private readonly Table<TestExtractTemplateEntity> table;
        private readonly ExtractTestDataContext _testDataContext;
        private readonly TestDataContext _testContext;
        private readonly IManageTestRepository _manageTestRepository;
        public TestExtractTemplateRepository(IConnectionString conn, IManageTestRepository manageTestRepository)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<TestExtractTemplateEntity>();
            _testDataContext = ExtractTestDataContext.Get(connectionString);
            _manageTestRepository = manageTestRepository;
            _testContext = TestDataContext.Get(connectionString);

            Mapper.CreateMap<TestExtractTemplate, TestExtractTemplateEntity>();
        }

        public IQueryable<TestExtractTemplate> Select()
        {
            return table.Select(x => new TestExtractTemplate
                    {
                        CreateDate = x.CreateDate,
                        DisplayOrder = x.DisplayOrder,
                        ID = x.ID,
                        Name = x.Name
                    });
        }

        public void Save(TestExtractTemplate item)
        {
            var entity = table.FirstOrDefault(x => x.ID.Equals(item.ID));

            if (entity.IsNull())
            {
                entity = new TestExtractTemplateEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.ID = entity.ID;
        }

        public void Delete(TestExtractTemplate item)
        {
            if (item.IsNotNull())
            {
                //TODO:
            }
        }

        public List<ListItem> GetGradeHaveTestResult(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetGradeHaveTestResult(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(),filter.UserId,filter.UserRoleId).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.GradeID,
                    Name = o.Name
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListSubjectItem> GetSubjectHaveTestResult(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetSubjectHaveTestResult(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.GradeId, filter.UserId,filter.UserRoleId).ToList();
            if (query.Count > 0)
                return query.GroupBy(s => s.Name).Select(x => new ListSubjectItem { Id = x.Key, Name = x.Key }).OrderBy(x => x.Name).ToList();
            return new List<ListSubjectItem>();
        }

        public List<ListItem> GetBankHaveTestResult(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetBankHaveTestResult(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.GradeId, filter.SubjectName,filter.UserId,filter.UserRoleId).ToList();

            var result = new List<ListItem>();
            if (query.Count > 0)
                result = query.Select(o => new ListItem()
                {
                    Id = o.BankID,
                    Name = o.Name
                }).ToList();

            var userBankAccess = _manageTestRepository.GetUserBankAccess(new UserBankAccessCriteriaDTO {
                UserId = filter.UserId,
                DistrictId = filter.DistrictId,
                SubjectId = filter.SubjectId,
                GradeIds = new List<int> { filter.GradeId },
                SubjectNames = new List<string> { filter.SubjectName }
            });

            var bankIds =_testContext.TestEntities.Where(m => userBankAccess.BankIncludeIds.Contains(m.BankID)
                        && m.TestResultEntities.Count > 0 && m.TestResultEntities.Any(h => h.ResultDate >= filter.StartDate && h.ResultDate <= filter.EndDate))
                        .Select(m => m.BankID).ToList();

            // include
            var includeBanks = _testContext.BankEntities.Where(m => userBankAccess.BankIncludeIds.Contains(m.BankID)
                                    && bankIds.Contains(m.BankID)).Select(o => new ListItem()
            {
                Id = o.BankID,
                Name = o.Name
            }).ToList();

            result.AddRange(includeBanks);
            result = result.DistinctBy(m => m.Id).ToList();
            // exclude
            result = result.Where(m => !userBankAccess.BankExcludeIds.Contains(m.Id)).ToList();

            return result;
        }

        public List<ListItem> GetTestHaveTestResult(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetVirtualTestHaveTestResult(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.BankdId, filter.UserId, filter.UserRoleId).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.VirtualTestID,
                    Name = o.Name
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListItem> GetGradeHaveTest(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetGradeHaveTest(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.UserId, filter.UserRoleId).ToList();
            if (query.Count > 0)
                return query.OrderBy(x => x.Order).Select(o => new ListItem()
                {
                    Id = o.GradeID,
                    Name = o.Name
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListSubjectItem> GetSubjectHaveTest(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetSubjectHaveTest(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.UserId, filter.UserRoleId, filter.GradeId).ToList();
            if (query.Count > 0)
                return query.GroupBy(s => s.Name).Select(x => new ListSubjectItem { Id = x.Key, Name = x.Key }).OrderBy(x => x.Name).ToList();

            return new List<ListSubjectItem>();
        }

        public List<ListItem> GetBankHaveTest(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetBankHaveTest(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.UserId, filter.UserRoleId, filter.GradeId, filter.SubjectId, filter.SubjectName).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.BankID,
                    Name = o.Name
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListItem> GetVirtualTests(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetVirtualTest(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.BankdId).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.VirtualTestID,
                    Name = o.Name
                }).ToList();
            return new List<ListItem>();
        }
        public List<ListItem> GetSchoolHaveTestResult(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetSchoolsHaveTestResult(filter.DistrictId, filter.StartDate.ToShortDateString(),
               filter.EndDate.AddDays(1).ToShortDateString(), filter.BankdId, filter.ListTestIDs,filter.UserId,filter.UserRoleId).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.SchoolID,
                    Name = o.NAME
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListItem> GetSchoolHaveUser(int districtId, int userId, int roleId)
        {
            var query = _testDataContext.extrGetSchoolsHaveUser(districtId, userId, roleId).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.SchoolID,
                    Name = o.NAME
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListItem> GetTeacherHaveTestResult(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetTeachersHaveTestResult(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.SchoolId, filter.BankdId, filter.ListTestIDs,filter.UserId,filter.UserRoleId).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.UserID,
                    Name = o.TeacherName
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListItem> GetClassHaveTestResult(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetClassesHaveTestResult(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.SchoolId, filter.TeacherId, filter.BankdId, filter.ListTestIDs,filter.UserId,filter.UserRoleId).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.ClassID,
                    Name = o.NAME
                }).ToList();
            return new List<ListItem>();
        }

        public List<StudentMeta> GetStudentHaveTestResult(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetStudentsHaveTestResult(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.SchoolId, filter.TeacherId, filter.ClassId, filter.BankdId, filter.ListTestIDs,filter.UserId,filter.UserRoleId).ToList();
            if (query.Count > 0)
                return query.Select(o => new StudentMeta()
                {
                    StudentID = o.StudentID,
                    Name = o.StudentName,
                    Code = o.Code
                }).ToList();
            return new List<StudentMeta>();
        }

        public List<ExportUserInformation> ExportUserInformation(string lstTestResultIds, int districtId)
        {
            var query = _testDataContext.ExportUserInformation(lstTestResultIds, districtId).ToList();
            return query.Select(o => new ExportUserInformation()
            {
                CreatedDate = o.CreatedDate.GetValueOrDefault(),
                Email = o.Email,
                ModifiedDate = o.ModifiedDate.GetValueOrDefault(),
                NameFirst = o.NameFirst,
                NameLast = o.NameLast,
                SchoolID = o.SchoolID.GetValueOrDefault(),
                SchoolName = o.SchoolName,
                UserID = o.UserID.GetValueOrDefault(),
                Username = o.Username
            }).ToList();
        }

        public List<ExportTest> ExportTest(string lstTestResultIds, int districtId)
        {
            var query = _testDataContext.ExportTest(lstTestResultIds, districtId).ToList();
            return query.Select(o => new ExportTest()
            {
                BankName = o.BankName,
                Grade = o.Grade,
                Subject = o.Subject,
                TestName = o.TestName
            }).ToList();
        }

        public List<ExportQuestionTemplate> ExportQuestionTemplate(string lstTestResultIds, int districtId)
        {
            var query = _testDataContext.ExportQuestionTemplate(lstTestResultIds, districtId).ToList();
            return query.Select(o => new ExportQuestionTemplate()
            {
                Name = o.name,
                Other = o.Other,
                PointsPossible = o.PointsPossible,
                QuestionOrder = o.QuestionOrder,
                Skill = o.Skill,
                Standard = o.Standard,
                Topic = o.Topic
            }).ToList();
        }

        public List<ExportTestResultTemplate> ExportTestResultTemplate(string lstTestResultIds, int districtId)
        {
            var query = _testDataContext.ExportTestResultTemplate(lstTestResultIds, districtId).ToList();
            return query.Select(o => new ExportTestResultTemplate()
            {
                ClassName = o.ClassName,
                Code = o.Code,
                DistrictTermName = o.DistrictTermName,
                Email = o.Email,
                FirstName = o.FirstName,
                LastName = o.LastName,
                ResultDate = o.ResultDate,
                SchoolName = o.SchoolName,
                SchoolID = o.SchoolID,
                ScoreRaw = o.ScoreRaw.GetValueOrDefault(),
                StatusName = o.StatusName,
                StudentID = o.StudentID,
                TestName = o.TestName,
                TimeSettingValue = o.TimeSettingValue,
                UserID = o.UserID,
                Username = o.Username
            }).ToList();
        }

        public List<ExportPointsEarnedData> ExportPointsEarned(string listTestResultIDs, int districtID)
        {
            return _testDataContext.ExportPointsEarned(listTestResultIDs, districtID).Select(
                x => new ExportPointsEarnedData
                     {
                         ClassName = x.ClassName,
                         PointsEarned = x.PointsEarned,
                         PointsPossible = x.PointsPossible,
                         QuestionOrder = x.QuestionOrder,
                         SchoolID = x.SchoolID,
                         SchoolName = x.SchoolName,
                         StudentID = x.StudentID,
                         Term = x.Term,
                         TestName = x.TestName,
                         TestResultID = x.TestResultID,
                         UserID = x.UserID,
                         UserName = x.UserName,
                         WasAnswered = x.WasAnswered
                     }).ToList();
        }

        public List<ExportStudentResponseData> ExportStudentResponse(string listTestResultIDs, int districtID, string guid)
        {
            return _testDataContext.ExportStudentResponse(listTestResultIDs, districtID, guid).Select(
                x => new ExportStudentResponseData
                     {
                         SchoolID = x.SchoolID,
                         SchoolName = x.SchoolName,
                         StudentID = x.StudentID,
                         Term = x.Term,
                         TestName = x.TestName,
                         TestResultID = x.TestResultID,
                         UserID = x.UserID,
                         UserName = x.UserName,
                         ClassName = x.ClassName,
                         AnswerLetterString = x.AnswerLetterString ?? string.Empty
                     }).ToList();
        }

        public List<ExportClassTestAssignmentData> ExportClassTestAssignment(string listTestResultIDs, int districtID)
        {
            return _testDataContext.ExportClassTestAssignmentsTemplate(listTestResultIDs, districtID).Select(
                x => new ExportClassTestAssignmentData
                     {
                         SchoolName = x.SchoolName,
                         UserID = x.UserID,
                         ClassName = x.ClassName,
                         SchoolID = x.SchoolID,
                         Assigned = x.Assigned,
                         Code = x.Code,
                         VirtualTestID = x.VirtualtestID,
                         Test = x.Test,
                         Username = x.Username,
                         NotStarted = x.NotStarted ?? 0,
                         Completed = x.Completed ?? 0,
                         Started = x.Started ?? 0,
                         WaitingForReview = x.WaitingForReview ?? 0
                     }).ToList();
        }

        public List<ExportRosterData> ExportRoster(string listTestResultIDs, int districtID)
        {
            return _testDataContext.ExportRosterTemplate(listTestResultIDs, districtID).Select(
                x => new ExportRosterData
                     {
                         UserID = x.UserID,
                         ClassName = x.ClassName,
                         SchoolID = x.SchoolID,
                         SchoolName = x.SchoolName,
                         StudentID = x.StudentID,
                         Term = x.Term,
                         ClassCreated = x.ClassCreated,
                         Course = x.Course,
                         Email = x.Email,
                         //FirstName = x.FirstName,
                         Gender = x.Gender,
                         Grade = x.Grade,
                         //LastName = x.LastName,
                         LocationCode = x.LocationCode,
                         ParentEmail = x.ParentEmail,
                         ParentFirstName = x.ParentFirstName,
                         ParentLastName = x.ParentLastName,
                         ParentPhone = x.ParentPhone,
                         Program = x.Program,
                         Race = x.Race,
                         Section = x.Section,
                         StudentCreate = x.StudentCreate,
                         StudentModified = x.StudentModified,
                         Username = x.Username
                     }).ToList();
        }

        public List<ListItem> GetGradeHaveTestAssignment(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetGradeHaveTestAssignment(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.UserId, filter.UserRoleId).ToList();
            if (query.Count > 0)
                return query.OrderBy(x => x.Order).Select(o => new ListItem()
                {
                    Id = o.GradeID,
                    Name = o.Name
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListSubjectItem> GetSubjectHaveTestAssignment(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetSubjectHaveTestAssignment(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.UserId, filter.UserRoleId, filter.GradeId).ToList();
            if (query.Count > 0)
                return query.GroupBy(s => s.Name).Select(x => new ListSubjectItem { Id = x.Key, Name = x.Key }).OrderBy(x => x.Name).ToList();

            return new List<ListSubjectItem>();
        }

        public List<ListItem> GetBankHaveTestAssignment(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetBankHaveTestAssignment(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.UserId, filter.UserRoleId, filter.GradeId, filter.SubjectId, filter.SubjectName).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.BankID,
                    Name = o.Name
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListItem> GetVirtualTestsHaveTestAssignment(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetVirtualTestHaveTestAssignment(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.BankdId, filter.UserId, filter.UserRoleId).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.VirtualTestID,
                    Name = o.Name
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListItem> GetSchoolHaveTestAssignment(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetSchoolsHaveTestAssignment(filter.DistrictId, filter.StartDate.ToShortDateString(),
               filter.EndDate.AddDays(1).ToShortDateString(), filter.BankdId, filter.ListTestIDs, filter.UserId, filter.UserRoleId).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.SchoolID,
                    Name = o.NAME
                }).ToList();
            return new List<ListItem>();
        }
        public List<ListItem> GetTeacherHaveTestAssignment(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetTeachersHaveTestAssignment(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.SchoolId, filter.BankdId, filter.ListTestIDs, filter.UserId, filter.UserRoleId).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.UserID,
                    Name = o.TeacherName
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListItem> GetClassHaveTestAssignment(ExtractLocalCustom filter)
        {
            var query = _testDataContext.extrGetClassesHaveTestAssignment(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.SchoolId, filter.TeacherId, filter.BankdId, filter.ListTestIDs, filter.UserId, filter.UserRoleId).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.ClassID,
                    Name = o.NAME
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListItem> GetTermHaveStudent(int districtId, int userId, int roleId)
        {
            var query = _testDataContext.extrGetTermHaveStudent(districtId, userId, roleId).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.DistrictTermID,
                    Name = o.Name
                }).ToList();
            return new List<ListItem>();
        }
        public List<ListItem> GetSchoolHaveStudent(ExtractRosterCustom filter)
        {
            var query = _testDataContext.extrGetSchoolHaveStudent(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.UserId, filter.UserRoleId).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.SchoolID,
                    Name = o.Name
                }).ToList();
            return new List<ListItem>();
        }
        public List<ListItem> GetTeacherHaveStudent(ExtractRosterCustom filter)
        {
            var query = _testDataContext.extrGetTeacherHaveStudent(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.SchoolId, filter.UserId, filter.UserRoleId).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.UserID,
                    Name = o.TeacherName
                }).ToList();
            return new List<ListItem>();
        }
        public List<ListItem> GetClassHaveStudent(ExtractRosterCustom filter)
        {
            var query = _testDataContext.extrGetClassHaveStudent(filter.DistrictId, filter.StartDate.ToShortDateString(),
                filter.EndDate.AddDays(1).ToShortDateString(), filter.SchoolId, filter.TeacherId, filter.UserId, filter.UserRoleId).ToList();
            if (query.Count > 0)
                return query.Select(o => new ListItem()
                {
                    Id = o.ClassID,
                    Name = o.Name
                }).ToList();
            return new List<ListItem>();
        }
    }
}
