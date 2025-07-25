using Amazon.ConfigService.Model;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.Old.DataLockerForStudent;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class DataLockerForStudentRepository : IDataLockerForStudentRepository
    {
        DataLockerContextDataContext _context;
        public DataLockerForStudentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = DataLockerContextDataContext.Get(connectionString);
            _context.CommandTimeout = 36000;
        }

        public GetAttachmentForStudentResponse GetListAttachmentForStudents(GetDatalockerForStudentPaginationRequest request)
        {
            IQueryable<TestResultScoreUploadFileModels> iQueryResultScoreArtifact = null;
            var users = _context.UserDLEntities.AsQueryable();
            var studentUser = _context.StudentUserDLEntities.AsQueryable();
            var classs = _context.ClassDLEntities.AsQueryable();
            var virtualTests = _context.VirtualTestDLEntities.AsQueryable();
            var qTITestClassAssignment = _context.QTITestClassAssignmentDLEntities.AsQueryable();
            var qTITestStudentAssignment = _context.QTITestStudentAssignmentDLEntities.AsQueryable();
            var testResults = _context.TestResultDLEntities.AsQueryable();
            var testResultScore = _context.TestResultScoreDLEntities.AsQueryable();
            var testResultSubScore = _context.TestResultSubScoreDLEntities.AsQueryable();
            var testResultScoreUploadFile = _context.TestResultScoreUploadFileDLEntities.AsQueryable();
            var virtualTestCustomScores = _context.VirtualTestCustomScoreDLEntities.AsQueryable();
            var virtualTest_VirtualTestCustomScores = _context.VirtualTest_VirtualTestCustomScoreDLEntities.AsQueryable();
            var virtualTestCustomSubScores = _context.VirtualTestCustomSubScoreEntities.AsQueryable();
            var virtualTestCustomMetaDatas = _context.VirtualTestCustomMetaDataDLEntities.AsQueryable();

            var preferences = _context.PreferenceDLEntities.AsQueryable().Where(w => w.Level == ContaintUtil.DataLockerPreferenceLevelPublish);

            int studentId = studentUser.FirstOrDefault(w => w.UserId == request.UserId).StudentId;
            var qTITestClassAssignmentIds = qTITestStudentAssignment.Where(w => w.StudentId == studentId).Select(s => s.QTITestClassAssignmentID).ToList();
            var virtualTestIds = qTITestClassAssignment.Where(w => qTITestClassAssignmentIds.Contains(w.QTITestClassAssignmentID)).Select(s => s.VirtualTestID).ToList();
            var testResultIds = testResults.Where(w => w.StudentID == studentId && virtualTestIds.Contains(w.VirtualTestID)).Select(s => s.TestResultID).ToList();
            var testResultScoreIds = testResultScore.Where(w => testResultIds.Contains(w.TestResultID)).Select(s => s.TestResultScoreID).ToList();
            var testResultSubScoreIds = testResultSubScore.Where(w => testResultScoreIds.Contains(w.TestResultScoreID)).Select(s => s.TestResultSubScoreID).ToList();

            if (!string.IsNullOrWhiteSpace(request.SearchString))
            {
                virtualTests = virtualTests.Where(w => w.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            if (testResultScoreIds != null && testResultScoreIds.Any())
            {

                iQueryResultScoreArtifact = testResultScore
                    .Join(virtualTest_VirtualTestCustomScores, tr => tr.TestResultDLEntity.VirtualTestID, vt_vtcs => vt_vtcs.VirtualTestID, (tr, vt_vtcs) => new { tr, vt_vtcs })
                    .Join(virtualTestCustomScores, x => x.vt_vtcs.VirtualTestCustomScoreID, vtcs => vtcs.VirtualTestCustomScoreID, (x, vtcs) => new { tr = x.tr, vtcs })
                    .Join(virtualTestCustomMetaDatas, x => x.vtcs.VirtualTestCustomScoreID, mtdt => mtdt.VirtualTestCustomScoreID, (x, mtdt) => new { tr = x.tr, vtcs = x.vtcs, mtdt })
                    .Where(w => testResultScoreIds.Contains(w.tr.TestResultScoreID) && w.mtdt.ScoreType == "Artifact" && w.mtdt.VirtualTestCustomSubScoreID == null).Select(n => new TestResultScoreUploadFileModels
                    {
                        TestResultScoreID = n.tr.TestResultScoreID,
                        TestResultSubScoreID = 0,
                        ArtifactName = "Overall Score",
                        TestResultID = n.tr.TestResultID,
                        MetaData = n.mtdt.MetaData,
                        Order = n.mtdt.Order
                    }).AsQueryable();
            }
            if (testResultSubScoreIds != null && testResultSubScoreIds.Any())
            {
                var testResultScoreUploadFileQuery = testResultScore.Join(testResultSubScore, resultScore => resultScore.TestResultScoreID, subScore => subScore.TestResultScoreID, (resultScore, subScore) => new { resultScore, subScore })
                        .Join(virtualTest_VirtualTestCustomScores, m => m.resultScore.TestResultDLEntity.VirtualTestID, vTestCusScore => vTestCusScore.VirtualTestID, (m, vTestCusScore) => new { resultScore = m.resultScore, subScore = m.subScore, vTestCusScore })
                        .Join(virtualTestCustomSubScores, m => m.vTestCusScore.VirtualTestCustomScoreID, vCusSubScore => vCusSubScore.VirtualTestCustomScoreID, (m, vCusSubScore) => new { resultScore = m.resultScore, subScore = m.subScore, vTestCusScore = m.vTestCusScore, vCusSubScore })
                        .Join(virtualTestCustomMetaDatas, m => m.vCusSubScore.VirtualTestCustomSubScoreID, virtualTestMeta => virtualTestMeta.VirtualTestCustomSubScoreID, (m, virtualTestMeta) => new { resultScore = m.resultScore, subScore = m.subScore, vCusSubScore = m.vCusSubScore, virtualTestMeta })
                        .Where(w => testResultSubScoreIds.Contains(w.subScore.TestResultSubScoreID) && w.virtualTestMeta.ScoreType == "Artifact"  && w.vCusSubScore.Name == w.subScore.Name)
                       .Select(n => new TestResultScoreUploadFileModels
                       {
                           TestResultScoreID = n.subScore.TestResultScoreID,
                           TestResultSubScoreID = n.subScore.TestResultSubScoreID,
                           ArtifactName = n.subScore.Name,
                           TestResultID = n.resultScore.TestResultID,
                           MetaData = n.virtualTestMeta.MetaData,
                           Order = n.virtualTestMeta.Order
                       }).AsQueryable();

                if (iQueryResultScoreArtifact != null)
                {
                    iQueryResultScoreArtifact = iQueryResultScoreArtifact.Union(testResultScoreUploadFileQuery);
                }
                else
                {
                    iQueryResultScoreArtifact = testResultScoreUploadFileQuery;
                }
            }

            if (iQueryResultScoreArtifact != null)
            {
                var resultScoreArtifacts = iQueryResultScoreArtifact
                                .Join(testResults, trf => trf.TestResultID, tr => tr.TestResultID, (trf, tr) => new { trf, tr })
                                .Join(virtualTests, group1 => group1.tr.VirtualTestID, vt => vt.VirtualTestID, (group1, vt) => new { trf = group1.trf, tr = group1.tr, vt })
                                .Join(qTITestClassAssignment,
                                    group2 => new { group2.tr.VirtualTestID, ClassID = group2.tr.ClassID.GetValueOrDefault(0) },
                                    qtta => new { qtta.VirtualTestID, qtta.ClassID },
                                    (group2, qtta) => new { trf = group2.trf, tr = group2.tr, vt = group2.vt, qtta })
                                .Join(preferences, group3 => group3.qtta.QTITestClassAssignmentID, pr => pr.ID, (group3, pr) => new { trf = group3.trf, tr = group3.tr, vt = group3.vt, qtta = group3.qtta, pr })
                                .Join(users, group4 => group4.tr.UserID, u => u.UserID, (group4, u) => new { trf = group4.trf, tr = group4.tr, vt = group4.vt, qtta = group4.qtta, pr = group4.pr, u })
                                .Join(classs, group5 => group5.tr.ClassID, c => c.ClassID, (group5, c) => new { trf = group5.trf, tr = group5.tr, vt = group5.vt, qtta = group5.qtta, pr = group5.pr, u = group5.u, c })
                                .Where(w => w.qtta.Status == (int)QTITestClassAssignmentStatusEnum.Publish)
                                .Select(z => new AttachmentForStudentModel
                                {
                                    ResultDate = z.tr.ResultDate,
                                    VirtualTestName = z.vt.Name,
                                    ClassName = z.c.Name,
                                    TeacherName = z.u.FullName,
                                    AttachmentName = z.trf.ArtifactName,
                                    Attachments = z.c.Name,
                                    ClassId = z.c.ClassID,
                                    TeacherID = z.tr.TeacherID,
                                    VirtualTestId = z.vt.VirtualTestID,
                                    FilterJson = z.pr.Value,
                                    TestResultScoreID = z.trf.TestResultScoreID,
                                    TestResultSubScoreID = z.trf.TestResultSubScoreID,
                                    Status = z.qtta.Status,
                                    MetaData = z.trf.MetaData,
                                    Preferences = JsonConvert.DeserializeObject<PreferencesValueJson>(z.pr.Value),
                                    PublishDate = z.pr.UpdatedDate,
                                    Order = z.trf.Order
                                })
                                .GroupBy(x => new { x.TestResultScoreID, x.TestResultSubScoreID })
                                .Select(x => x.FirstOrDefault())
                                .OrderBy(x => x.ResultDate).ThenByDescending(x => x.VirtualTestName);

                var response = new GetAttachmentForStudentResponse();
                var data = new List<AttachmentForStudentModel>();
                var artifactsIQueryable = testResultScoreUploadFile.AsQueryable();
                IQueryable<TestResultScoreUploadFileDLEntity> iQueryArtifacts = null;
                if (resultScoreArtifacts != null)
                {
                    int countBreak = request.PageSize + request.PageSize * request.StartRow;
                    foreach (var score in resultScoreArtifacts)
                    {
                        if (score.TestResultSubScoreID > 0)
                        {
                            iQueryArtifacts = artifactsIQueryable.Where(x => x.TestResultSubScoreID.Equals(score.TestResultSubScoreID.Value));
                        }
                        else
                        {
                            iQueryArtifacts = artifactsIQueryable.Where(x => x.TestResultScoreID.Equals(score.TestResultScoreID.Value));
                        }
                        if (artifactsIQueryable != null)
                        {
                            score.Artifacts = iQueryArtifacts.Select(x => new TestResultScoreArtifact()
                            {
                                TestResultScoreUploadFileID = x.TestResultScoreUploadFileID,
                                Name = x.FileName,
                                IsLink = x.IsUrl ?? false,
                                Url = x.IsUrl == true ? x.FileName : "",
                                TagValue = x.Tag,
                                DocumentGuid = x.DocumentGUID,
                                CreatedBy = x.CreatedBy
                            }).ToList();
                        }                        
                        string dateExpried = score.Preferences.ExpriedOn.DateExpried;
                        string timeExpried = score.Preferences.ExpriedOn.TimeExpried;
                        string dateDeadline = score.Preferences.ExpriedOn.DateDeadline;
                        string dateTimeJoin = string.Format("{0} {1}", dateExpried, timeExpried);
                        DateTime dateTimeCheck = request.CurrentDateDistrict.AddDays(1);
                        if (score.Preferences.ExpriedOn.TypeExpiredOn == ((int)PreferencePublishStudent.Deadline).ToString() && score.PublishDate.HasValue)
                        {                            
                            DateTime dateTimeOut;
                            DateTime.TryParse(dateTimeJoin, out dateTimeOut);
                            dateTimeCheck = dateTimeOut;
                        }
                        else if (score.Preferences.ExpriedOn.TypeExpiredOn == ((int)PreferencePublishStudent.Duration).ToString() && !string.IsNullOrWhiteSpace(dateExpried))
                        {
                            dateTimeCheck  = score.PublishDate.Value.AddDays(int.Parse(dateDeadline));
                        }
                        if (dateTimeCheck >= request.CurrentDateDistrict)
                        {
                            if (score.Preferences.ModificationUploadedArtifacts.AllowModification == "0")
                            {
                                if (!score.Artifacts.Any(a => a.CreatedBy.HasValue && a.CreatedBy.Value == request.UserId))
                                {
                                    data.Add(score);
                                }
                            }
                            else
                            {
                                data.Add(score);
                            }
                        }
                    }
                    response.TotalRecord = data.Count;
                    data = data.Skip(request.StartRow).Take(request.PageSize).ToList();
                }

                if (!string.IsNullOrEmpty(request.SortColumn) && !string.IsNullOrEmpty(request.SortDirection))
                {
                    switch (request.SortColumn)
                    {
                        case "ResultDate":
                            data = request.SortDirection.Equals("ASC")
                                ? data.OrderBy(x => x.ResultDate).ThenByDescending(o=>o.Order).ToList()
                                : data.OrderByDescending(x => x.ResultDate).ThenByDescending(o=>o.Order).ToList();
                            break;
                        case "VirtualTestName":
                            data = request.SortDirection.Equals("ASC")
                                ? data.OrderBy(x => x.VirtualTestName).ToList()
                                : data.OrderByDescending(x => x.VirtualTestName).ToList();
                            break;
                        default:
                            data = data.OrderBy(x => x.ResultDate).ThenByDescending(x => x.VirtualTestName).ToList();
                            break;
                    }
                }
                response.Data = data;
                return response;
            }
            return new GetAttachmentForStudentResponse();
        }

        public QTITestClassAssignmentData GetQTITestClassAssignment(int virtualTestID, int classID, int type)
        {
            var qTITestClassAssignmentData = _context.QTITestClassAssignmentDLEntities.AsQueryable().FirstOrDefault(w => w.VirtualTestID == virtualTestID && w.ClassID == classID && w.Type == type);
            if (qTITestClassAssignmentData != null)
            {
                return new QTITestClassAssignmentData()
                {
                    QTITestClassAssignmentId = qTITestClassAssignmentData.QTITestClassAssignmentID,
                    VirtualTestId = qTITestClassAssignmentData.VirtualTestID,
                    ClassId = qTITestClassAssignmentData.ClassID,
                    AssignmentDate = qTITestClassAssignmentData.AssignmentDate,
                    Code = qTITestClassAssignmentData.Code,
                    CodeTimestamp = qTITestClassAssignmentData.CodeTimestamp,
                    AssignmentGuId = qTITestClassAssignmentData.AssignmentGUID,
                    Status = qTITestClassAssignmentData.Status,
                    Type = qTITestClassAssignmentData.Type,
                    DistrictID = qTITestClassAssignmentData.DistrictID.Value
                };
            }
            return null;
        }
        public List<int> GetStudentTestResultByVirtualTestAndClass(int virtualTestID, int classID)
        {
            var testResults = _context.TestResultDLEntities.AsQueryable();
            return testResults.Where(w => w.VirtualTestID == virtualTestID && w.ClassID == classID).Select(s => s.StudentID).Distinct().ToList();
        }

        public (List<TestResultScoreArtifact>, List<Guid?>) SaveStudentArtifacts(SaveStudentAttachmentsParameters model, int userId)
        {
            var deletedDocumentGuids = new List<Guid?>();
            if (model.DeletedItems.Any())
            {
                
                var deletedIds = model.DeletedItems.Select(x => x.TestResultScoreUploadFileID).ToList();
                var list = _context.TestResultScoreUploadFileDLEntities
                    .Where(x => deletedIds.Contains(x.TestResultScoreUploadFileID));

                deletedDocumentGuids = list.Where(p => p.DocumentGUID.HasValue && p.DocumentGUID != Guid.Empty).Select(p => p.DocumentGUID).ToList();

                _context.TestResultScoreUploadFileDLEntities.DeleteAllOnSubmit(list);
                _context.TestResultScoreUploadFileDLEntities.Context.SubmitChanges();
            }

            if (model.AddedItems.Any())
            {
                var addModels = new List<TestResultScoreUploadFileDLEntity>();
                var dateNow = DateTime.UtcNow;
                foreach (var item in model.AddedItems)
                {
                    addModels.Add(new TestResultScoreUploadFileDLEntity()
                    {
                        CreatedBy = userId,
                        DocumentGUID = item.DocumentGuid,
                        TestResultScoreID = model.TestResultSubScoreID.HasValue ? null : model.TestResultScoreID,
                        TestResultSubScoreID = model.TestResultSubScoreID ?? null,
                        UpdatedDate = dateNow,
                        UploadedDate = dateNow,
                        IsUrl = item.IsUrl,
                        FileName = item.IsUrl == true ? item.Name : null
                    });
                }
                _context.TestResultScoreUploadFileDLEntities.InsertAllOnSubmit(addModels);
                _context.TestResultScoreUploadFileDLEntities.Context.SubmitChanges();
            }
            return (_context.TestResultScoreUploadFileDLEntities
                .Where(x => model.TestResultScoreID.Equals(x.TestResultScoreID) || model.TestResultSubScoreID.Equals(x.TestResultSubScoreID))
                .Select(x => new TestResultScoreArtifact()
                {
                    TestResultScoreUploadFileID = x.TestResultScoreUploadFileID,
                    Name = x.FileName,
                    IsLink = x.IsUrl ?? false,
                    Url = x.IsUrl == true ? x.FileName : "",
                    TagValue = x.Tag,
                    DocumentGuid = x.DocumentGUID,
                    CreatedBy = x.CreatedBy
                }).ToList(), deletedDocumentGuids);
        }
    }
}
