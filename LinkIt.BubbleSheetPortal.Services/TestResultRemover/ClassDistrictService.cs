using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Services.TestResultRemover
{
    public class ClassDistrictService
    {
        private readonly IReadOnlyRepository<ClassDistrict> repository;
        private readonly ITestResultRepository testResultRepository;
        private readonly IVirtualTestRepository virtualTestRepository;
        private readonly IQTITestClassAssignmentReadOnlyRepository qtiTestClassAssignmentReadOnlyRepository;
        private readonly IDocumentManagement documentManagement;
        private readonly IAnswerAttachmentRepository _answerAttachmentRepository;

        public ClassDistrictService(IReadOnlyRepository<ClassDistrict> repository,
            ITestResultRepository testResultRepository,
            IVirtualTestRepository virtualTestRepository,
            IQTITestClassAssignmentReadOnlyRepository qtiTestClassAssignmentReadOnlyRepository,
            IDocumentManagement documentManagement,
            IAnswerAttachmentRepository answerAttachmentRepository)
        {
            this.repository = repository;
            this.testResultRepository = testResultRepository;
            this.virtualTestRepository = virtualTestRepository;
            this.qtiTestClassAssignmentReadOnlyRepository = qtiTestClassAssignmentReadOnlyRepository;
            this.documentManagement = documentManagement;
            _answerAttachmentRepository = answerAttachmentRepository;
        }

        public IQueryable<ClassDistrict> GetClassByDistrictId(int districtId, bool isRegrader)
        {
            if (isRegrader)
            {
                return repository.Select().Where(x => x.DistrictId.Equals(districtId) && x.VirtualTestSourceId != 3).OrderBy(o => o.Name);
            }
            return repository.Select().Where(x => x.DistrictId.Equals(districtId)).OrderBy(o=>o.Name);
        }

        public bool DeleteTestResultAndSubItem (List<int> listTestResultIds, int userId)
        {
            try
            {
                foreach (int item in listTestResultIds)
                {
                    var testResult = testResultRepository.Select().FirstOrDefault(o => o.TestResultId == item);
                    if (testResult != null)
                    {
                        ReEvaluateBadgeForMOITest(testResult, item);
                        DeleteArtifact(testResult.QTIOnlineTestSessionID, userId);
                        testResultRepository.Delete(testResult);
                    }
                    else
                    {
                        return false;
                    }

                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteTestResultArtifacts(List<int> testResultIds, int userId)
        {
            try
            {
                var qtiOnlineTestSessionIDs = testResultRepository
                    .Select()
                    .Where(x => testResultIds.Contains(x.TestResultId) && x.QTIOnlineTestSessionID.HasValue)
                    .Select(x => x.QTIOnlineTestSessionID)
                    .ToList();

                foreach (int qtiOnlineTestSessionID in qtiOnlineTestSessionIDs)
                {
                    var answerAttachments = qtiTestClassAssignmentReadOnlyRepository.GetAnswerAttachments(qtiOnlineTestSessionID);

                    foreach(var attachment in answerAttachments)
                    {
                        _answerAttachmentRepository.DeleteAnswerAttachment(attachment.DocumentGuid.Value);
                        documentManagement.DeleteDocument(attachment.DocumentGuid.Value, userId);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ReEvaluateBadgeForMOITest(TestResult testResult, int testResultID)
        {
            var virtualTest =
                virtualTestRepository.Select()
                    .FirstOrDefault(x => x.VirtualTestID.Equals(testResult.VirtualTestId));
            if (virtualTest != null && (virtualTest.VirtualTestSubTypeID == (int) VirtualTestSubType.MOI || virtualTest.VirtualTestSubTypeID == (int) VirtualTestSubType.FDOI))
            {
                testResultRepository.ReEvaluateBadge(testResultID);
            }
        }

        private void DeleteArtifact(int? qtiOnlineTestSessionId, int userId)
        {
            if (!qtiOnlineTestSessionId.HasValue) return;

            var answerAttachments = qtiTestClassAssignmentReadOnlyRepository.GetAnswerAttachments(qtiOnlineTestSessionId.Value);

            foreach (var attachment in answerAttachments)
            {
                documentManagement.DeleteDocument(attachment.DocumentGuid.Value, userId);
            }
        }

        public void DeleteTestResultAndSubItemV2(IEnumerable<int> testResultIds)
        {
            var testResultIdsValid = testResultRepository.Select().Where(o => testResultIds.Contains(o.TestResultId)).Select(x => x.TestResultId).ToList();
            if (testResultIdsValid.Any())
            {
                testResultRepository.ReEvaluateBadgeV2(testResultIdsValid.ToIntCommaSeparatedString());
                var listGuids = _answerAttachmentRepository.GetDocumentGuids(testResultIds);
                listGuids = listGuids.Where(x => x.HasValue && x.Value != Guid.Empty).ToList();
                documentManagement.DeleteDocuments(listGuids);
                testResultRepository.DeleteTestResultAndSubItemsV2(testResultIdsValid.ToIntCommaSeparatedString());
            }
        }
    }
}
