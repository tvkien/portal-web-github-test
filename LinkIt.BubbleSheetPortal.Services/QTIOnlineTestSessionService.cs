using System;
using System.Globalization;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;
using LinkIt.BubbleSheetPortal.Services.TestResultRemover;
using LinkIt.BubbleSheetPortal.Common.Enum;
using System.Xml.Linq;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QTIOnlineTestSessionService
    {
        private const int AutoGraderStatusNotProcess = 0;
        private const int AutoGraderStatusFailedProcess = -1;

        private readonly IQTIOnlineTestSessionRepository repository;
        private readonly IAutoGradingQueueRepository _autoGradingQueueDataRepository;
        private readonly VirtualTestDistrictService _virtualTestDistrictService;
        private readonly TestResultLogService _testResultLogService;
        private readonly TestResultService _testResultService;
        private readonly ConfigurationService _configurationService;
        private readonly PreferencesService _preferencesService;

        public QTIOnlineTestSessionService(IQTIOnlineTestSessionRepository repository, IAutoGradingQueueRepository autoGradingQueueDataRepository,
            VirtualTestDistrictService virtualTestDistrictService, TestResultLogService testResultLogService
            , TestResultService testResultService, ConfigurationService configurationService,
            PreferencesService preferencesService)
        {
            this.repository = repository;
            _autoGradingQueueDataRepository = autoGradingQueueDataRepository;
            _virtualTestDistrictService = virtualTestDistrictService;
            _testResultLogService = testResultLogService;
            _testResultService = testResultService;
            _configurationService = configurationService;
            _preferencesService = preferencesService;
        }

        public IQueryable<QTIOnlineTestSession> Select()
        {
            return repository.Select();
        }

        public QTIOnlineTestSession GetQTIOnlineTestSessionByID(int qtiOnlineTestSessionID)
        {
            var result = repository.Select().FirstOrDefault(o => o.QTIOnlineTestSessionId == qtiOnlineTestSessionID);
            return result;
        }
        public List<QTIOnlineTestSession> GetListQTIOnlineTestSession(string assignmentGuid)
        {
            var result = repository.Select().Where(o => o.AssignmentGUId == assignmentGuid).ToList();
            return result;
        }
        public List<int> PauseOnlineTests(List<int?> qtiOnlineTestSessionIDs)
        {
            var listPauseSuccess = new List<int>();
            if (qtiOnlineTestSessionIDs == null) return listPauseSuccess;

            foreach (var qtiOnlineTestSessionID in qtiOnlineTestSessionIDs)
            {
                if (qtiOnlineTestSessionID == null) continue;

                var qtiOnlineTestSession = repository.Select().FirstOrDefault(o => o.QTIOnlineTestSessionId == qtiOnlineTestSessionID);

                if (qtiOnlineTestSession == null) continue;

                if (qtiOnlineTestSession.StatusId == (int)QTIOnlineTestSessionStatusEnum.Created || qtiOnlineTestSession.StatusId == (int)QTIOnlineTestSessionStatusEnum.InProgress)
                {
                    qtiOnlineTestSession.StatusId = (int)QTIOnlineTestSessionStatusEnum.Paused;
                    repository.Save(qtiOnlineTestSession);
                    listPauseSuccess.Add(qtiOnlineTestSession.QTIOnlineTestSessionId);
                }
            }

            return listPauseSuccess;
        }

        public void SubmitOnlineTests(List<int?> qtiOnlineTestSessionIDs)
        {
            if (qtiOnlineTestSessionIDs == null) return;
            foreach (var qtiOnlineTestSessionID in qtiOnlineTestSessionIDs)
            {
                if (qtiOnlineTestSessionID == null) continue;
                var queue =
                    _autoGradingQueueDataRepository.Select()
                        .FirstOrDefault(o => o.QTIOnlineTestSessionID == qtiOnlineTestSessionID && o.Status == AutoGraderStatusNotProcess);
                if (queue != null) continue;

                var qtiOnlineTestSession =
                    repository.Select().FirstOrDefault(o => o.QTIOnlineTestSessionId == qtiOnlineTestSessionID);
                if (qtiOnlineTestSession == null) return;

                if (qtiOnlineTestSession.StatusId == (int)QTIOnlineTestSessionStatusEnum.Completed
                 || qtiOnlineTestSession.StatusId == (int)QTIOnlineTestSessionStatusEnum.PendingReview)
                    continue;

                qtiOnlineTestSession.StatusId = 5;
                repository.Save(qtiOnlineTestSession);

                var autoGradingQueue = new AutoGradingQueueData
                {
                    QTIOnlineTestSessionID = qtiOnlineTestSessionID.Value,
                    CreatedDate = DateTime.UtcNow,
                    Status = AutoGraderStatusNotProcess,
                    ForceGrading = false,
                    Type = 1
                };
                _autoGradingQueueDataRepository.Save(autoGradingQueue);
            }
        }

        public void ReopenTest(int qtiOnlineTestSessionID, List<ImageIndexInQuestion> imageIndexs)
        {
            var images = XmlUtils.BuildXml(imageIndexs);
            repository.ReopenTest(qtiOnlineTestSessionID, string.Join(",", images));
        }

        public void RedoFailedGradingProcess(List<int?> qtiOnlineTestSessionIDs)
        {
            if (qtiOnlineTestSessionIDs == null) return;
            foreach (var qtiOnlineTestSessionID in qtiOnlineTestSessionIDs)
            {
                if (qtiOnlineTestSessionID == null) continue;
                var queue =
                    _autoGradingQueueDataRepository.Select()
                        .Where(o => o.QTIOnlineTestSessionID == qtiOnlineTestSessionID
                        && o.Status == AutoGraderStatusFailedProcess && !o.IsAwaitingRetry)
                        .OrderByDescending(o => o.AutoGradingQueueID).FirstOrDefault();
                if (queue == null) continue;

                queue.Status = AutoGraderStatusNotProcess;

                _autoGradingQueueDataRepository.Save(queue);
            }
        }

        public bool CanReopenTest(int qtiOnlineTestSessionID)
        {
            return repository.CanReopenTest(qtiOnlineTestSessionID);
        }

        public void LogTestResultDataBeforeRemove(TestResultAuditModel model, string type = ContaintUtil.Remover)
        {
            TestResult testResult = null;
            if (model.TestResultID > 0)
            {
                testResult = _testResultService.GetTestResultById(model.TestResultID);
            }
            else
            {
                testResult = _testResultService.GetTestResultByTestSessionId(model.QTIOnlineTestSessionID);
            }

            if (testResult == null) return;
            model.TestResultID = testResult.TestResultId;

            var testResultIDStr = model.TestResultID.ToString(CultureInfo.InvariantCulture);

            var testResultAudit = CreateTestResultAudit(model.VisitorsIPAddr, model.TestResultID, model.UserID, type);
            _testResultLogService.Save(testResultAudit);

            var testResultLog =
                _virtualTestDistrictService.GetTestResultDetails(testResultIDStr).ToList();
            _testResultLogService.Save(testResultLog);

            var testResultScoreLogs = _virtualTestDistrictService.GetTestResultScores(testResultIDStr);
            _testResultLogService.Save(testResultScoreLogs);

            var testResultScoreIds = testResultScoreLogs.Select(x => x.TestResultScoreID);
            var listTestResultScoreIds = string.Join(",", testResultScoreIds);
            var testResultSubScoreLogs = _virtualTestDistrictService.GetTestResultSubScores(listTestResultScoreIds);
            _testResultLogService.Save(testResultSubScoreLogs);

            var testResultProgamLogs = _virtualTestDistrictService.GetTestResultProgram(testResultIDStr);
            _testResultLogService.Save(testResultProgamLogs);

            var answers = _virtualTestDistrictService.GetAnswersByTestResultId(testResultIDStr);
            _testResultLogService.Save(answers);

            var answerIds = answers.Select(x => x.AnswerID);
            var listAnswerIds = string.Join(",", answerIds);
            var answerSubLogs = _virtualTestDistrictService.GetAnswerSubsByAnswerId(listAnswerIds);

            _testResultLogService.Save(answerSubLogs);
        }

        private TestResultAudit CreateTestResultAudit(string visitorsIPAddr, int testResultID, int userID, string type)
        {
            var testResultAudit = new TestResultAudit
            {
                TestResultId = testResultID,
                AuditDate = DateTime.Now,
                IPAddress = visitorsIPAddr,
                UserId = userID,
                Type = type
            };

            return testResultAudit;
        }

        public AutoGradingQueueData GetAutoGradingQueueByQTOnlineTestSessionID(int qTIOnlineTestSessionID)
        {
            return _autoGradingQueueDataRepository.GetAutoGradingQueueByQTOnlineTestSessionID(qTIOnlineTestSessionID);
        }
        public IEnumerable<AutoGradingQueueData> GetAutoGradingQueueByQTOnlineTestSessionID(IEnumerable<int> qTIOnlineTestSessionIds)
        {
            return _autoGradingQueueDataRepository.GetAutoGradingQueueByQTOnlineTestSessionID(qTIOnlineTestSessionIds);
        }


        public void ReopenFailedGradingProcess(List<int?> qtiOnlineTestSessionIDs)
        {
            if (qtiOnlineTestSessionIDs == null) return;
            foreach (var qtiOnlineTestSessionID in qtiOnlineTestSessionIDs)
            {
                if (qtiOnlineTestSessionID == null) continue;

                var qtiOnlineTestSession =
                    repository.Select().FirstOrDefault(o => o.QTIOnlineTestSessionId == qtiOnlineTestSessionID);
                if (qtiOnlineTestSession == null) continue;
                qtiOnlineTestSession.StatusId = 2;
                repository.Save(qtiOnlineTestSession);
            }
        }

        public List<TestStudentSessionExportItem> GetTestStudentSessionsExport(GetTestStudentSessionExportRequest request)
        {
            var response = repository.GetTestStudentSessionsExport(request).ToList();

            var result = new List<TestStudentSessionExportItem>();

            foreach (var item in response)
            {
                var students = TransformToStudentObjects(item.Students);
                if (students == null) continue;
                foreach (var student in students)
                {
                    var exportItem = new TestStudentSessionExportItem
                    {
                        SchoolCode = item.SchoolCode,
                        SchoolName = item.SchoolName,
                        UserName = item.UserName,
                        UserCode = item.UserCode,
                        Term = item.Term,
                        ClassName = item.ClassName,
                        ClassSection = item.ClassSection,
                        CourseNumber = item.CourseNumber,
                        AssignmentDate = item.AssignmentDate,
                        TestCode = item.TestCode,
                        Grade = item.Grade,
                        FirstName = student.FirstName,
                        LastName = student.LastName,
                        MiddleName = student.MiddleName,
                        StudentLocalID = GetStudentLocalID(student),
                        StudentStateID = student.StudentStateID,
                        Gender = student.Gender,
                        TestName = item.TestName,
                        Race = student.Race,

                        TestSessionStatus = GetTestStudentSessionStatus(student.StatusID),
                    };
                    result.Add(exportItem);
                }
            }
            return result;
        }

        private string GetStudentLocalID(QTITestSessionStudent item)
        {
            var config = _configurationService.GetConfigurationByKey("StudentLocalProperty");
            if (config == null) return string.Empty;

            var studentLocalProperty = config.Value;
            var property = item.GetType().GetProperty(studentLocalProperty);

            return property == null ? string.Empty : property.GetValue(item).ToString();
        }

        private string GetTestStudentSessionStatus(int statusID)
        {
            if (statusID == 1)
                return "Created";
            if (statusID == 2)
                return "Started";
            if (statusID == 3)
                return "Paused";
            if (statusID == 4)
                return "Completed";
            if (statusID == 5)
                return "WaitingForReview";
            return "Not Started";
        }

        private List<QTITestSessionStudent> TransformToStudentObjects(string xmlData)
        {
            if (string.IsNullOrWhiteSpace(xmlData)) return new List<QTITestSessionStudent>();

            var xdoc = XDocument.Parse(string.Format("<Students>{0}</Students>", xmlData));

            var result = new List<QTITestSessionStudent>();

            foreach (var item in xdoc.Element("Students").Elements("S"))
            {
                var student = new QTITestSessionStudent
                {
                    FirstName = item.Element("FN") != null ? item.Element("FN").Value : string.Empty,

                    LastName = item.Element("LN") != null ? item.Element("LN").Value : string.Empty,

                    StudentStateID = item.Element("SSID") != null ? item.Element("SSID").Value : string.Empty,

                    StudentLocalID = item.Element("SLID") != null ? item.Element("SLID").Value : string.Empty,

                    StudentAltCode = item.Element("AltCode") != null ? item.Element("AltCode").Value : string.Empty,

                    Gender = item.Element("Gender") != null ? item.Element("Gender").Value : string.Empty,

                    Race = item.Element("Race") != null ? item.Element("Race").Value : string.Empty,

                    StatusID = item.Element("StatusID") != null ? int.Parse(item.Element("StatusID").Value) : 1,
                    StudentId = item.Element("StudentId") != null ? int.Parse(item.Element("StudentId").Value) : 1
                };

                result.Add(student);
            }

            return result;
        }

        public List<int> TeacherSubmitOnlineTests(List<int?> qtiOnlineTestSessionIDs, int requestUserID)
        {
            List<int> qtiOnlineSubmited = new List<int>();
            if (qtiOnlineTestSessionIDs == null) return qtiOnlineSubmited;
            foreach (var qtiOnlineTestSessionID in qtiOnlineTestSessionIDs)
            {
                if (qtiOnlineTestSessionID.HasValue)
                {
                    var autoGradingQueue = repository.SubmitOnlineTest(qtiOnlineTestSessionID.Value, false, string.Empty, 1, requestUserID);
                    if (autoGradingQueue != null)
                        qtiOnlineSubmited.Add(qtiOnlineTestSessionID.Value);
                }
            }

            return qtiOnlineSubmited;
        }

        public bool HasExistTestInProgress(int virtualTestId)
        {
            return repository.HasExistTestInProgress(virtualTestId);
        }
    }
}
