using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Audit;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.GradingShorcuts;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models.Monitoring;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestTaking;
using LinkIt.BubbleSheetPortal.Data.Repositories.Helper;
using System.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting;
using LinkIt.BubbleSheetPortal.Data.Repositories.MultipleTestResults;
using LinkIt.BubbleSheetPortal.Data.Extensions;
using System.Linq.Expressions;
using LinkIt.BubbleSheetPortal.Common.CustomException;
using LinkIt.BubbleSheetPortal.Models.DTOs.Survey;
using LinkIt.BubbleSheetPortal.Models.DTOs.TeacherReview;
using LinkIt.BubbleSheetPortal.Models.DTOs.ViewAttachment;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DTOs.Retake;
using LinkIt.BubbleSheetPortal.Services.CodeGen;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;
using System.Web.Configuration;
using System.ComponentModel;
using LinkIt.BubbleSheetPortal.Models.DTOs.Users;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QTITestClassAssignmentService
    {
        private const string NotStarted = "Not Started";

        private readonly IRepository<QTITestClassAssignmentData> _testClassAssignmentRepository;
        private readonly IRepository<QTITestStudentAssignmentData> _testStudentAssignmentRepository;
        private readonly IQTITestClassAssignmentReadOnlyRepository _testClassAssignmentReadOnlyRepository;
        private readonly IAutoGradingQueueRepository _autoGradingQueueDataRepository;
        private readonly IQTITestClassAssignmentRepository _iQTITestClassAssignmentRepository;
        private readonly IRepository<Answer> _answerRepository;
        private readonly IRepository<AnswerSubData> _answerSubDataRepository;
        private readonly IRepository<QTIOnlineTestSession> _qtiOnlineTestSessionRepository;
        private readonly IRepository<AnswerAuditData> _answerAuditRepository;
        private readonly IRepository<AnswerSubAuditData> _answerSubAuditRepository;
        private readonly IRepository<QTIOnlineTestSessionAnswerAuditData> _qtiOnlineTestSessionAnswerAuditRepository;
        private readonly IRepository<QTIOnlineTestSessionAnswerSubAuditData> _qtiOnlineTestSessionAnswerSubAuditRepository;
        private readonly IRepository<QtiOnlineTestSessionAnswer> _qtiOnlineTestSessionAnswerRepository;
        private readonly IRepository<QtiOnlineTestSessionAnswerSubData> _qtiOnlineTestSessionAnswerSubDataRepository;
        private readonly IBulkHelper _bulkHelper;
        private readonly QTIOnlineTestSessionAnswerTimeTrackDynamo _qTIOnlineTestSessionAnswerTimeTrackDynamoRepository;

        readonly IMultipleTestResultRepository _multipleTestResultRepository;
        readonly IRepository<Student> _studentRepository;

        private readonly GroupPrintingService _groupPrintingService;
        private readonly PreferencesService _preferencesService;
        private readonly QTIOnlineTestSessionService _qtiOnlineTestSessionService;
        private readonly ClassService _classService;
        private readonly SchoolService _schoolService;
        private readonly IAnswerAttachmentService _answerAttachmentService;
        private readonly IRepository<TestResult> _testResultRepository;
        private readonly IVirtualTestRepository _virtualTestRepository;
        private readonly QTITestStudentAssignmentService _qTITestStudentAssignmentService;
        private readonly ParentConnectService _parentConnectService;
        private readonly AuthenticationCodeGenerator _authenticationCodeGenerator;

        public QTITestClassAssignmentService(IRepository<QTITestClassAssignmentData> testClassAssignmentRepository,
            IQTITestClassAssignmentReadOnlyRepository testClassAssignmentReadOnlyRepository,
            IQTITestClassAssignmentRepository iQTITestClassAssignmentRepository,
            IRepository<QTITestStudentAssignmentData> testStudentAssignmentRepository,
            IAutoGradingQueueRepository autoGradingQueueDataRepository,
            IRepository<Answer> answerRepository,
            IRepository<AnswerSubData> answerSubDataRepository,
            IRepository<QTIOnlineTestSession> qtiOnlineTestSessionRepository,
            IRepository<AnswerAuditData> answerAuditRepository,
            IRepository<AnswerSubAuditData> answerSubAuditRepository,
            IRepository<QTIOnlineTestSessionAnswerAuditData> qtiOnlineTestSessionAnswerAuditRepository,
            IRepository<QTIOnlineTestSessionAnswerSubAuditData> qtiOnlineTestSessionAnswerSubAuditRepository,
            IRepository<QtiOnlineTestSessionAnswer> qtiOnlineTestSessionAnswerRepository,
            IRepository<QtiOnlineTestSessionAnswerSubData> qtiOnlineTestSessionAnswerSubDataRepository,
            IBulkHelper bulkHelper,
            QTIOnlineTestSessionAnswerTimeTrackDynamo qTIOnlineTestSessionAnswerTimeTrackDynamoRepository,
            IMultipleTestResultRepository multipleTestResultRepository,
            IRepository<Student> studentRepository,
            GroupPrintingService groupPrintingService,
            ClassStudentService classStudentService,
            QTIOnlineTestSessionService qtiOnlineTestSessionService,
            ClassService classService,
            SchoolService schoolService,
            PreferencesService preferencesService,
            IAnswerAttachmentService answerAttachmentService,
            IRepository<TestResult> testResultRepository,
            IVirtualTestRepository virtualTestRepository,
            QTITestStudentAssignmentService qTITestStudentAssignmentService,
            ParentConnectService parentConnectService,
            AuthenticationCodeGenerator authenticationCodeGenerator)
        {
            _testClassAssignmentRepository = testClassAssignmentRepository;
            _testClassAssignmentReadOnlyRepository = testClassAssignmentReadOnlyRepository;
            _testStudentAssignmentRepository = testStudentAssignmentRepository;
            _autoGradingQueueDataRepository = autoGradingQueueDataRepository;
            _answerRepository = answerRepository;
            _answerSubDataRepository = answerSubDataRepository;
            _qtiOnlineTestSessionRepository = qtiOnlineTestSessionRepository;
            _answerAuditRepository = answerAuditRepository;
            _answerSubAuditRepository = answerSubAuditRepository;
            _qtiOnlineTestSessionAnswerAuditRepository = qtiOnlineTestSessionAnswerAuditRepository;
            _qtiOnlineTestSessionAnswerSubAuditRepository = qtiOnlineTestSessionAnswerSubAuditRepository;
            _qtiOnlineTestSessionAnswerRepository = qtiOnlineTestSessionAnswerRepository;
            _qtiOnlineTestSessionAnswerSubDataRepository = qtiOnlineTestSessionAnswerSubDataRepository;
            this._bulkHelper = bulkHelper;
            _qTIOnlineTestSessionAnswerTimeTrackDynamoRepository = qTIOnlineTestSessionAnswerTimeTrackDynamoRepository;
            _iQTITestClassAssignmentRepository = iQTITestClassAssignmentRepository;
            _multipleTestResultRepository = multipleTestResultRepository;
            _studentRepository = studentRepository;
            _groupPrintingService = groupPrintingService;
            _preferencesService = preferencesService;
            _answerAttachmentService = answerAttachmentService;
            _qtiOnlineTestSessionService = qtiOnlineTestSessionService;
            _classService = classService;
            _schoolService = schoolService;
            _testResultRepository = testResultRepository;
            _virtualTestRepository = virtualTestRepository;
            _qTITestStudentAssignmentService = qTITestStudentAssignmentService;
            _parentConnectService = parentConnectService;
            _authenticationCodeGenerator = authenticationCodeGenerator;
        }

        public bool InValidCode(string code)
        {
            return _testClassAssignmentRepository.Select().Any(o => o.Code.Equals(code));
        }

        public QTITestClassAssignmentData GetAssignmentByTestCode(string testCode)
        {
            return _testClassAssignmentRepository.Select().FirstOrDefault(x => x.Code == testCode);
        }

        public bool AssignClass(QTITestClassAssignmentData item)
        {
            if (item == null) return false;
            _testClassAssignmentRepository.Save(item);
            return true;
        }

        public bool Save(QTITestClassAssignmentData item)
        {
            if (item == null) return false;
            _testClassAssignmentRepository.Save(item);
            return true;
        }

        public void ChangeStatus(int assignmentID, int status, int userID, List<int> students = null, int type = 0, bool IsOldUI = false)
        {
            var classAssignment =
                _testClassAssignmentRepository.Select().FirstOrDefault(o => o.QTITestClassAssignmentId == assignmentID);
            if (classAssignment == null) return;
            if (type == (int)AssignmentType.Roster)
            {
                //insert student when add after assign class
                this.AddStudentAfterAssignClass(classAssignment);
            }
            var studentClassAssignments = new List<QTITestStudentAssignmentData>();
            if (!IsOldUI)
            {               
                if (type == (int)AssignmentType.Student || (type == (int)AssignmentType.Class && students.Count == 0) || (type == (int)AssignmentType.Roster && students.Count == 0))
                {
                    studentClassAssignments = _testStudentAssignmentRepository.Select().Where(w => w.QTITestClassAssignmentId == assignmentID).ToList();
                }
                else
                {
                    studentClassAssignments = _testStudentAssignmentRepository.Select().Where(w => w.QTITestClassAssignmentId == assignmentID && students.Contains(w.StudentId)).ToList();
                }
                if (studentClassAssignments != null && studentClassAssignments.Count() > 0)
                {
                    foreach (var studentClassAssignment in studentClassAssignments)
                    {
                        studentClassAssignment.Status = status;
                        _testStudentAssignmentRepository.Save(studentClassAssignment);
                    }
                }
                if (type == (int)AssignmentType.Roster)
                {
                    var studentClassAssignmentOutsite = _testStudentAssignmentRepository.Select().Where(w => w.QTITestClassAssignmentId == assignmentID);
                    var studentIds = studentClassAssignmentOutsite != null && studentClassAssignmentOutsite.Any() ? studentClassAssignmentOutsite.Select(s => s.StudentId).ToList() : new List<int>();
                    var studentIdsInsert = students.Where(w => !studentIds.Contains(w)).ToList();
                    //insert db
                    var objs = studentIdsInsert.Select(s => new QTITestStudentAssignmentData()
                    {
                        StudentId = s,
                        QTITestClassAssignmentId = assignmentID,
                        Status = status,
                        IsHide = classAssignment.IsHide
                    }).ToList();
                    _qTITestStudentAssignmentService.AssignStudents(objs);
                }
                if (status == 0)
                {
                    classAssignment.Status = GetStatusForClassAssignment(assignmentID, status, classAssignment.ClassId, classAssignment.Type);
                }
                else
                {
                    classAssignment.Status = 1;
                }
            }
            else
            {
                studentClassAssignments = _testStudentAssignmentRepository.Select().Where(w => w.QTITestClassAssignmentId == assignmentID).ToList();
                if (studentClassAssignments != null && studentClassAssignments.Count() > 0)
                {
                    foreach (var studentClassAssignment in studentClassAssignments)
                    {
                        studentClassAssignment.Status = status;
                        _testStudentAssignmentRepository.Save(studentClassAssignment);
                    }
                }
                classAssignment.Status = status;
            }
                       
            classAssignment.ModifiedBy = "Portal";
            classAssignment.ModifiedDate = DateTime.UtcNow;
            classAssignment.ModifiedUserId = userID;
            _testClassAssignmentRepository.Save(classAssignment);
        }

        public void ChangeStatusMultipleAssignment(List<int> assignmentIDs, int status, int userID, bool IsOldUI = false)
        {
            var assignments =
                _testClassAssignmentRepository.Select().Where(o => assignmentIDs.Contains(o.QTITestClassAssignmentId)).ToList();
            if (assignments == null || assignments.Count == 0) return;

            foreach (var assignment in assignments)
            {
                var studentClassAssignments = new List<QTITestStudentAssignmentData>();
                if (!IsOldUI)
                {
                    studentClassAssignments = _testStudentAssignmentRepository.Select().Where(w => w.QTITestClassAssignmentId == assignment.QTITestClassAssignmentId).ToList();
                    if (studentClassAssignments != null && studentClassAssignments.Count() > 0)
                    {
                        foreach (var studentClassAssignment in studentClassAssignments)
                        {
                            studentClassAssignment.Status = status;
                            _testStudentAssignmentRepository.Save(studentClassAssignment);
                        }
                    }
                    if (assignment.Type == (int)AssignmentType.Roster)
                    {
                        var studentIdOld = studentClassAssignments.Select(s => s.StudentId).Distinct().ToList();
                        if (studentIdOld != null && studentIdOld.Any())
                        {
                            var studentClassAssignmentOutsite = _testStudentAssignmentRepository.Select().Where(w => w.QTITestClassAssignmentId == assignment.QTITestClassAssignmentId);
                            var studentIds = studentClassAssignmentOutsite != null && studentClassAssignmentOutsite.Any() ? studentClassAssignmentOutsite.Select(s => s.StudentId).ToList() : new List<int>();
                            var studentIdsInsert = studentIds.Where(w => !studentIdOld.Contains(w)).ToList();
                            if (studentIdsInsert != null && studentIdsInsert.Any())
                            {
                                //insert db
                                var objs = studentIdsInsert.Select(s => new QTITestStudentAssignmentData()
                                {
                                    StudentId = s,
                                    QTITestClassAssignmentId = assignment.QTITestClassAssignmentId,
                                    Status = status,
                                    IsHide = assignment.IsHide
                                }).ToList();
                                _qTITestStudentAssignmentService.AssignStudents(objs);
                            }                            
                        }                        
                    }
                }
                else
                {
                    studentClassAssignments = _testStudentAssignmentRepository.Select().Where(w => w.QTITestClassAssignmentId == assignment.QTITestClassAssignmentId).ToList();
                    if (studentClassAssignments != null && studentClassAssignments.Count() > 0)
                    {
                        foreach (var studentClassAssignment in studentClassAssignments)
                        {
                            studentClassAssignment.Status = status;
                            _testStudentAssignmentRepository.Save(studentClassAssignment);
                        }
                    }
                }
            }
            
            assignments.ForEach(x =>
            {
                x.Status = status;
                x.ModifiedBy = "Portal";
                x.ModifiedDate = DateTime.UtcNow;
                x.ModifiedUserId = userID;
            });

            _iQTITestClassAssignmentRepository.SaveMutipleRecord(assignments);
        }

        public void ChangeStatusShowHide(int assignmentID, bool isHide, int userID, List<int> students = null, int type = 0, bool IsOldUI = false)
        {
            var assignment =
                _testClassAssignmentRepository.Select().FirstOrDefault(o => o.QTITestClassAssignmentId == assignmentID);
            if (assignment == null) return;
            if (type == (int)AssignmentType.Roster)
            {
                //insert student when add after assign class
                this.AddStudentAfterAssignClass(assignment);
            }
            var studentClassAssignments = new List<QTITestStudentAssignmentData>();
            if (!IsOldUI)
            {
                if (type == (int)AssignmentType.Student || (type == (int)AssignmentType.Class && students.Count == 0) || (type == (int)AssignmentType.Roster && students.Count == 0))
                {
                    studentClassAssignments = _testStudentAssignmentRepository.Select().Where(w => w.QTITestClassAssignmentId == assignmentID).ToList();
                }
                else
                {
                    studentClassAssignments = _testStudentAssignmentRepository.Select().Where(w => w.QTITestClassAssignmentId == assignmentID && students.Contains(w.StudentId)).ToList();
                }
                if (studentClassAssignments != null && studentClassAssignments.Count() > 0)
                {
                    foreach (var studentClassAssignment in studentClassAssignments)
                    {
                        studentClassAssignment.IsHide = isHide;
                        _testStudentAssignmentRepository.Save(studentClassAssignment);
                    }
                }
                if (type == (int)AssignmentType.Roster)
                {
                    var studentClassAssignmentOutsite = _testStudentAssignmentRepository.Select().Where(w => w.QTITestClassAssignmentId == assignmentID);
                    var studentIds = studentClassAssignmentOutsite != null && studentClassAssignmentOutsite.Any() ? studentClassAssignmentOutsite.Select(s => s.StudentId).ToList() : new List<int>();
                    var studentIdsInsert = students.Where(w => !studentIds.Contains(w)).ToList();
                    //insert db
                    var objs = studentIdsInsert.Select(s => new QTITestStudentAssignmentData()
                    {
                        StudentId = s,
                        QTITestClassAssignmentId = assignmentID,
                        Status = assignment.Status,
                        IsHide = isHide
                    }).ToList();
                    _qTITestStudentAssignmentService.AssignStudents(objs);
                }
                //assignment.Status = status;
                if (isHide)
                {
                    assignment.IsHide = GetIsHideForClassAssignment(assignmentID, isHide, assignment.ClassId, assignment.Type);
                }
                else
                {
                    assignment.IsHide = false;
                }
            }
            else
            {
                studentClassAssignments = _testStudentAssignmentRepository.Select().Where(w => w.QTITestClassAssignmentId == assignmentID).ToList();
                if (studentClassAssignments != null && studentClassAssignments.Count() > 0)
                {
                    foreach (var studentClassAssignment in studentClassAssignments)
                    {
                        studentClassAssignment.IsHide = isHide;
                        _testStudentAssignmentRepository.Save(studentClassAssignment);
                    }
                }
                assignment.IsHide = isHide;
            }
            assignment.ModifiedBy = "Portal";
            assignment.ModifiedDate = DateTime.UtcNow;
            assignment.ModifiedUserId = userID;
            _testClassAssignmentRepository.Save(assignment);
        }

        public void ChangeStatusShowHideMutipleAssignment(List<int> assignmentIDs, bool isHide, int userID, bool IsOldUI = false)
        {
            var assignments =
                _testClassAssignmentRepository.Select().Where(o => assignmentIDs.Contains(o.QTITestClassAssignmentId)).ToList();
            if (assignments == null || assignments.Count == 0) return;
            foreach (var assignment in assignments)
            {
                var studentClassAssignments = new List<QTITestStudentAssignmentData>();
                if (!IsOldUI)
                {
                    studentClassAssignments = _testStudentAssignmentRepository.Select().Where(w => w.QTITestClassAssignmentId == assignment.QTITestClassAssignmentId).ToList();
                    if (studentClassAssignments != null && studentClassAssignments.Count() > 0)
                    {
                        foreach (var studentClassAssignment in studentClassAssignments)
                        {
                            studentClassAssignment.IsHide = isHide;
                            _testStudentAssignmentRepository.Save(studentClassAssignment);
                        }
                    }
                    if (assignment.Type == (int)AssignmentType.Roster)
                    {
                        var studentIdOld = studentClassAssignments.Select(s => s.StudentId).Distinct().ToList();
                        if (studentIdOld != null && studentIdOld.Any())
                        {

                            var studentClassAssignmentOutsite = _testStudentAssignmentRepository.Select().Where(w => w.QTITestClassAssignmentId == assignment.QTITestClassAssignmentId);
                            var studentIds = studentClassAssignmentOutsite != null && studentClassAssignmentOutsite.Any() ? studentClassAssignmentOutsite.Select(s => s.StudentId).ToList() : new List<int>();
                            var studentIdsInsert = studentIds.Where(w => !studentIdOld.Contains(w)).ToList();
                            if (studentIdsInsert != null && studentIdsInsert.Any())
                            {
                                //insert db
                                var objs = studentIdsInsert.Select(s => new QTITestStudentAssignmentData()
                                {
                                    StudentId = s,
                                    QTITestClassAssignmentId = assignment.QTITestClassAssignmentId,
                                    Status = assignment.Status,
                                    IsHide = isHide
                                }).ToList();
                                _qTITestStudentAssignmentService.AssignStudents(objs);
                            }
                        }
                    }
                }
                else
                {
                    studentClassAssignments = _testStudentAssignmentRepository.Select().Where(w => w.QTITestClassAssignmentId == assignment.QTITestClassAssignmentId).ToList();
                    if (studentClassAssignments != null && studentClassAssignments.Count() > 0)
                    {
                        foreach (var studentClassAssignment in studentClassAssignments)
                        {
                            studentClassAssignment.IsHide = isHide;
                            _testStudentAssignmentRepository.Save(studentClassAssignment);
                        }
                    }
                }
            }
            assignments.ForEach(x =>
            {
                x.IsHide = isHide;
                x.ModifiedBy = "Portal";
                x.ModifiedDate = DateTime.UtcNow;
                x.ModifiedUserId = userID;
            });

            _iQTITestClassAssignmentRepository.SaveMutipleRecord(assignments);
        }

        public IQueryable<QTITestClassAssignment> GetTestClassAssignments_V1(string assignDate, bool onlyShowPendingReview, bool showActiveClassTestAssignment,
            int? userID, int? districtID, int? qtiTestClassAssignmentId, int schoolID)
        {
            //TODO: branch: TestCode_PortalHyperlink, add column IsTeacherLed from virtualtest table
            return _testClassAssignmentReadOnlyRepository.GetTestClassAssignments_V1(assignDate, onlyShowPendingReview, showActiveClassTestAssignment, userID, districtID, qtiTestClassAssignmentId, schoolID)
                .Select(x => new QTITestClassAssignment
                {
                    AssignmentDate = x.AssignmentDate,
                    ClassID = x.ClassID,
                    ClassName = x.ClassName,
                    TeacherName = string.IsNullOrEmpty(x.TeacherName) ? x.TeacherName : x.TeacherName.Replace(",", ""),
                    Code = x.Code,
                    CodeTime = x.CodeTime,
                    QTITestClassAssignmentID = x.QTITestClassAssignmentID,
                    Started = x.Started,
                    NotStarted = x.NotStarted,
                    WaitingForReview = x.WaitingForReview,
                    Completed = x.Completed,
                    TestName = x.TestName,
                    VirtualTestID = x.VirtualTestID,
                    DistrictID = x.DistrictID,
                    BankName = x.BankName,
                    GradeName = x.GradeName,
                    SubjectName = x.SubjectName,
                    Status = x.Status,
                    StudentNames = x.StudentNames,
                    AssignmentType = x.AssignmentType,
                    BankIsLocked = x.BankIsLocked,
                    IsTeacherLed = x.IsTeacherLed,
                    AssignmentModifiedUserID = x.AssignmentModifiedUserID,
                    AssignmentFirstName = x.AssignmentFirstName,
                    AssignmentLastName = x.AssignmentLastName,
                    IsHide = x.IsHide,
                    BankId = x.BankId
                }); ;
        }

        public GetTestClassAssignmentsResponse GetTestClassAssignmentsRemoveTempTable(GetTestClassAssignmentsRequest request)
        {
            return _testClassAssignmentReadOnlyRepository.GetTestClassAssignments(request);
        }

        public QTITestClassAssignmentData CheckUnPubLishForm(int virtualTestId, int classId)
        {
            return _testClassAssignmentRepository.Select().FirstOrDefault(o => o.VirtualTestId == virtualTestId && o.ClassId == classId && o.Status == (int)QTITestClassAssignmentStatusEnum.Publish && o.Type == (int)AssignmentType.DataLocker);
        }

        public IEnumerable<TestTakingPopupDetailDto> MonitoringTestGetPopupDetail(int qtiTestClassAssignmentId, IEnumerable<SessionIdAndActiveStatusDto> isolatingDatas)
        {
            /*
               public int QTIOnlineTestSessionId { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? Timestamp { get; set; }
        public int StatusId { get; set; }
            */
            string tempTableName = "#isolatingDatas";
            string createTableQuery = $"create table {tempTableName}(QTIOnlineTestSessionId int,LastLoginDate datetime,Timestamp datetime,StatusId int)";
            string finalizeProcedure = "dbo.MonitoringTestGetPopupDetail";
            using (DataSet popupDetails = _bulkHelper.BulkCopy(createTableQuery, tempTableName, isolatingDatas, finalizeProcedure, "@qtiTestClassAssignmentId", qtiTestClassAssignmentId))
            {
                if (popupDetails?.Tables?.Count > 0)
                {
                    return popupDetails.Tables[0].Rows.Cast<DataRow>()
                        .Select(c => new TestTakingPopupDetailDto()
                        {
                            StatusCode = c[nameof(TestTakingPopupDetailDto.StatusCode)].ToString(),
                            StudentNames = c[nameof(TestTakingPopupDetailDto.StudentNames)]?.ToString()
                            .Split(new char[] { Constanst.SYMBOL_BAR }, StringSplitOptions.RemoveEmptyEntries)
                            .Where(studentName => !string.IsNullOrEmpty(studentName)).ToArray() ?? new string[0]
                        });
                }
            }
            return new TestTakingPopupDetailDto[0];
        }

        public List<QTITestClassAssignment> GetTestClassAssignmentsExport(ExportTestAssignmentCriteria criteria, string assignDate, int userId, int districtId)
        {
            var data = new List<QTITestClassAssignment>();
            if (!string.IsNullOrEmpty(criteria.AssignmentCodes))
            {
                data =
                   _testClassAssignmentReadOnlyRepository.GetTestClassAssignmentsPassThroughExport(criteria.AssignmentCodes, criteria.OnlyShowPendingReview,
                    criteria.ShowActiveClassTestAssignment, userId, districtId).ToList();

                //filter search
                if (!string.IsNullOrEmpty(criteria.Grade))
                    data = data.Where(x => !string.IsNullOrEmpty(x.GradeName) && x.GradeName.ToLower().Contains(criteria.Grade.ToLower().Trim())).ToList();

                if (!string.IsNullOrEmpty(criteria.Subject))
                    data = data.Where(x => !string.IsNullOrEmpty(x.SubjectName) && x.SubjectName.ToLower().Contains(criteria.Subject.ToLower().Trim())).ToList();

                if (!string.IsNullOrEmpty(criteria.Bank))
                    data = data.Where(x => !string.IsNullOrEmpty(x.BankName) && x.BankName.ToLower().Contains(criteria.Bank.ToLower().Trim())).ToList();

                if (!string.IsNullOrEmpty(criteria.Teacher))
                    data = data.Where(x => !string.IsNullOrEmpty(x.TeacherName) && x.TeacherName.ToLower().Contains(criteria.Teacher.ToLower().Trim())).ToList();

                if (!string.IsNullOrEmpty(criteria.Class))
                    data = data.Where(x => !string.IsNullOrEmpty(x.ClassName) && x.ClassName.ToLower().Contains(criteria.Class.ToLower().Trim())).ToList();

                if (!string.IsNullOrEmpty(criteria.TestName))
                    data = data.Where(x => !string.IsNullOrEmpty(x.TestName) && x.TestName.ToLower().Contains(criteria.TestName.ToLower().Trim())).ToList();

                if (!string.IsNullOrEmpty(criteria.Student))
                    data = data.Where(x => !string.IsNullOrEmpty(x.StudentNames) && x.StudentNames.ToLower().Contains(criteria.Student.ToLower().Trim())).ToList();

                if (!string.IsNullOrEmpty(criteria.Code))
                    data = data.Where(x => !string.IsNullOrEmpty(x.Code) && x.Code.ToLower().Contains(criteria.Code.ToLower().Trim())).ToList();

                if (!string.IsNullOrEmpty(criteria.SearchBox))
                    data =
                        data.Where(
                            x =>
                                (!string.IsNullOrEmpty(x.GradeName) && x.GradeName.ToLower().Contains(criteria.SearchBox.ToLower().Trim()))
                                || (!string.IsNullOrEmpty(x.SubjectName) && x.SubjectName.ToLower().Contains(criteria.SearchBox.ToLower().Trim()))
                                 || (!string.IsNullOrEmpty(x.BankName) && x.BankName.ToLower().Contains(criteria.SearchBox.ToLower().Trim()))
                                 || (!string.IsNullOrEmpty(x.TeacherName) && x.TeacherName.ToLower().Contains(criteria.SearchBox.ToLower().Trim()))
                                 || (!string.IsNullOrEmpty(x.ClassName) && x.ClassName.ToLower().Contains(criteria.SearchBox.ToLower().Trim()))
                                 || (!string.IsNullOrEmpty(x.TestName) && x.TestName.ToLower().Contains(criteria.SearchBox.ToLower().Trim()))
                                 || (!string.IsNullOrEmpty(x.StudentNames) && x.StudentNames.ToLower().Contains(criteria.SearchBox.ToLower().Trim()))
                                 || (!string.IsNullOrEmpty(x.Code) && x.Code.ToLower().Contains(criteria.SearchBox.ToLower().Trim()))).ToList();

                data = data.OrderByDescending(x => x.AssignmentDate).ToList();
            }
            return data;
        }

        public IQueryable<QTITestClassAssignmentOTT> GetTestClassAssignmentsOTT(string assignDate, int? userID, int? districtID, bool showActiveClassTestAssignment, int schoolID)
        {
            return _testClassAssignmentReadOnlyRepository.GetTestClassAssignmentsOTT(assignDate, userID, districtID, showActiveClassTestAssignment, schoolID)
                .Select(x => new QTITestClassAssignmentOTT
                {
                    AssignmentDate = x.AssignmentDate,
                    ClassID = x.ClassID,
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherName,
                    Code = x.Code,
                    CodeTime = x.CodeTime,
                    QTITestClassAssignmentID = x.QTITestClassAssignmentID,
                    Started = x.Started,
                    NotStarted = x.NotStarted,
                    WaitingForReview = x.WaitingForReview,
                    Completed = x.Completed,
                    Autograding = x.Autograding,
                    Paused = x.Paused,
                    TestName = x.TestName,
                    VirtualTestID = x.VirtualTestID,
                    DistrictID = x.DistrictID,
                    BankName = x.BankName,
                    GradeName = x.GradeName,
                    SubjectName = x.SubjectName,
                    Status = x.Status,
                    StudentNames = x.StudentNames,
                    AssignmentType = x.AssignmentType,
                    SchoolName = x.SchoolName,
                }); ;
        }

        public IQueryable<QTITestClassAssignmentOTTRefresh> GetTestClassAssignmentsOTTRefresh(string qtiTestClassAssignmentIDs)
        {
            return _testClassAssignmentReadOnlyRepository.GetTestClassAssignmentsOTTRefresh(qtiTestClassAssignmentIDs);
        }

        public List<GetProctorTestViewDataResult> GetProctorTestViewData(int qtiTestClassAssignmentID)
        {
            return _testClassAssignmentReadOnlyRepository.GetProctorTestViewData(qtiTestClassAssignmentID);
        }

        public IQueryable<QTITestClassAssignment> GetTestClassAssignmentsPassThrough(string assignmentCodes, bool onlyShowPendingReview, bool showActiveClassTestAssignment, int? userID, int? districtID)
        {
            return _testClassAssignmentReadOnlyRepository.GetTestClassAssignmentsPassThrough(assignmentCodes, onlyShowPendingReview, showActiveClassTestAssignment, userID, districtID)
                .Select(x => new QTITestClassAssignment
                {
                    AssignmentDate = x.AssignmentDate,
                    ClassID = x.ClassID,
                    ClassName = x.ClassName,
                    TeacherName = string.IsNullOrEmpty(x.TeacherName) ? x.TeacherName : x.TeacherName.Replace(",", ""),
                    Code = x.Code,
                    CodeTime = x.CodeTime,
                    QTITestClassAssignmentID = x.QTITestClassAssignmentID,
                    Started = x.Started,
                    NotStarted = x.NotStarted,
                    WaitingForReview = x.WaitingForReview,
                    Completed = x.Completed,
                    TestName = x.TestName,
                    VirtualTestID = x.VirtualTestID,
                    DistrictID = x.DistrictID,
                    BankName = x.BankName,
                    GradeName = x.GradeName,
                    SubjectName = x.SubjectName,
                    Status = x.Status,
                    StudentNames = x.StudentNames,
                    AssignmentType = x.AssignmentType,
                    BankIsLocked = x.BankIsLocked,
                    IsTeacherLed = x.IsTeacherLed,
                    IsHide = x.IsHide,
                    BankId = x.BankId
                }); ;
        }

        public IQueryable<QTITestClassAssignmentData> GetTestAssignmentByClassId(int classId)
        {
            return _testClassAssignmentRepository.Select().Where(en => en.ClassId == classId);
        }

        public IQueryable<QTITestStudentAssignment> GetTestStudentAssignments(int? qtiTestClassAssignmentID, int? districtID)
        {
            return _testClassAssignmentReadOnlyRepository.GetTestStudentAssignments(qtiTestClassAssignmentID, districtID);
        }

        public bool IsTestAssignmentInNotStartedStatus(int? districtID, int testClassAssignmentID, int testStudentAssignmentID)
        {
            var testAssignment =
                _testClassAssignmentReadOnlyRepository.GetTestStudentAssignments(testClassAssignmentID, districtID)
                    .FirstOrDefault(o => o.QTITestStudentAssignmentID == testStudentAssignmentID);
            if (testAssignment == null) return true;

            return string.CompareOrdinal(NotStarted, testAssignment.AssignmentState) == 0;
        }

        public void DeassignStudent(int testClassAssignmentID, int testStudentAssignmentID)
        {
            var testClassAssignment =
                _testClassAssignmentRepository.Select().FirstOrDefault(o => o.QTITestClassAssignmentId == testClassAssignmentID);
            var testStudentAssignment =
                _testStudentAssignmentRepository.Select().FirstOrDefault(o => o.QTITestStudentAssignmentId == testStudentAssignmentID);

            if (testClassAssignment == null || testStudentAssignment == null) return;

            _testStudentAssignmentRepository.Delete(testStudentAssignment);
        }

        public IQueryable<QTIVirtualTest> GetQTIVirtualTest(int virtualTestID)
        {
            return _testClassAssignmentReadOnlyRepository.GetQTIVirtualTest(virtualTestID);
        }

        public IQueryable<QTITestState> GetTestState(int qtiOnlineTestSessionID)
        {
            return _testClassAssignmentReadOnlyRepository.GetTestState(qtiOnlineTestSessionID);
        }

        public IQueryable<QTITestState> GetTestStateTOS(int qtiOnlineTestSessionID)
        {
            return _testClassAssignmentReadOnlyRepository.GetTestStateTOS(qtiOnlineTestSessionID);
        }
        public AnswerViewer GetAnswerOfStudent(int testResultID, int virtualQuestionID)
        {
            return _testClassAssignmentReadOnlyRepository.GetAnswerOfStudent(testResultID, virtualQuestionID);
        }
        public void UpdateAnswerPointsEarned(UpdateAnswerPointsEarnedDto dto)
        {
            var qtiOnlineTestSession =
                _qtiOnlineTestSessionRepository.Select()
                    .FirstOrDefault(o => o.QTIOnlineTestSessionId == dto.QTIOnlineTestSessionID);

            InsertAnswerAudit(new InsertAnswerAuditDTO
            {
                UserID = dto.UserID,
                AnswerID = dto.AnswerID,
                AnswerSubID = dto.AnswerSubID,
                QTIOnlineTestSession = qtiOnlineTestSession,
                NewPointsEarned = dto.PointsEarned
            });

            InsertAnswerSubAudit(new InsertAnswerSubAuditDTO
            {
                UserID = dto.UserID,
                AnswerID = dto.AnswerID,
                AnswerSubID = dto.AnswerSubID,
                QTIOnlineTestSession = qtiOnlineTestSession,
                NewPointsEarned = dto.PointsEarned
            });

            _testClassAssignmentReadOnlyRepository.UpdateAnswerPointsEarned(dto.QTIOnlineTestSessionID, dto.AnswerID, dto.AnswerSubID, dto.PointsEarned, dto.UserID);
        }

        public void InsertAnswerAudit(InsertAnswerAuditDTO dto)
        {
            if (dto.QTIOnlineTestSession == null || !dto.AnswerID.HasValue) return;

            if (dto.QTIOnlineTestSession.StatusId == (int)QTIOnlineTestSessionStatusEnum.Completed)
            {
                var answer = _answerRepository.Select().FirstOrDefault(o => o.AnswerID == dto.AnswerID.Value);
                if (answer == null) return;

                dto.PreviousPointsEarned = answer.PointsEarned;

                _answerAuditRepository.Save(Transform(dto));
            }
            else if (dto.QTIOnlineTestSession.StatusId == (int)QTIOnlineTestSessionStatusEnum.PendingReview)
            {
                var qtiOnlineTestSessionAnswer = _qtiOnlineTestSessionAnswerRepository.Select().FirstOrDefault(o => o.QtiOnlineTestSessionAnswerID == dto.AnswerID.Value);
                if (qtiOnlineTestSessionAnswer == null) return;

                dto.PreviousPointsEarned = qtiOnlineTestSessionAnswer.PointsEarned;
                dto.VirtualQuestionID = qtiOnlineTestSessionAnswer.VirtualQuestionID;
                dto.QTIOnlineTestSessionID = dto.QTIOnlineTestSession.QTIOnlineTestSessionId;

                _qtiOnlineTestSessionAnswerAuditRepository.Save(TransformToQTI(dto));
            }
        }

        public AnswerAuditData Transform(InsertAnswerAuditDTO dto)
        {
            if (dto.QTIOnlineTestSession == null || !dto.AnswerID.HasValue) return null;

            var result = new AnswerAuditData
            {
                AnswerID = dto.AnswerID.Value,
                UserID = dto.UserID,
                DateTimeStamp = DateTime.UtcNow,
                PreviousValue = dto.PreviousPointsEarned,
                NewValue = dto.NewPointsEarned
            };

            return result;
        }

        public QTIOnlineTestSessionAnswerAuditData TransformToQTI(InsertAnswerAuditDTO dto)
        {
            if (dto.QTIOnlineTestSession == null || !dto.AnswerID.HasValue) return null;

            var result = new QTIOnlineTestSessionAnswerAuditData
            {
                QTIOnlineTestSessionAnswerID = dto.AnswerID.Value,
                UserID = dto.UserID,
                DateTimeStamp = DateTime.UtcNow,
                PreviousValue = dto.PreviousPointsEarned,
                NewValue = dto.NewPointsEarned,
                QTIOnlineTestSessionID = dto.QTIOnlineTestSessionID,
                VirtualQuestionID = dto.VirtualQuestionID
            };

            return result;
        }

        public void InsertAnswerSubAudit(InsertAnswerSubAuditDTO dto)
        {
            if (dto.QTIOnlineTestSession == null || !dto.AnswerID.HasValue || !dto.AnswerSubID.HasValue) return;

            if (dto.QTIOnlineTestSession.StatusId == (int)QTIOnlineTestSessionStatusEnum.Completed)
            {
                var answerSub = _answerSubDataRepository.Select().FirstOrDefault(o => o.AnswerSubID == dto.AnswerSubID.Value);
                if (answerSub == null) return;

                dto.PreviousPointsEarned = answerSub.PointsEarned;

                _answerSubAuditRepository.Save(Transform(dto));
            }
            else if (dto.QTIOnlineTestSession.StatusId == (int)QTIOnlineTestSessionStatusEnum.PendingReview)
            {
                var qtiOnlineTestSessionAnswerSub = _qtiOnlineTestSessionAnswerSubDataRepository.Select().FirstOrDefault(o => o.QTIOnlineTestSessionAnswerSubID == dto.AnswerSubID);
                if (qtiOnlineTestSessionAnswerSub == null) return;

                dto.PreviousPointsEarned = qtiOnlineTestSessionAnswerSub.PointsEarned;
                dto.VirtualQuestionSubID = qtiOnlineTestSessionAnswerSub.VirtualQuestionSubID;
                dto.QTIOnlineTestSessionID = dto.QTIOnlineTestSession.QTIOnlineTestSessionId;

                _qtiOnlineTestSessionAnswerSubAuditRepository.Save(TransformToQTI(dto));
            }
        }

        public AnswerSubAuditData Transform(InsertAnswerSubAuditDTO dto)
        {
            if (dto.QTIOnlineTestSession == null || !dto.AnswerID.HasValue || !dto.AnswerSubID.HasValue) return null;

            var result = new AnswerSubAuditData
            {
                AnswerSubID = dto.AnswerSubID.Value,
                UserID = dto.UserID,
                DateTimeStamp = DateTime.UtcNow,
                PreviousValue = dto.PreviousPointsEarned,
                NewValue = dto.NewPointsEarned
            };

            return result;
        }

        public QTIOnlineTestSessionAnswerSubAuditData TransformToQTI(InsertAnswerSubAuditDTO dto)
        {
            if (dto.QTIOnlineTestSession == null || !dto.AnswerID.HasValue || !dto.AnswerSubID.HasValue) return null;

            var result = new QTIOnlineTestSessionAnswerSubAuditData
            {
                QTIOnlineTestSessionAnswerSubID = dto.AnswerSubID.Value,
                UserID = dto.UserID,
                DateTimeStamp = DateTime.UtcNow,
                PreviousValue = dto.PreviousPointsEarned,
                NewValue = dto.NewPointsEarned,
                QTIOnlineTestSessionID = dto.QTIOnlineTestSessionID,
                VirtualQuestionSubID = dto.VirtualQuestionSubID
            };

            return result;
        }

        public void UpdateAnswerText(int? answerID, int? answerSubID, bool? saved)
        {
            _testClassAssignmentReadOnlyRepository.UpdateAnswerText(answerID, answerSubID, saved);
        }

        public void AddAutoGradingQueue(AutoGradingQueueData item)
        {
            _autoGradingQueueDataRepository.Save(item);
        }

        public string GetPreferenceOptionValue(int? qtiTestClassAssignmentID, int userID, string optionName)
        {
            var preferences =
                _testClassAssignmentReadOnlyRepository.GetPreferencesWithDefaultForOnlineTest(qtiTestClassAssignmentID,
                    userID);
            if (preferences == null || preferences.Count == 0) return null;
            var value = GetPreferenceOptionValue(preferences, "testassignment", optionName);
            if (value != null) return value;

            var bankLocked = preferences.Select(o => o.BankLocked).FirstOrDefault();
            if (bankLocked == null || !bankLocked.Value)
            {
                value = GetPreferenceOptionValue(preferences, "user", optionName);
                if (value != null) return value;
            }

            value = GetPreferenceOptionValue(preferences, "district", optionName);
            if (value != null) return value;

            value = GetPreferenceOptionValue(preferences, "enterprise", optionName);
            return value;
        }

        private string GetPreferenceOptionValue(List<Preferences> preferences, string level, string optionName)
        {
            if (preferences == null || preferences.Count == 0) return null;
            var testAssignment = preferences.FirstOrDefault(o => o.Level == level);
            if (testAssignment != null)
            {
                var value = GetPreferenceOptionValue(testAssignment.Value, optionName);
                return value;
            }

            return null;
        }

        private string GetPreferenceOptionValue(string preferenceStr, string optionName)
        {
            if (string.IsNullOrWhiteSpace(preferenceStr)) return null;
            preferenceStr = preferenceStr.ToLower();
            var xml = new XmlDocument();
            xml.LoadXml(preferenceStr);
            var options = xml.SelectNodes("/testsettings/options");
            if (options == null) return null;

            var option = options[0];
            var value = option[optionName] != null
                ? option[optionName].InnerText
                : null;

            return value;
        }

        public Dictionary<int, PreferenceOptions> GetPreferencesForOnlineTest(IEnumerable<int> qtiTestClassAssignmentIds)
        {
            var preferenceData = _testClassAssignmentReadOnlyRepository.GetPreferencesForOnlineTest(qtiTestClassAssignmentIds);
            var preferenceOptions = preferenceData
                .Where(c => c.QtiTestClassAssignmentId.HasValue)
                .ToDictionary(c => c.QtiTestClassAssignmentId ?? 0,
                c => GeneratePreferenceOptionsFromPreferenceData(c.RESULT ?? ""));
            return preferenceOptions;
        }

        public PreferenceOptions GetPreferencesForOnlineTest(int? qtiTestClassAssignmentID)
        {
            var preferenceData = _testClassAssignmentReadOnlyRepository.GetPreferencesForOnlineTest(qtiTestClassAssignmentID);
            PreferenceOptions preferenceOptions = GeneratePreferenceOptionsFromPreferenceData(preferenceData);
            return preferenceOptions;
        }

        public bool CanActive(int qtiTestClassAssignmentID)
        {
            return _iQTITestClassAssignmentRepository.CanActiveForRetake(qtiTestClassAssignmentID);
        }

        private PreferenceOptions GeneratePreferenceOptionsFromPreferenceData(string preferenceData)
        {
            if (preferenceData == null) return null;

            preferenceData = preferenceData.ToLower();

            var xml = new XmlDocument();
            xml.LoadXml(preferenceData);
            var options = xml.SelectNodes("/testsettings/options");
            if (options == null) return null;

            var option = options[0];
            var preferenceOptions = new PreferenceOptions
            {
                TimeLimit = option["timelimit"] != null ? GetBool(option["timelimit"].InnerText) : null,
                Deadline = option["deadline"] != null ? GetDateTime(option["deadline"].InnerText) : null,
                Duration = option["duration"] != null ? GetInt(option["duration"].InnerText) : null,
                BranchingTest = option["branchingtest"] != null ? GetBool(option["branchingtest"].InnerText) : null,

                //multipleChoiceClickMethod  : 0-ClickButton, 1-ClickAnswer
                MultipleChoiceClickMethod = option["multiplechoiceclickmethod"] != null ? option["multiplechoiceclickmethod"].InnerText : "0",

                TestSchedule = option["testschedule"] != null ? GetBool(option["testschedule"].InnerText) : null,
                TestScheduleToDate = option["testscheduletodate"] != null ? GetDateTime(option["testscheduletodate"].InnerText) : null,
                TestScheduleFromDate = option["testschedulefromdate"] != null ? GetDateTime(option["testschedulefromdate"].InnerText) : null,
                TestScheduleTimezoneOffset = option["testscheduletimezoneoffset"] != null ? GetDouble(option["testscheduletimezoneoffset"].InnerText) : null,
                TestScheduleStartTime = option["testschedulestarttime"] != null ? GetTimeSpan(option["testschedulestarttime"].InnerText) : null,
                TestScheduleEndTime = option["testscheduleendtime"] != null ? GetTimeSpan(option["testscheduleendtime"].InnerText) : null,
                TestScheduleDayOfWeek = option["testscheduledayofweek"] != null ? option["testscheduledayofweek"].InnerText : null,
                QuestionNumberLabel = option["questiongrouplabelschema"] != null ? option["questiongrouplabelschema"].InnerText : "0",
                SectionAvailability = option["sectionavailability"]?.GetAttribute("on") == "1",
                OpenSections = new List<int>(),
                AnonymizedScoring = GetBool(option["anonymizedscoring"]?.InnerText) ?? false
            };
            if (preferenceOptions.SectionAvailability && option["sectionavailability"].HasChildNodes)
            {
                foreach (XmlNode xmlNode in option["sectionavailability"].ChildNodes)
                {
                    if (xmlNode.Name.Equals("sectionitem") && xmlNode.Attributes["open"] != null)
                    {
                        int.TryParse(xmlNode.Attributes["sectionid"].Value, out int sectionId);
                        int.TryParse(xmlNode.Attributes["open"].Value, out int isOpen);
                        if (isOpen == 1)
                            preferenceOptions.OpenSections.Add(sectionId);
                    }
                }
            }
            preferenceOptions.TestScheduleSeparateDateAndTime = preferenceOptions.TestScheduleStartTime != null;
            return preferenceOptions;
        }

        private TimeSpan? GetTimeSpan(string innerText)
        {
            if (TimeSpan.TryParse(innerText, out TimeSpan ts))
            {
                return ts;
            }
            return null;
        }

        public VirtualTestData GetVirtualTestByID(int virtualTestID)
        {
            var result = _testClassAssignmentReadOnlyRepository.GetVirtualTestByID(virtualTestID);
            return result;
        }

        public IQueryable<StudentAssignSameTest> CheckAssignSameTest(int? assignmentTypeID, string studentIDs, string classIDs, int virtualTestID,
            bool? isRoster, int? groupID)
        {
            var result = _testClassAssignmentReadOnlyRepository.CheckAssignSameTest(assignmentTypeID, studentIDs,
                classIDs, virtualTestID, isRoster, groupID);
            return result;
        }

        public List<VirtualTestForPrinting> GetVirtualTestForPrinting(int virtualTestID)
        {
            var result = _testClassAssignmentReadOnlyRepository.GetVirtualTestForPrinting(virtualTestID);
            return result;
        }

        public List<VirtualTestForPrinting> GetVirtualTestAnswerKeyForPrinting(int virtualTestID, int userID)
        {
            var result = _testClassAssignmentReadOnlyRepository.GetVirtualTestAnswerKeyForPrinting(virtualTestID, userID);
            return result;
        }

        public PrintTestOfStudentDTO GetPrintTestOfStudent(int virtualTestID, bool manuallyGradedOnly)
        {
            var result = _testClassAssignmentReadOnlyRepository.GetPrintTestOfStudent(virtualTestID, manuallyGradedOnly);
            result.VirtualTestForPrinting =
                result.VirtualTestForPrinting.Where(x => x.VirtualSectionMode == VirtualSectionModeConstant.Normal)
                    .ToList();
            return result;
        }

        public QTITestClassAssignmentData GetQtiTestClassAssignment(int qtiTestClassAssignmentID)
        {
            var result =
                _testClassAssignmentReadOnlyRepository.GetTestClassAssignmentDetails(qtiTestClassAssignmentID);
            var testAssignment = _testClassAssignmentRepository.Select().FirstOrDefault(x => x.QTITestClassAssignmentId == qtiTestClassAssignmentID);

            if(testAssignment != null)
            {
                result.DistrictID = testAssignment.DistrictID;
            }
            return result;
        }

        public QTITestClassAssignmentData GetQtiTestClassAssignmentByAssignmentGUID(string assignmentGUID)
        {
            if (string.IsNullOrWhiteSpace(assignmentGUID)) return null;
            var result =
                _testClassAssignmentRepository.Select()
                    .FirstOrDefault(o => o.AssignmentGuId == assignmentGUID);
            return result;
        }

        private DateTime? GetDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            DateTime result;
            if (DateTime.TryParse(value, out result))
            {
                if (value.Length <= 10)
                    return result;
                return result.ToUniversalTime();
            }

            return null;
        }

        private int? GetInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        private double? GetDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            double result;

            if (double.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }
        private bool? GetBool(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            value = value.Trim().ToLower();
            if (String.Compare("true", value, StringComparison.Ordinal) == 0 ||
                String.Compare("1", value, StringComparison.Ordinal) == 0)
                return true;
            if (String.Compare("false", value, StringComparison.Ordinal) == 0 ||
                String.Compare("0", value, StringComparison.Ordinal) == 0)
                return false;
            return null;
        }

        public string GetTestCodeByAssignmentGUID(string guid)
        {
            var testClassAssignment = _testClassAssignmentRepository.Select().FirstOrDefault(x => x.AssignmentGuId == guid);
            if (testClassAssignment != null)
                return testClassAssignment.Code;

            return null;
        }
        public string GetTestCodeByTestClassIds(List<int> testClassIds)
        {
            var testClassAssignments = _testClassAssignmentRepository.Select()
                .Where(x => testClassIds.Contains(x.QTITestClassAssignmentId)).Select(x => x.Code).ToList();
            if (testClassAssignments != null)
                return string.Join(";", testClassAssignments);

            return null;
        }


        public AnswerSubInfo GetAnswerSubInfoByAnswerSubID(int qtiOnlineTestSessionID, int answerID, int answerSubID)
        {
            var result = _testClassAssignmentReadOnlyRepository.GetAnswerSubInfoByAnswerSubID(qtiOnlineTestSessionID,
                answerID, answerSubID);
            return result;
        }

        public AnswerInfo GetAnswerInfoByAnswerID(int qtiOnlineTestSessionID, int answerID)
        {
            var result = _testClassAssignmentReadOnlyRepository.GetAnswerInfoByAnswerID(qtiOnlineTestSessionID,
                answerID);
            return result;
        }

        public void GradingShortcuts(GradingShortcutsRequest request, int userId)
        {
            if (request.StudentGradingShortcuts == null || request.StudentGradingShortcuts.Count == 0) return;
            var completedQTIOnlineTestSessions = request.StudentGradingShortcuts.Where(o => o.StatusID == 4);
            if (completedQTIOnlineTestSessions.Any())
            {
                var dto = Transform(request, userId);
                dto.QTIOnlineTestSessionIDs = Deduction(completedQTIOnlineTestSessions.ToList());
                _testClassAssignmentReadOnlyRepository.GradingShortcutsCompleted(dto);
            }

            var notCompletedQTIOnlineTestSessions = GetStudentGradingShortcutInNotCompledtedStatus(request.StudentGradingShortcuts);
            if (notCompletedQTIOnlineTestSessions.Any())
            {
                var dto = Transform(request, userId);
                dto.QTIOnlineTestSessionIDs = Deduction(notCompletedQTIOnlineTestSessions.ToList());
                _testClassAssignmentReadOnlyRepository.GradingShortcutsPendingReview(dto);
            }
        }

        public List<StudentGradingShortcuts> GetStudentGradingShortcutInNotCompledtedStatus(List<StudentGradingShortcuts> data)
        {
            if (data == null) return new List<StudentGradingShortcuts>();
            var result = new List<StudentGradingShortcuts>();
            foreach (var item in data)
            {
                if (!item.QTIOnlineTestSessionID.HasValue) continue;
                if (item.StatusID == 4) continue;
                var status = GetGradingProcessStatus(item.QTIOnlineTestSessionID.Value);
                if (status == GradingProcessStatusEnum.FailedAndNotWaitingRetry
                    || status == GradingProcessStatusEnum.FailedAndWaitingRetry)
                    continue;

                result.Add(item);
            }

            return result;
        }

        public GradingShortcutsDTO Transform(GradingShortcutsRequest request, int userId)
        {
            if (request == null) return null;

            var dto = new GradingShortcutsDTO
            {
                QTITestClassAssignmentID = request.QTITestClassAssignmentID,
                AnswerID = request.AnswerID,
                AnswerSubID = request.AnswerSubID,
                AssignPointsEarned = request.AssignPointsEarned,
                GradeType = request.GradeType,
                UnAnswered = request.UnAnswered,
                Answered = request.Answered,
                //QTIOnlineTestSessionIDs = Deduction(request.StudentGradingShortcuts)
                UserId = userId
            };

            return dto;
        }

        public static string Deduction(List<StudentGradingShortcuts> studentGradingShortcuts)
        {
            if (studentGradingShortcuts == null) return null;
            var result = string.Join(",", studentGradingShortcuts.Select(o => o.QTIOnlineTestSessionID));

            return result;
        }

        public GradingProcessStatusEnum GetGradingProcessStatus(int qtiOnlineTestSessionID)
        {
            var qtiOnlineTestSession = _qtiOnlineTestSessionRepository.Select().FirstOrDefault(o => o.QTIOnlineTestSessionId == qtiOnlineTestSessionID);
            var autoGradingQueue = _autoGradingQueueDataRepository.GetAutoGradingQueueByQTOnlineTestSessionID(qtiOnlineTestSessionID);
            var result = GetGradingProcessStatusService.GetGradingProcessStatus(qtiOnlineTestSession, autoGradingQueue);

            return result;
        }
        public AutoGradingQueueData GetGradingQueue(int qtiOnlineTestSessionID)
        {
            var autoGradingQueue = _autoGradingQueueDataRepository.GetAutoGradingQueueByQTOnlineTestSessionID(qtiOnlineTestSessionID);
            return autoGradingQueue;
        }
        public NextApplicableStudent GetNextApplicableStudent(int qTITestClassAssignmentId, int virtualQuestionId, bool isManuallyGradedOnly, int studentID, bool ignoreCheckOverrideAutoGraded, string pendingStudentIDs = "")
        {
            bool isOverrideAutoGraded = true;
            if (!ignoreCheckOverrideAutoGraded)
            {
                var reference = _preferencesService.GetPreferenceByAssignmentLeveAndID(qTITestClassAssignmentId);
                if (reference != null)
                {
                    var obj = new ETLXmlSerialization<TestSettingsMapDTO>();
                    var referenceMap = obj.DeserializeXmlToObject(reference.Value);
                    if (referenceMap != null && referenceMap.TestSettingViewModel != null)
                    {
                        if (referenceMap.TestSettingViewModel.OverrideAutoGradedTextEntry == "0") isOverrideAutoGraded = false;
                    }
                }
            }
            // L25-3016: #2
            if (isOverrideAutoGraded && !isManuallyGradedOnly && !string.IsNullOrEmpty(pendingStudentIDs))
            {
                var studentIDs = pendingStudentIDs.Split(',').Select(e => int.Parse(e)).ToList();
                var assignment = _testClassAssignmentReadOnlyRepository.GetTestClassAssignmentDetails(qTITestClassAssignmentId);
                if (assignment == null)
                {
                    return new NextApplicableStudent()
                    {
                        IsLastStudent = false,
                        StudentID = 0,
                        VirtualQuestionID = 0
                    };
                }
                var sessionDictionary = _qtiOnlineTestSessionRepository.Select()
                    .Where(e => e.AssignmentGUId == assignment.AssignmentGuId && studentIDs.Contains(e.StudentId))
                    .ToDictionary(e => e.QTIOnlineTestSessionId);
                var sesstionAnswers = _qtiOnlineTestSessionAnswerRepository.Select()
                    .Where(e => sessionDictionary.Keys.Contains(e.QtiOnlineTestSessionID))
                    .ToList()
                    .Where(e => e.Overridden != true)
                    .OrderBy(e => e.QuestionOrder)
                    .ThenBy(e => studentIDs.IndexOf(sessionDictionary[e.QtiOnlineTestSessionID].StudentId));
                var sameQuestion = sesstionAnswers.Where(e => e.VirtualQuestionID == virtualQuestionId);
                var result = (sameQuestion.Count() > 0 ? sameQuestion : sesstionAnswers)
                    .FirstOrDefault();
                if (result != null)
                {
                    return new NextApplicableStudent
                    {
                        IsLastStudent = false,
                        StudentID = sessionDictionary[result.QtiOnlineTestSessionID].StudentId,
                        VirtualQuestionID = result.VirtualQuestionID
                    };
                } else
                {
                    return new NextApplicableStudent()
                    {
                        IsLastStudent = false,
                        StudentID = 0,
                        VirtualQuestionID = 0
                    };
                }
            }
            return _testClassAssignmentReadOnlyRepository.GetNextApplicableStudent(qTITestClassAssignmentId,
                virtualQuestionId, isManuallyGradedOnly, studentID, isOverrideAutoGraded);
        }

        public bool IsExistAutoGradingQueueBeingGraded(List<int?> qtiOnlineTestSessionIDs)
        {
            var autogradingQueue =
                _autoGradingQueueDataRepository.Select()
                    .Where(
                        x =>
                            qtiOnlineTestSessionIDs.Contains(x.QTIOnlineTestSessionID) &&
                            (x.Status == 0 || (x.Status == -1 && x.IsAwaitingRetry)));
            if (autogradingQueue.Any())
                return true;

            return false;
        }


        public NextApplicableQuestion GetNextApplicableQuestion(int qTITestClassAssignmentId, int studentId, bool isManuallyGradedOnly, bool ignoreOverrideAutoGraded)
        {
            bool isOverrideAutoGraded = true;
            if (!ignoreOverrideAutoGraded)
            {
                var reference = _preferencesService.GetPreferenceByAssignmentLeveAndID(qTITestClassAssignmentId);
                if (reference != null)
                {
                    var obj = new ETLXmlSerialization<TestSettingsMapDTO>();
                    var referenceMap = obj.DeserializeXmlToObject(reference.Value);
                    if (referenceMap != null && referenceMap.TestSettingViewModel != null)
                    {
                        if (referenceMap.TestSettingViewModel.OverrideAutoGradedTextEntry == "0") isOverrideAutoGraded = false;
                    }
                }
            }
            return _testClassAssignmentReadOnlyRepository.GetNextApplicableQuestion(qTITestClassAssignmentId, studentId, isManuallyGradedOnly, isOverrideAutoGraded);
        }
        /// <summary>
        /// Update answer text to QTIOnlineTestSessionAnswer table on main DB with QTIOnlineSession in Pending Review status
        /// </summary>
        /// <param name="answerText"></param>
        /// <param name="answerID"></param>
        /// <param name="answerSubID"></param>
        public void UpdateAnswerTextInPendingReviewStatus(string answerText, int answerID, int? answerSubID)
        {
            var qtiOnlineTestSessionAnswer = _qtiOnlineTestSessionAnswerRepository.Select().FirstOrDefault(o => o.QtiOnlineTestSessionAnswerID == answerID);
            if (qtiOnlineTestSessionAnswer == null) return;

            if (answerSubID.HasValue == false)
            {
                qtiOnlineTestSessionAnswer.AnswerText = answerText;
                _qtiOnlineTestSessionAnswerRepository.Save(qtiOnlineTestSessionAnswer);
            }
            else
            {
                var subAnswer = _qtiOnlineTestSessionAnswerSubDataRepository.Select().FirstOrDefault(x => x.QTIOnlineTestSessionAnswerSubID == answerSubID.Value);
                if (subAnswer != null)
                {
                    subAnswer.AnswerText = answerText;
                    _qtiOnlineTestSessionAnswerSubDataRepository.Save(subAnswer);
                }
            }

        }

        /// <summary>
        /// Support recover answer text from post answer logs with QTIOnlineSesion in completed mode
        /// </summary>
        /// <param name="answerText"></param>
        /// <param name="answerID"></param>
        /// <param name="answerSubID"></param>
        public void UpdateAnswerTextInCompletedStatus(string answerText, int answerID, int? answerSubID)
        {
            var answer = _answerRepository.Select().FirstOrDefault(x => x.AnswerID == answerID);
            if (answer == null) return;

            if (answerSubID.HasValue == false)
            {
                answer.AnswerText = answerText;
                _answerRepository.Save(answer);
            }
            else
            {
                var answerSub = _answerSubDataRepository.Select().FirstOrDefault(x => x.AnswerSubID == answerSubID);
                if (answerSub != null)
                {
                    answerSub.AnswerText = answerText;
                    _answerSubDataRepository.Save(answerSub);
                }
            }
        }

        public IQueryable<QTITestClassAssignmentForStudent> GetTestClassAssignmentsForStudent(int studentId, int districtID)
        {
            return _testClassAssignmentReadOnlyRepository.GetTestClassAssignmentsForStudent(studentId, districtID);
        }

        /// <summary>
        /// Get Status each student by QTITestClassAssignment
        /// </summary>
        /// <param name="qtiTestClassAssignmentId"></param>
        /// <returns></returns>
        public List<StudentTestStatusCustom> GetStudentTestStatus(int qtiTestClassAssignmentId)
        {
            try
            {
                var results = new List<StudentTestStatusCustom>();
                var lstObj = _testClassAssignmentReadOnlyRepository.GetStudentTestStatusByClassAssignmentId(qtiTestClassAssignmentId);
                if (lstObj != null)
                {
                    string strJoin = "";
                    lstObj.Where(o => o.Started > 0).ToList().ForEach(o =>
                    {
                        strJoin = string.Format("{0}|{1}", strJoin, string.Format("{0}&{1}", o.Status, o.StudentFullName));
                    });
                    var objStarted = new StudentTestStatusCustom()
                    {
                        StudentStatus = "Started",
                        Total = lstObj.Count(o => o.Started > 0),
                        ListStudents = strJoin.Length > 0 ? strJoin.Substring(1) : strJoin
                    };
                    results.Add(objStarted);

                    strJoin = "";
                    lstObj.Where(o => o.Completed > 0).ToList().ForEach(o =>
                    {
                        strJoin = string.Format("{0}|{1}", strJoin, string.Format("{0}&{1}", o.Status, o.StudentFullName));
                    });
                    var objCompleted = new StudentTestStatusCustom()
                    {
                        StudentStatus = "Completed",
                        Total = lstObj.Count(o => o.Completed > 0),
                        ListStudents = strJoin.Length > 0 ? strJoin.Substring(1) : strJoin
                    };
                    results.Add(objCompleted);

                    strJoin = "";
                    lstObj.Where(o => o.WaitingForReview > 0).ToList().ForEach(o =>
                    {
                        strJoin = string.Format("{0}|{1}", strJoin, string.Format("{0}&{1}", o.Status, o.StudentFullName));
                    });
                    var objWaitingForReview = new StudentTestStatusCustom()
                    {
                        StudentStatus = "WaitingForReview",
                        Total = lstObj.Count(o => o.WaitingForReview > 0),
                        ListStudents = strJoin.Length > 0 ? strJoin.Substring(1) : strJoin
                    };
                    results.Add(objWaitingForReview);

                    strJoin = "";
                    lstObj.Where(o => o.NotStarted > 0).ToList().ForEach(o =>
                    {
                        strJoin = string.Format("{0}|{1}", strJoin, string.Format("{0}&{1}", o.Status, o.StudentFullName));
                    });
                    var objNotStarted = new StudentTestStatusCustom()
                    {
                        StudentStatus = "NotStarted",
                        Total = lstObj.Count(o => o.NotStarted > 0),
                        ListStudents = strJoin.Length > 0 ? strJoin.Substring(1) : strJoin
                    };
                    results.Add(objNotStarted);

                    return results;
                }
            }
            catch (Exception ex)
            {
                return new List<StudentTestStatusCustom>();
            }
            return null;
        }
        public string GetAllStudents(int qtiTestClassAssignmentId)
        {
            try
            {
                var lstObj = _testClassAssignmentReadOnlyRepository.GetStudentTestStatusByClassAssignmentId(qtiTestClassAssignmentId);
                if (lstObj.Any())
                {
                    string strJoin = "";
                    lstObj.ForEach(o =>
                    {
                        strJoin = string.Format("{0}|{1}", strJoin, string.Format("{0}&{1}", o.Status, o.StudentFullName));
                    });
                    return strJoin.Substring(1);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public List<StudentTestStatus> GetListStudentTestStatus(int qtiTestClassAssignmentId)
        {
            try
            {
                return _testClassAssignmentReadOnlyRepository.GetListStudentTestStatus(qtiTestClassAssignmentId);
            }
            catch (Exception ex)
            {
                return new List<StudentTestStatus>();
            }
        }
        public List<AlgorithmicQuestionExpression> GetAlgorithmicQuestionExpressions(int virtualTestID)
        {
            return _testClassAssignmentReadOnlyRepository.GetAlgorithmicQuestionExpressions(virtualTestID);
        }

        /// <summary>
        /// Get active test class assignment by virtualTestId and ClassId
        /// </summary>
        /// <param name="testId"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        public QTITestClassAssignmentData GetLatestActiveAssignmentByVirtualTestIAndClass(int testId, int classId)
        {
            return _testClassAssignmentRepository.Select().Where(x => x.VirtualTestId == testId && x.ClassId == classId && x.Status == 1)
                .OrderByDescending(x => x.AssignmentDate).FirstOrDefault();
        }

        public void DeactivatePreviousAssignments(int testId, int classId, int userId)
        {
            var assignments = _testClassAssignmentRepository.Select().Where(x => x.VirtualTestId == testId && x.ClassId == classId & x.Status == 1)
                .Select(x => x.QTITestClassAssignmentId).ToList();

            foreach (int assignmentId in assignments)
            {
                var assignment = _testClassAssignmentRepository.Select().FirstOrDefault(x => x.QTITestClassAssignmentId == assignmentId);
                if (assignment != null)
                {
                    assignment.Status = 0;
                    assignment.ModifiedBy = "Portal";
                    assignment.ModifiedDate = DateTime.UtcNow;
                    assignment.ModifiedUserId = userId;
                }
                _testClassAssignmentRepository.Save(assignment);
            }
        }

        public GetTestClassAssignmentsOTTResponse GetTestClassAssignmentsOTT(GetTestClassAssignmentsOTTRequest request)
        {
            return _testClassAssignmentReadOnlyRepository.GetTestClassAssignmentsOTT(request);
        }

        public QTITestClassAssignmentData GetSurveyRosterAssignment(int districtId, int classId, int virtualtestId)
        {
            var result = _testClassAssignmentRepository.Select()
                .FirstOrDefault(o => o.DistrictID == districtId
                && o.ClassId == classId
                && o.VirtualTestId == virtualtestId
                && o.Type == 3);
            //Type on QTITestClassAssignment:
            //--1 TestClassSignment
            //--2 Passthrought
            //--3 Roster at Time of Test Taking
            //--4 TeacherPreview
            return result;
        }

        public QTITestClassAssignmentData GetSurveyRosterByCode(string testCode, int virtualTestId, int districtId)
        {
            //Type on QTITestClassAssignment:1(TestClassSignment), 2(Passthrought), 3(Roster at Time of Test Taking), 4(TeacherPreview)
            return _testClassAssignmentRepository.Select().FirstOrDefault(x => x.Code == testCode && x.Type == 3 && x.VirtualTestId == virtualTestId && x.DistrictID == districtId);
        }
        public QTITestClassAssignmentData GetSurveyAssignmentByCode(string testCode, int virtualTestId, int districtId)
        {
            return _testClassAssignmentRepository.Select().FirstOrDefault(x => x.Code == testCode && x.VirtualTestId == virtualTestId && x.DistrictID == districtId);
        }

        public int GetTotalSpentTimeByQTIOnlineTestSessionID(int qtiOnlineTestSessionId)
        {
            return _qTIOnlineTestSessionAnswerTimeTrackDynamoRepository.GetTotalSpentTimeByQTIOnlineTestSessionID(qtiOnlineTestSessionId);
        }

        public StudentAssginmentGroupDto GetStudentAssginment(int virtualTestId, List<int> selectedStudentIds, string classIds, string search = "", string sortColumns = "AssignmentDate", int pageSize = 0)
        {
            var result = new StudentAssginmentGroupDto();
            var sSearch = !string.IsNullOrEmpty(search) ? search.Trim() : search;
            var studentAssignment = _multipleTestResultRepository.GetStudentAssignment(virtualTestId, selectedStudentIds, classIds, sSearch, sortColumns, pageSize).ToList();

            result.OnlineTest = studentAssignment.Where(x => x.Type == Constanst.ASSIGNMENT_TYPE_ONLINE).OrderBy(x => x.FirstName).ToList();
            result.BubbleSheet = studentAssignment.Where(x => x.Type == Constanst.ASSIGNMENT_TYPE_BUBBLESHEET && x.StudentId > 0).ToList();

            // handel bubblesheet with roster generation
            var bulkGenerate = studentAssignment.Where(x => x.StudentId == 0 & !string.IsNullOrEmpty(x.StudentIds)).ToList();

            List<StudentAssignmentDto> students;
            var studentIds = (from studentId in bulkGenerate.SelectMany(c => c.StudentIds.ToIntArray(" "))
                              join selectedStudentId in selectedStudentIds
                              on studentId equals selectedStudentId
                              select studentId).Distinct().ToArray();
            Expression<Func<Student, StudentAssignmentDto>> selector = x => new StudentAssignmentDto
            {
                StudentId = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Code = x.Code,
            };
            students = _studentRepository.Select()
                .FilterOnLargeSet(selector, (subSet) =>
                {
                    return x => subSet.Contains(x.Id);
                }, studentIds)
                .ToList();

            result.BubbleSheet.AddRange(students);
            result.BubbleSheet = result.BubbleSheet.OrderBy(x => x.FirstName).ToList();

            return result;
        }

        public StudentAssginmentGroupDto GetStudentAssginmentGrouping(int virtualTestId, List<int> studentId, int groupId = 0, string classIds = "", string search = "", string sortColumns = "AssignmentDate", int pageSize = 0)
        {
            var groupStudents = _groupPrintingService.GetGroupStudents(groupId).Select(x => new { x.StudentId, x.ClassId }).Distinct();
            var result = new StudentAssginmentGroupDto();

            if (groupId > 0)
            {
                var studentAssignments = GetStudentAssginment(virtualTestId, groupStudents.Select(o => o.StudentId).ToList(), string.Join(",", groupStudents.Select(o => o.ClassId).ToList()), search, sortColumns, pageSize);
                result.BubbleSheet.AddRange(studentAssignments.BubbleSheet.Distinct());
                result.OnlineTest.AddRange(studentAssignments.OnlineTest.Distinct());
            }
            else
            {
                var studentAssignments = GetStudentAssginment(virtualTestId, studentId, classIds, search, sortColumns, pageSize);
                result.OnlineTest = studentAssignments.OnlineTest;
                result.BubbleSheet = studentAssignments.BubbleSheet;
            }

            return result;
        }

        public IQueryable<SurveyAssignmentResultsResponse> GetSurveyAssignmentResult(int districtId, int districtTermId, int surveyId, int surveyAssignmentType, string sortBy, string searchText, int skip = 0, int take = 10, bool isShowInactive = true)
        {
            return _testClassAssignmentReadOnlyRepository.GetSurveyAssignmentResultsByType(districtId, districtTermId, surveyId, surveyAssignmentType, sortBy, searchText, skip, take, isShowInactive);
        }

        public void BatchSaveAssignmentPublicIndividualized(SurveyAssignParameter param)
        {
            _testClassAssignmentReadOnlyRepository.BatchSavePublicAssignSurvey(param);
        }
        public List<AssignResult> BatchSaveAssignmentPrivate(SurveyAssignParameter param)
        {
            return _testClassAssignmentReadOnlyRepository.BatchSavePrivateAssignSurvey(param);
        }
        public bool CheckAllowAssignSurvey(int districtId, int distristTermId, int surveyId, int surveyAssignmentType)
        {
            return _testClassAssignmentReadOnlyRepository.CheckAllowAssignSurvey(districtId, distristTermId, surveyId, surveyAssignmentType);
        }
        public List<QTITestClassAssignmentForSurveyDto> GetForSurvey(int userId, int roleId, int studentId)
        {
            var assignments = _testClassAssignmentReadOnlyRepository.GetForSurvey(userId, studentId, roleId);
            CheckTestAssignmentsForSurvey(ref assignments);
            return assignments;
        }
        public List<AssignResult> GetSurveyPrivateAssignmentOfStudentAndParent(string assignmentIds)
        {
            return _testClassAssignmentReadOnlyRepository.GetSurveyPrivateAssignmentOfStudentAndParent(assignmentIds);
        }
        private void CheckTestAssignmentsForSurvey(ref List<QTITestClassAssignmentForSurveyDto> qTITestClassAssignmentForStudent)
        {
            var testSessionIds = qTITestClassAssignmentForStudent
                .Select(c => c.QTIOnlineTestSessionId)
                .Where(c => c.HasValue).Select(c => c.Value)
                .Distinct()
                .ToArray();

            var assignmentHasNoAutoGradingQueue = GetAllTestSessionIdHasNoAutoGradingQueueForSurvey(testSessionIds);

            var testClassIdOfAssignmentThatHasNoSessionId = qTITestClassAssignmentForStudent
                .Where(c => !c.QTIOnlineTestSessionId.HasValue)
                .Select(c => c.QTITestClassAssignmentId)
                .ToArray();

            var qTITestClassAssignmentIdHasNoAutoGrading = (from assignment in qTITestClassAssignmentForStudent
                                                            join id in assignmentHasNoAutoGradingQueue
                                                            on assignment.QTIOnlineTestSessionId equals id
                                                            select assignment.QTITestClassAssignmentId)
                                                            .ToArray()
                                                            .Concat(testClassIdOfAssignmentThatHasNoSessionId)
                                                            .Distinct()
                                                            .ToArray();

            var preferences = GetPreferencesForOnlineTest(qTITestClassAssignmentIdHasNoAutoGrading);

            var preferencesWithAssignments = (from assignment in qTITestClassAssignmentForStudent
                                              join preference in preferences
                                              on assignment.QTITestClassAssignmentId equals preference.Key
                                              select new
                                              {
                                                  assignment,
                                                  preference = preference.Value
                                              }).ToArray();

            var nowInSchoolTimeZone = new Dictionary<int, DateTime>();
            foreach (var preferenceWithAssignment in preferencesWithAssignments)
            {
                var preference = preferenceWithAssignment.preference;
                var assignment = preferenceWithAssignment.assignment;

                var classDetail = _classService.GetClassById(assignment.ClassId);
                DateTime schoolNowDateTime = DateTime.UtcNow;

                if (classDetail != null)
                {
                    var existing = nowInSchoolTimeZone.TryGetValue(classDetail.SchoolId.GetValueOrDefault(), out schoolNowDateTime);

                    if (!existing)
                    {
                        schoolNowDateTime = _schoolService.GetCurrentDateTimeBySchoolId(classDetail.SchoolId.GetValueOrDefault());
                        nowInSchoolTimeZone.Add(classDetail.SchoolId.GetValueOrDefault(), schoolNowDateTime);
                    }
                }

                string errorMessage = GetErrorString(assignment, preference, schoolNowDateTime);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    assignment.IsValid = false;
                    assignment.ErrorMsg = errorMessage;
                }
            }
        }

        private IEnumerable<int> GetAllTestSessionIdHasNoAutoGradingQueueForSurvey(IEnumerable<int> qTIOnlineTestSessionID)
        {
            var autoGradingQueues = _qtiOnlineTestSessionService.GetAutoGradingQueueByQTOnlineTestSessionID(qTIOnlineTestSessionID);


            var sessionIdsThatHasNoAutoGradingQueue = (from sessionId in qTIOnlineTestSessionID
                                                       join queue in autoGradingQueues
                                                       on sessionId equals queue.QTIOnlineTestSessionID
                                                       into joined
                                                       from j in joined.DefaultIfEmpty()
                                                       where j == null
                                                       select sessionId)
                                .ToArray();

            return sessionIdsThatHasNoAutoGradingQueue;
        }

        private string GetErrorString(QTITestClassAssignmentForSurveyDto assignment, PreferenceOptions preference, DateTime schoolNowDateTime)
        {
            if (preference != null)
            {
                double timeRemain = 0;
                //Check Time Limit
                if (preference.TimeLimit.HasValue && preference.TimeLimit.Value)
                {
                    string errorTimeLimit = "Test Period for this session has expired.";
                    if (preference.Deadline.HasValue && preference.Deadline > DateTime.MinValue)
                    {
                        timeRemain = preference.Deadline.Value.Subtract(schoolNowDateTime).TotalSeconds;
                        if (timeRemain < 0)
                        {
                            return errorTimeLimit;
                        }
                    }
                    else if (preference.Duration.HasValue && preference.Duration > 0)
                    {
                        if (assignment.QTIOnlineTestSessionId.HasValue)
                        {
                            int totalSpentTime = GetTotalSpentTimeByQTIOnlineTestSessionID(assignment.QTIOnlineTestSessionId.Value);
                            if (totalSpentTime > 0)
                            {
                                timeRemain = (preference.Duration.Value * 60) - totalSpentTime;
                                if (timeRemain < 0)
                                {
                                    return errorTimeLimit;
                                }
                            }
                        }
                    }
                }

                //Check Time TestSchedule
                if (!ValidTestSchedule(preference, schoolNowDateTime))
                {
                    return "This test is currently inactive. Please contact your teacher to activate the test.";
                }
            }
            return string.Empty;
        }

        private bool ValidTestSchedule(PreferenceOptions preference, DateTime schoolNowDateTime)
        {
            if (preference.TestSchedule != null
                && preference.TestScheduleFromDate != null
                && preference.TestScheduleToDate != null
                && preference.TestScheduleDayOfWeek != null
                && preference.TestScheduleTimezoneOffset != null
                && preference.TestSchedule.Value)
            {
                DateTime strStartDate = preference.TestScheduleFromDate.GetValueOrDefault();
                DateTime strEndDate = preference.TestScheduleToDate.GetValueOrDefault();
                string strDateOfWeek = preference.TestScheduleDayOfWeek;

                DateTime startDate = DateTime.Parse($"{strStartDate.ToShortDateString()} {preference.TestScheduleStartTime}");
                DateTime endDate = DateTime.Parse($"{strEndDate.ToShortDateString()} {preference.TestScheduleEndTime}");

                string[] strDateAllow = strDateOfWeek.Split('|');
                int currentIndex = (int)schoolNowDateTime.DayOfWeek;
                string[] dateOfWeek = { "sun", "mon", "tue", "wed", "thu", "fri", "sat" };

                if (schoolNowDateTime < startDate
                            || schoolNowDateTime.TimeOfDay < startDate.TimeOfDay
                            || schoolNowDateTime > endDate
                            || schoolNowDateTime.TimeOfDay > endDate.TimeOfDay
                            || !strDateAllow.Contains(dateOfWeek[currentIndex]))
                {
                    return false;
                }
            }
            return true;
        }

        public CheckMatchEmailDto CheckMatchEmail(string email, int districtId, int virtualTestId, int termId, int assignmentType)
        {
            return _iQTITestClassAssignmentRepository.CheckMatchEmail(email, districtId, virtualTestId, termId, assignmentType);
        }
        public bool IsAllStudentTestSessionCompleted(int qtiTestClassAssignmentId)
        {
            return _testClassAssignmentReadOnlyRepository.IsAllStudentTestSessionCompleted(qtiTestClassAssignmentId);
        }
        public bool IsTestHasManuallyQuestion(int qtiTestClassAssignmentId)
        {
            return _testClassAssignmentReadOnlyRepository.IsTestHasManuallyQuestion(qtiTestClassAssignmentId);
        }

        public IEnumerable<AnswerAttachmentDto> GetAnswerAttachments(int qtiOnlineTestSessionId)
        {
            return _testClassAssignmentReadOnlyRepository.GetAnswerAttachments(qtiOnlineTestSessionId);
        }

        public AttachmentDto ViewAttachment(Guid documentGuid, int qtiOnlineTestSessionId, int userId, RoleEnum role)
        {
            var canAccess = UserCanViewAttachment(documentGuid, qtiOnlineTestSessionId, userId, role);
            if (!canAccess)
            {
                throw new ForbiddenException("You don't have permission to access this attachment");
            }

            return _answerAttachmentService.ViewAttachment(documentGuid);
        }

        private bool UserCanViewAttachment(Guid documentGuid, int qtiOnlineTestSessionId, int userId, RoleEnum role)
        {
            return _answerAttachmentService.CheckUserCanAssessArtifact(userId, role, documentGuid) && ExistAttachmentOnQTIOnlineTestSession(documentGuid, qtiOnlineTestSessionId);
        }

        private bool ExistAttachmentOnQTIOnlineTestSession(Guid documentGuid, int qtiOnlineTestSessionId)
        {
            var answerAttachments = _testClassAssignmentReadOnlyRepository.GetAnswerAttachments(qtiOnlineTestSessionId);
            return answerAttachments.Any(aa => aa.DocumentGuid == documentGuid);
        }

        public SelectTestCustomDto GetGradeSubjectBankTestForRetake(string guid)
        {
            return _testClassAssignmentReadOnlyRepository.GetGradeSubjectBankTestForRetake(guid);
        }
        public List<StudentRetakeCustomDto> GetStudentStatusForTestRetake(int virtualTestId, string guid, string originalTestName)
        {
            var studentStatusTestRetake = _testClassAssignmentReadOnlyRepository.GetStudentStatusForTestRetake(virtualTestId, guid);
            if (studentStatusTestRetake != null)
            {
                foreach (var studentRetake in studentStatusTestRetake)
                {
                    string newTestName = studentRetake.VirtualTestName.Replace(originalTestName, string.Empty);
                    if (string.IsNullOrEmpty(newTestName))
                    {
                        studentRetake.VirtualTestDisplayName = Constanst.ORIGINAL_TEST;
                    }
                    else if (newTestName.Contains("PR"))
                    {
                        studentRetake.VirtualTestDisplayName = newTestName.Replace("PR", AssignmentRetakeConstants.PARTIAL_RETAKE);
                    }
                    else
                    {
                        studentRetake.VirtualTestDisplayName = newTestName.Replace("FR", AssignmentRetakeConstants.FULL_RETAKE);
                    }
                }

                return studentStatusTestRetake;
            }
            return null;
        }
        public List<RetakeListOfDisplayQuestionsDto> GetRetakeListOfDisplayQuestions(int virtualTestId, string studentIds, string guid, string testName)
        {
            return _testClassAssignmentReadOnlyRepository.GetRetakeListOfDisplayQuestions(virtualTestId, studentIds, guid, testName);
        }

        public LoadAssignmentRetakeResponse GetRetakeTestAssignResults(LoadAssignmentRetakeRequest request)
        {
            return _testClassAssignmentReadOnlyRepository.GetTestAssignResultForRetake(request);
        }

        public List<TestRetakeDataStudentInfo> GetRetakeDataStudentInfo(int virtualTestId , List<TestRetakeDataStudentInfo> students)
        {
            var resultStudentInfo = new List<TestRetakeDataStudentInfo>();
            var studentIds = students.Select(x => x.StudentId);
            var classIds = students.Select(x => x.ClassId);
            var qtiTestClassAssignment = _testClassAssignmentRepository.Select().Where(x => x.VirtualTestId == virtualTestId && x.Status == (int)QTITestClassAssignmentStatusEnum.Active && classIds.Contains(x.ClassId));
            var studentIdsTestResult = _testResultRepository.Select().Where(x => x.VirtualTestId == virtualTestId && studentIds.Contains(x.StudentId) && classIds.Contains(x.ClassId))?.Select(x => x.StudentId).ToList();
            if (qtiTestClassAssignment != null || studentIdsTestResult != null)
            {
                resultStudentInfo.AddRange(students);
                var qtiTestClassAssignmentIds = qtiTestClassAssignment.Select(x => x.QTITestClassAssignmentId).ToList();
                var qtiTestStudentIdsAssignment = _testStudentAssignmentRepository.Select().Where(x => qtiTestClassAssignmentIds.Contains(x.QTITestClassAssignmentId))?.Select(x=>x.StudentId).ToList();              
                if(qtiTestStudentIdsAssignment != null)
                {
                    var newStudentIds = studentIdsTestResult.Union(qtiTestStudentIdsAssignment).ToList();
                    resultStudentInfo = resultStudentInfo.Where(x => !newStudentIds.Any(y => y == x.StudentId)).ToList();
                }
                else
                    resultStudentInfo = resultStudentInfo.Where(x => !studentIdsTestResult.Any(y => y == x.StudentId)).ToList();
            }
            return resultStudentInfo;
        }

        public (List<int> TestsAssignmentCanActiveIds, List<string> TestNameErrors) GetVirtualTestsCanActive(List<int> qtiTestClassAssignmentIDs)
        {
            var qtiTestClassAssignActive = new List<int>();
            var testNameNotActive = new List<string>();
            var qtiVirtualTest = _testClassAssignmentRepository.Select()
                .Where(x => qtiTestClassAssignmentIDs.Contains(x.QTITestClassAssignmentId) && x.Status != (int)QTITestClassAssignmentStatusEnum.Active)
                .OrderByDescending(x => x.AssignmentDate)
                .Select(x => new
                {
                    VirtualTestId = x.VirtualTestId,
                    QtiTestId = x.QTITestClassAssignmentId
                }).ToList();

            var virtualTestIds = qtiVirtualTest.Select(x => x.VirtualTestId).ToList();
            var retakeVirtualTest = _virtualTestRepository.Select()
                .Where(x => virtualTestIds.Contains(x.VirtualTestID) && x.DatasetOriginID == (int)DataSetOriginEnum.Item_Based_Score_Retake)
                .Select(x => new
                {
                    VirtualTestId = x.VirtualTestID,
                    Name = x.Name
                }).ToList();
            if(retakeVirtualTest != null && retakeVirtualTest.Any())
            {
                var retakeVirtualTestIds = retakeVirtualTest.Select(x => x.VirtualTestId);
                qtiVirtualTest = qtiVirtualTest.Where(x => retakeVirtualTestIds.Contains(x.VirtualTestId)).ToList();
                var qtiRetakeIds = qtiVirtualTest.Select(x => x.QtiTestId).ToList();
                qtiTestClassAssignActive.AddRange(qtiTestClassAssignmentIDs.Where(x => !qtiRetakeIds.Any(y => y == x)).ToList());

                var qtiTestRetakeAssignActive = new List<int>();
                foreach (var id in qtiRetakeIds)
                {
                    if(CanActive(id))
                    {
                        qtiTestRetakeAssignActive.Add(id);
                    }
                    else
                    {
                        var testId = qtiVirtualTest.FirstOrDefault(x => x.QtiTestId == id)?.VirtualTestId;
                        var testName = retakeVirtualTest.FirstOrDefault(x => x.VirtualTestId == testId).Name;
                        testNameNotActive.Add(testName);
                    }
                }

                var testRetakeAssignmentGroupByTestId = qtiVirtualTest
                    .Where(x => qtiTestRetakeAssignActive.Contains(x.QtiTestId))
                    .GroupBy(x => x.VirtualTestId).ToList();

                qtiTestClassAssignActive.AddRange(testRetakeAssignmentGroupByTestId.Where(x => x.Count() == 1).SelectMany(x => x.Select(y => y.QtiTestId)));

                foreach (var testRetakeAssignmentsGrouped in testRetakeAssignmentGroupByTestId.Where(x =>x.Count() > 1))
                {
                    bool hadActivedAssignment = false;

                    foreach (var assignment in testRetakeAssignmentsGrouped)
                    {
                        if (!hadActivedAssignment)
                        {
                            qtiTestClassAssignActive.Add(assignment.QtiTestId);
                            hadActivedAssignment = true;
                        }
                        else
                        {
                            var testId = qtiVirtualTest.FirstOrDefault(x => x.QtiTestId == assignment.QtiTestId)?.VirtualTestId;
                            var testName = retakeVirtualTest.FirstOrDefault(x => x.VirtualTestId == testId).Name;
                            testNameNotActive.Add(testName);
                        }
                    }
                }
            }
            else
            {
                qtiTestClassAssignActive.AddRange(qtiTestClassAssignmentIDs);
            }
            return (qtiTestClassAssignActive, testNameNotActive);
        }

        private int GetStatusForClassAssignment(int qtiTestClassAssignmentID, int status, int classId, int assignmentType)
        {
            var statusClass = 1;
            var allStudents = _qTITestStudentAssignmentService.GetByQTITestClassAssignmentId(qtiTestClassAssignmentID);
            var studentDeactvivate = allStudents.Where(w => w.Status == 0);
            if (allStudents != null && allStudents.Any())
            {
                switch (assignmentType)
                {
                    case (int)AssignmentType.Class: case (int)AssignmentType.Student:
                        if (studentDeactvivate != null && studentDeactvivate.Any() && studentDeactvivate.Count() == allStudents.Count())
                        {
                            statusClass = 0;
                        }
                        break;
                    case (int)AssignmentType.Roster:
                        var countAllStudentInClass = _parentConnectService.GetAllStudentInClass(classId);
                        var studentIdDeactivated = studentDeactvivate.Select(s=>s.StudentId).Distinct().ToList();
                        var studentAddAfterAssigns = countAllStudentInClass.Where(w => !studentIdDeactivated.Contains(w.StudentID)).ToList();
                        if (studentDeactvivate != null && studentDeactvivate.Any() && studentDeactvivate.Count() == allStudents.Count() && studentAddAfterAssigns.Count() == 0)
                        {
                            statusClass = 0;
                        }
                        break;
                    default:
                        statusClass = 1;
                        break;
                }
            }
            else
            {
                statusClass = status;
            }            
            return statusClass;
        }
        private bool GetIsHideForClassAssignment(int qtiTestClassAssignmentID, bool isHide, int classId, int assignmentType)
        {
            var isHideClass = false;
            var allStudents = _qTITestStudentAssignmentService.GetByQTITestClassAssignmentId(qtiTestClassAssignmentID);
            var studentHides = allStudents.Where(w => w.IsHide.HasValue && w.IsHide.Value == true);
            if (allStudents != null && allStudents.Any())
            {
                switch (assignmentType)
                {
                    case (int)AssignmentType.Class: case (int)AssignmentType.Student:
                        if (studentHides != null && studentHides.Any() && studentHides.Count() == allStudents.Count())
                        {
                            isHideClass = true;
                        }
                        break;
                    case (int)AssignmentType.Roster:
                        var countAllStudentInClass = _parentConnectService.GetAllStudentInClass(classId);
                        var studentIdIsHide = studentHides.Select(s => s.StudentId).Distinct().ToList();
                        var studentAddAfterAssigns = countAllStudentInClass.Where(w => !studentIdIsHide.Contains(w.StudentID)).ToList();
                        if (studentHides != null && studentHides.Any() && studentHides.Count() == allStudents.Count() && studentAddAfterAssigns.Count() == 0)
                        {
                            isHideClass = true;
                        }
                        break;
                    default:
                        isHideClass = false;
                        break;
                }
            }
            else
            {
                isHideClass = isHide;
            }
            return isHideClass;
        }

        private bool AddStudentAfterAssignClass(QTITestClassAssignmentData assignment)
        {
            var studentIdOlds = new List<int>();
            var studentAssignments = _testStudentAssignmentRepository.Select().Where(w => w.QTITestClassAssignmentId == assignment.QTITestClassAssignmentId);
            if (studentAssignments != null)
            {
                studentIdOlds = studentAssignments.Select(s => s.StudentId).Distinct().ToList();
            }
            var listStudentIdInserts = _parentConnectService.GetAllStudentInClass(assignment.ClassId).Where(w => !studentIdOlds.Contains(w.StudentID)).Select(s => s.StudentID).Distinct().ToList();
            if (listStudentIdInserts != null && listStudentIdInserts.Any())
            {
                var dataInsers = listStudentIdInserts.Select(s => new QTITestStudentAssignmentData()
                {
                    QTITestClassAssignmentId = assignment.QTITestClassAssignmentId,
                    StudentId = s,
                    IsHide = assignment.IsHide,
                    Status = assignment.Status
                }).ToList();
                return _qTITestStudentAssignmentService.AssignStudents(dataInsers);
            }            
            return true;
        }

        public string GenerateAuthenticationCode(TestAssignmentGenerateAuthenticationCode request)
        {
            var entity = _testClassAssignmentRepository.Select()
                .FirstOrDefault(o => o.QTITestClassAssignmentId == request.QtiTestClassAssignmentID && o.AuthenticationCode == request.AuthenticationCode);

            if(entity == null) return string.Empty;

            var authenticationCode = _authenticationCodeGenerator.GenerateAuthenticationCode();
            var authenticationCodeExpirationDate = _authenticationCodeGenerator.GetExpirationDate(entity.ClassId);
            entity.SetAuthenticationCode(authenticationCode, authenticationCodeExpirationDate);

            _testClassAssignmentRepository.Save(entity);
            return entity.AuthenticationCode;
        }

        public void GenerateAuthenticationCode(int qtiTestClassAssignmentId, bool isEnable)
        {
            var entity = _testClassAssignmentRepository.Select()
                 .FirstOrDefault(x => x.QTITestClassAssignmentId == qtiTestClassAssignmentId);

            if (entity == null) return;

            string authenticationCode = null;
            DateTime? authenticationCodeExpirationDate = null;

            if (isEnable)
            {
                authenticationCode = _authenticationCodeGenerator.GenerateAuthenticationCode();
                authenticationCodeExpirationDate = _authenticationCodeGenerator.GetExpirationDate(entity.ClassId);
            }

            entity.SetAuthenticationCode(authenticationCode, authenticationCodeExpirationDate);
            _testClassAssignmentRepository.Save(entity);
        }

        public void InsertMultipleRecord(List<QTITestClassAssignmentData> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            _iQTITestClassAssignmentRepository.InsertMultipleRecord(items);
        }
    }
}
