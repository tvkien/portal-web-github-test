using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Models.Old.DataLockerForStudent;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models.Enum;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DataLockerForStudentService
    {
        private readonly IDataLockerForStudentRepository _dataLockerForStudentRepository;
        private readonly QTITestClassAssignmentService _qTITestClassAssignmentService;
        private readonly PreferencesService _preferencesService;
        private readonly IRepository<QTITestClassAssignmentData> _testClassAssignmentRepository;
        private readonly QTITestStudentAssignmentService _qTITestStudentAssignmentService;
        private readonly ClassStudentService _classStudentService;
        private readonly IDocumentManagement _documentManagement;

        public DataLockerForStudentService(
            IDataLockerForStudentRepository dataLockerForStudentRepository, QTITestClassAssignmentService qTITestClassAssignmentService,
            PreferencesService preferencesService, IRepository<QTITestClassAssignmentData> testClassAssignmentRepository,
            QTITestStudentAssignmentService qTITestStudentAssignmentService,
            ClassStudentService classStudentService,
            IDocumentManagement documentManagement)
        {
            _dataLockerForStudentRepository = dataLockerForStudentRepository;
            _qTITestClassAssignmentService = qTITestClassAssignmentService;
            _preferencesService = preferencesService;
            _testClassAssignmentRepository = testClassAssignmentRepository;
            _qTITestStudentAssignmentService = qTITestStudentAssignmentService;
            _classStudentService= classStudentService;
            _documentManagement = documentManagement;
        }

        public bool PublishDataLockerPreference(PublishFormToStudentModels model, int userId, int districtId, string code)
        {
            var lstStudentByClass = _classStudentService.GetClassStudentsByClassId(model.ClassID).Select(s => s.StudentId).ToList();

            if (lstStudentByClass == null || lstStudentByClass.Count == 0)
            {
                return false;
            }

            var lstStudentHasTestResult = _dataLockerForStudentRepository.GetStudentTestResultByVirtualTestAndClass(model.VirtualTestID, model.ClassID);

            if (lstStudentHasTestResult == null || lstStudentHasTestResult.Count == 0)
            {
                return false;
            }

            lstStudentByClass = lstStudentByClass.Where(w => lstStudentHasTestResult.Contains(w)).ToList();

            if (lstStudentByClass == null || lstStudentByClass.Count == 0)
            {
                return false;
            }

            var qTITestClassAssignmentData = _dataLockerForStudentRepository.GetQTITestClassAssignment(model.VirtualTestID, model.ClassID, int.Parse(AssignmentType.DataLocker.ToString("d")));

            if (qTITestClassAssignmentData == null)
            {
                qTITestClassAssignmentData = new QTITestClassAssignmentData
                {
                    VirtualTestId = model.VirtualTestID,
                    ClassId = model.ClassID,
                    AssignmentDate = DateTime.UtcNow,
                    Code = code,
                    CodeTimestamp = DateTime.UtcNow,
                    AssignmentGuId = Guid.NewGuid().ToString(),
                    Status = (int)QTITestClassAssignmentStatusEnum.Publish,
                    Type = (int)AssignmentType.DataLocker,
                    DistrictID = districtId
                };
            }
            else
            {
                qTITestClassAssignmentData.AssignmentDate = DateTime.UtcNow;
                qTITestClassAssignmentData.Status = (int)QTITestClassAssignmentStatusEnum.Publish;
            }

            _qTITestClassAssignmentService.Save(qTITestClassAssignmentData);

            SaveQTITestStudentAssignment (lstStudentByClass, qTITestClassAssignmentData.QTITestClassAssignmentId);


            var preferencesDetail = _preferencesService.GetPreferenceDataLockerPortalByLevelAndID(qTITestClassAssignmentData.QTITestClassAssignmentId, ContaintUtil.DataLockerPreferenceLevelPublish);

            if (preferencesDetail != null)
            {
                preferencesDetail.Value = JsonConvert.SerializeObject(model.ValueOject);
                preferencesDetail.UpdatedDate = DateTime.UtcNow;
                preferencesDetail.UpdatedBy = userId;
                
            }
            else
            {
                preferencesDetail = new Preferences
                {
                    Level = ContaintUtil.DataLockerPreferenceLevelPublish,
                    Id = qTITestClassAssignmentData.QTITestClassAssignmentId,
                    Label = ContaintUtil.DataLockerPreferencesSetting,
                    Value = JsonConvert.SerializeObject(model.ValueOject)
                };
            }

            _preferencesService.Save(preferencesDetail);
            return true;
        }

        private void SaveQTITestStudentAssignment(List<int> studentIds, int qTITestClassAssignmentId)
        {
            var qtiTestClassAssignmentDelete = _qTITestStudentAssignmentService.GetByQTITestClassAssignmentId(qTITestClassAssignmentId);

            if (qtiTestClassAssignmentDelete != null && qtiTestClassAssignmentDelete.Count > 0)
            {
                _qTITestStudentAssignmentService.DeleteAllStudent(qtiTestClassAssignmentDelete);
            }

            foreach (var item in studentIds)
            {
                var entity = new QTITestStudentAssignmentData()
                {
                    StudentId = item,
                    QTITestClassAssignmentId = qTITestClassAssignmentId
                };
                _qTITestStudentAssignmentService.AssignStudent(entity);
            }
        }
        public bool UnPublishDataLockerPreference(int virtualTestID, int classID)
        {
            bool result = true;
            var qTITestClassAssignmentData = _testClassAssignmentRepository.Select().FirstOrDefault(o =>
                o.VirtualTestId == virtualTestID &&
                o.ClassId == classID &&
                o.Type == (int)AssignmentType.DataLocker &&
                o.Status == (int)QTITestClassAssignmentStatusEnum.Publish);
            if (qTITestClassAssignmentData != null)
            {
                qTITestClassAssignmentData.Status = (int)QTITestClassAssignmentStatusEnum.Unpublish;
                _qTITestClassAssignmentService.Save(qTITestClassAssignmentData);
            }
            else
                result = false;

            return result;
        }

        public GetAttachmentForStudentResponse GetListAttachmentForStudents(GetDatalockerForStudentPaginationRequest request)
        {
            return _dataLockerForStudentRepository.GetListAttachmentForStudents(request);
        }

        public QTITestClassAssignmentData GetQTITestClassAssignment(int virtualTestID, int classID, int type)
        {
            return _dataLockerForStudentRepository.GetQTITestClassAssignment(virtualTestID, classID, type);
        }

        public List<TestResultScoreArtifact> SaveStudentArtifacts(SaveStudentAttachmentsParameters model, int userId)
        {
            var (testResultScoreArtifact, deletedDocumentGuids) = _dataLockerForStudentRepository.SaveStudentArtifacts(model, userId);
            if (deletedDocumentGuids.Any())
            {
                _documentManagement.DeleteDocuments(deletedDocumentGuids);
            }
           
            return testResultScoreArtifact;
        }
    }
}
