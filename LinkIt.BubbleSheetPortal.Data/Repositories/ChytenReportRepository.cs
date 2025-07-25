using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ChytenReportRepository : IChytenReportRepository
    {
        private readonly TestDataContext _testContext;
        private readonly Table<ChytenTestCenterEmailEntity> table;

        public ChytenReportRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<ChytenTestCenterEmailEntity>();
            _testContext = TestDataContext.Get(connectionString);
            _testContext.CommandTimeout = 180; //increase timeout value for dbcontext in report module
        }

        public IList<ListItem> GetBankByDistrictId(int districtId, string schoolIdString, int userId, int roleId)
        {
            return
                _testContext.GetBankByDistrictId(districtId, schoolIdString, userId, roleId)
                    .Select(x => new ListItem() {Id = x.BankID, Name = x.Name})
                    .ToList();
        }
        public IList<ListItem> GetSchoolByDistrictIdAndBankId(int districtId, int bankId, string schoolIdString, int userId, int roleId)
        {
            return
                _testContext.GetSchoolByDistrictIdAndBankId(districtId, bankId, schoolIdString, userId, roleId)
                    .Select(x => new ListItem() { Id = x.SchoolID, Name = x.NAME })
                    .ToList();
        }
        public IList<ListItem> GetTeacherByDistrictIdAndBankIdAndSchoolId(int districtId, int bankId, int schoolId, string schoolIdString, int userId, int roleId)
        {
            return
                _testContext.GetTeacherByDistrictIdAndBankIdAndSchoolId(districtId, bankId, schoolId, schoolIdString, userId, roleId)
                    .Select(x => new ListItem() { Id = x.UserID, Name = x.TeacherName })
                    .ToList();
        }
        public IList<ListItem> GetClassedHaveTestResult(int districtId, int bankId, int schoolId, int teacherId, int termId, string schoolIdString, int userId, int roleId)
        {
            return
                _testContext.GetClassesHaveTestResult(districtId, bankId, schoolId, teacherId, termId, schoolIdString, userId, roleId)
                    .Select(x => new ListItem() { Id = x.ClassID, Name = x.NAME })
                    .ToList();
        }
        public IList<ListItem> GetTermsHaveTestResult(int districtId, int bankId, int schoolId, int teacherId, string schoolIdString, int userId, int roleId)
        {
            return
                _testContext.GetTermsHaveTestResult(districtId, bankId, schoolId, teacherId, schoolIdString, userId, roleId)
                    .Select(x => new ListItem() { Id = x.DistrictTermID, Name = x.NAME })
                    .ToList();
        }
        public IList<SpecializedTestResult> GetTestResultFilter(int districtId, int bankId, int schoolId, int teacherId, int classId, int termId, string schoolIdString, int userId, int roleId)
        {
            return
                _testContext.SpecializedGetAllTestResultIdByFilter(districtId, bankId, schoolId, teacherId, classId, termId, schoolIdString, userId, roleId)
                    .Select(x => new SpecializedTestResult()
                                 {
                                     ClassNameCustom = x.ClassNameCustom, 
                                     SchoolName = x.SchoolName, 
                                     BankId = x.BankID,
                                     BankName = x.Name, 
                                     StudentCustom = x.StudentNameCustom, 
                                     StudentCode = x.StudentCode, 
                                     StudentId = x.studentID, 
                                     TeacherCustom = x.TeacherCustom
                                 }).ToList();
        }
        public IList<string> GetTestCenterZipCodesByEmail(string email)
        {
            return table.Where(x => x.Email == email).Select(x => x.Zipcode).ToList();
        }

        public IList<int?> GetTestCenterSchoolIdsByEmail(string email)
        {
            return table.Where(x => x.Email == email).Select(x => x.SchoolID).ToList();
        }

        public bool CheckTestCenterInActive(string zipcode)
        {
            var result = table.Where(x => x.Zipcode == zipcode && x.Active == false);
            if (result.Any())
                return true;
            return false;
        }

        public bool CheckTestCenterActive(string zipcode)
        {
            var result = table.Where(x => x.Zipcode == zipcode && x.Active == true);
            if (result.Any())
                return true;
            return false;
        }

        public int? GetSchoolIdTestCenter(string zipcode)
        {
            var result = table.Where(x => x.Zipcode == zipcode).ToList();
            if (result.Any())
                return result.First().SchoolID;
            return null;
        }
    }
}