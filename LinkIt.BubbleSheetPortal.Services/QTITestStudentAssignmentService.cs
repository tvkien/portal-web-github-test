using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QTITestStudentAssignmentService
    {
        private readonly IQTITestStudentAssignmentRepository _testStudentAssignmentRepository;
        private readonly DistrictDecodeService districtDecodeService;
        private readonly ConfigurationService configurationService;

        public QTITestStudentAssignmentService(
            IQTITestStudentAssignmentRepository testStudentAssignmentRepository,
            DistrictDecodeService districtDecodeService,
            ConfigurationService configurationService)
        {
            _testStudentAssignmentRepository = testStudentAssignmentRepository;
            this.districtDecodeService = districtDecodeService;
            this.configurationService = configurationService;
        }

        public List<QTITestStudentAssignmentData> GetByQTITestClassAssignmentId(int qtiTestclassAssignmentId)
        {
            return _testStudentAssignmentRepository.Select().Where(x => x.QTITestClassAssignmentId == qtiTestclassAssignmentId).ToList();
        }

        public bool AssignStudent(QTITestStudentAssignmentData item)
        {
            if (item == null) return false;
            _testStudentAssignmentRepository.Save(item);
            return true;
        }

        public bool AssignStudents(List<QTITestStudentAssignmentData> items)
        {
            if (items == null || (items != null && items.Count() == 0)) return false;
            foreach (var item in items)
            {
                _testStudentAssignmentRepository.Save(item);
            }            
            return true;
        }

        public bool DeleteStudent(QTITestStudentAssignmentData item)
        {
            if (item == null) return false;
            _testStudentAssignmentRepository.Delete(item);
            return true;
        }

        public bool DeleteAllStudent(List<QTITestStudentAssignmentData> items)
        {
            if (items == null) return false;
            foreach (var item in items)
            {
                _testStudentAssignmentRepository.Delete(item);
            }            
            return true;
        }

        public bool CheckAbleToAssignStudentLevel(int districtId)
        {
            var studentLevelAssignmentOption = districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, DistrictDecodeLabel.StudentLevelAssignment).FirstOrDefault();
            bool studentLevelAssignmentOptionValue;
            if (studentLevelAssignmentOption != null && bool.TryParse(studentLevelAssignmentOption.Value, out studentLevelAssignmentOptionValue))
                return studentLevelAssignmentOptionValue;
            else
                return configurationService.GetConfigurationByKeyWithDefaultValue(ConfigurationKey.StudentLevelAssignment, true);

        }

        public void InsertMultipleRecord(List<QTITestStudentAssignmentData> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            _testStudentAssignmentRepository.InsertMultipleRecord(items);
        }
    }
}
