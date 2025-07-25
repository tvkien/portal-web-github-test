using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ACTReport;
using LinkIt.BubbleSheetPortal.Models.ACTSummaryReport;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ChytenReportService
    {
        private readonly IChytenReportRepository chytenReportRepository;

        public ChytenReportService(IChytenReportRepository chytenReportRepository)
        {
            this.chytenReportRepository = chytenReportRepository;
        }

        public List<ListItem> GetBankByDistrictId(int districtId, string schoolIdString, int userId, int roleId)
        {
            return chytenReportRepository.GetBankByDistrictId(districtId, schoolIdString, userId, roleId).ToList();
        }

        public IList<ListItem> GetSchoolByDistrictIdAndBankId(int districtId, int bankId, string schoolIdString, int userId, int roleId)
        {
            return chytenReportRepository.GetSchoolByDistrictIdAndBankId(districtId, bankId, schoolIdString, userId, roleId);
        }

        public IList<ListItem> GetTeacherByDistrictIdAndBankIdAndSchoolId(int districtId, int bankId, int schoolId, string schoolIdString, int userId, int roleId)
        {
            return chytenReportRepository.GetTeacherByDistrictIdAndBankIdAndSchoolId(districtId, bankId, schoolId, schoolIdString, userId, roleId);
        }

        public IList<ListItem> GetClassedHaveTestResult(int districtId, int bankId, int schoolId, int teacherId,
            int termId, string schoolIdString, int userId, int roleId)
        {
            return chytenReportRepository.GetClassedHaveTestResult(districtId, bankId, schoolId, teacherId, termId, schoolIdString, userId, roleId);
        }

        public IList<ListItem> GetTermsHaveTestResult(int districtId, int bankId, int schoolId, int teacherId, string schoolIdString, int userId, int roleId)
        {
            return chytenReportRepository.GetTermsHaveTestResult(districtId, bankId, schoolId, teacherId, schoolIdString, userId, roleId);
        }

        public IList<SpecializedTestResult> GetTestResultFilter(int districtId, int bankId, int schoolId, int teacherId,
            int classId, int termId, string schoolIdString, int userId, int roleId)
        {
            return chytenReportRepository.GetTestResultFilter(districtId, bankId, schoolId, teacherId, classId, termId, schoolIdString, userId, roleId);
        }

        public IList<string> GetTestCenterZipCodesByEmail(string email)
        {
            return chytenReportRepository.GetTestCenterZipCodesByEmail(email);
        }

        public IList<int?> GetTestCenterSchoolIdsByEmail(string email)
        {
            return chytenReportRepository.GetTestCenterSchoolIdsByEmail(email);
        }

        public bool CheckTestCenterInActive(string zipcode)
        {
            return chytenReportRepository.CheckTestCenterInActive(zipcode);
        }
        public bool CheckTestCenterActive(string zipcode)
        {
            return chytenReportRepository.CheckTestCenterActive(zipcode);
        }
        public int? GetSchoolIdTestCenter(string zipcode)
        {
            return chytenReportRepository.GetSchoolIdTestCenter(zipcode);
        }
    }
}