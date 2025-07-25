using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.GradingShorcuts;
using LinkIt.BubbleSheetPortal.Models.Monitoring;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using LinkIt.BubbleSheetPortal.Data.Repositories.Helper;
using LinkIt.BubbleSheetPortal.Models.DTOs.Survey;
using LinkIt.BubbleSheetPortal.Models.DTOs.TeacherReview;
using System.Data;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting;
using LinkIt.BubbleSheetPortal.Models.DTOs.Retake;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTITestClassAssignmentReadOnlyRepository : IQTITestClassAssignmentReadOnlyRepository
    {
        private readonly TestDataContext _context;
        private readonly IConnectionString _connectionString;

        public QTITestClassAssignmentReadOnlyRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = TestDataContext.Get(connectionString);
            _connectionString = conn;
        }

        public GetTestClassAssignmentsResponse GetTestClassAssignments(GetTestClassAssignmentsRequest request)
        {
            var result = new GetTestClassAssignmentsResponse();
            var data = _context.GetTestClassAssignmentsRemoveTempTable(request.AssignDate, request.OnlyShowPedingReview, request.ShowActiveClassTestAssignment
                , request.UserID, request.DistrictID, request.QtiTestClassAssignmentId, request.SchoolID, request.GradeName, request.SubjectName, request.BankName
                , request.ClassName, request.TeacherName, request.StudentName, request.TestName, request.AssignmentCodes, request.GeneralSearch, request.SortColumn
                , request.SortDirection, request.StartRow, request.PageSize).
                ToList();            
            result.Data = data.Select(o => Transform(o)).ToList();
            result.TotalRecord = data?.FirstOrDefault()?.TotalRecords ?? 0;

            return result;
        }

        public IQueryable<QTITestClassAssignment> GetTestClassAssignments_V1(string assignDate, bool onlyShowPedingReview, bool showActiveClassTestAssignment,
            int? userID, int? districtID, int? qtiTestClassAssignmentId, int schoolID)
        {
            //TODO: Update correct Name when deploy 67.
            return _context.GetTestClassAssignments(assignDate, onlyShowPedingReview, showActiveClassTestAssignment, userID, districtID, qtiTestClassAssignmentId, schoolID).
                ToList().AsQueryable()
                .Select(x => new QTITestClassAssignment
                {
                    AssignmentDate = x.AssignmentDate,
                    ClassID = x.ClassID.Value,
                    ClassName = x.ClassName,
                    TeacherName = string.IsNullOrEmpty(x.TeacherName) ? x.TeacherName : x.TeacherName.Replace(",", ""),
                    Code = x.Code,
                    CodeTime = x.CodeTime,
                    QTITestClassAssignmentID = x.QTITestClassAssignmentID.Value,
                    Started = x.Started,
                    NotStarted = x.NotStarted,
                    WaitingForReview = x.WaitingForReview,
                    Completed = x.Completed,
                    TestName = x.TestName,
                    VirtualTestID = x.VirtualTestID.Value,
                    DistrictID = x.DistrictID,
                    BankName = x.BankName,
                    GradeName = x.GradeName,
                    SubjectName = x.SubjectName,
                    Status = x.Status,
                    StudentNames = x.Students,
                    AssignmentType = x.AssignmentType,
                    BankIsLocked = x.BankIsLocked,
                    AssignmentModifiedUserID = x.AssignmentModifiedUserID,
                    IsTeacherLed = x.IsTeacherLed,
                    AssignmentFirstName = x.AssignmentFirstName,
                    AssignmentLastName = x.AssignmentLastName,
                    IsHide = x.IsHide ?? false
                });
        }

        private QTITestClassAssignment Transform(GetTestClassAssignmentsRemoveTempTableResult x)
        {
            if (x == null) return null;

            var result = new QTITestClassAssignment
            {
                AssignmentDate = x.AssignmentDate,
                ClassID = x.ClassID ?? 0,
                ClassName = x.ClassName,
                TeacherName = x.TeacherName,
                Code = x.Code,
                CodeTime = x.CodeTime,
                QTITestClassAssignmentID = x.QTITestClassAssignmentID ?? 0,
                Started = x.Started,
                NotStarted = x.NotStarted,
                WaitingForReview = x.WaitingForReview,
                Completed = x.Completed,
                TestName = x.TestName,
                VirtualTestID = x.VirtualTestID ?? 0,
                DistrictID = x.DistrictID,
                BankName = x.BankName,
                GradeName = x.GradeName,
                SubjectName = x.SubjectName,
                Status = x.Status,
                StudentNames = x.Students,
                AssignmentType = x.AssignmentType,
                BankIsLocked = x.BankIsLocked ?? 0,
                IsTeacherLed = x.IsTeacherLed,
                AssignmentModifiedUserID = x.AssignmentModifiedUserID,
                AssignmentFirstName = x.AssignmentFirstName,
                AssignmentLastName = x.AssignmentLastName,
                IsHide = x.IsHide ?? false,
                BankId = x.BankID.GetValueOrDefault(),
                IsVirtualTestRetake = x.IsVirtualTestRetake == 1,
                AuthenticationCode = x.AuthenticationCode,
                IsAuthenticationCodeExpired = x.AuthenticationCodeExpirationDate < DateTime.UtcNow,
            };

            return result;
        }

        public IQueryable<QTITestClassAssignment> GetTestClassAssignmentsExport(string assignDate, bool onlyShowPedingReview, bool showActiveClassTestAssignment,
           int? userID, int? districtID, int? schoolId)
        {
            //TODO: Update correct Name when deploy 67.
            return _context.GetTestClassAssignmentsExport(assignDate, onlyShowPedingReview, showActiveClassTestAssignment, userID, districtID, schoolId).
                ToList().AsQueryable()
                .Select(x => new QTITestClassAssignment
                {
                    AssignmentDate = x.AssignmentDate,
                    ClassID = x.ClassID.Value,
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherName,
                    Code = x.Code,
                    CodeTime = x.CodeTime,
                    QTITestClassAssignmentID = x.QTITestClassAssignmentID.Value,
                    Started = x.Started,
                    NotStarted = x.NotStarted,
                    WaitingForReview = x.WaitingForReview,
                    Completed = x.Completed,
                    TestName = x.TestName,
                    VirtualTestID = x.VirtualTestID.Value,
                    DistrictID = x.DistrictID,
                    BankName = x.BankName,
                    GradeName = x.GradeName,
                    SubjectName = x.SubjectName,
                    Status = x.Status,
                    StudentNames = x.Students,
                    AssignmentType = x.AssignmentType,
                    BankIsLocked = x.BankIsLocked,
                    AssignmentModifiedUserID = x.AssignmentModifiedUserID,
                    IsTeacherLed = x.IsTeacherLed,
                    AssignmentFirstName = x.AssignmentFirstName,
                    AssignmentLastName = x.AssignmentLastName,
                    SchoolName = x.SchoolName
                });
        }

        public IQueryable<QTITestClassAssignmentOTT> GetTestClassAssignmentsOTT(string assignDate, int? userID, int? districtID, bool showActiveClassTestAssignment, int schoolID)
        {
            return _context.GetTestClassAssignmentsOTT(assignDate, userID, districtID, showActiveClassTestAssignment, schoolID).
                ToList().AsQueryable()
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
                    StudentNames = x.Students,
                    AssignmentType = x.AssignmentType,
                    SchoolName = x.SchoolName
                });
        }

        public IQueryable<QTITestClassAssignmentOTTRefresh> GetTestClassAssignmentsOTTRefresh(string qtiTestClassAssignmentIDs)
        {
            return _context.GetTestClassAssignmentsOTTRefresh(qtiTestClassAssignmentIDs).
                ToList().AsQueryable()
                .Select(x => new QTITestClassAssignmentOTTRefresh
                {
                    QTITestClassAssignmentID = x.QTITestClassAssignmentID,
                    Started = x.Started,
                    NotStarted = x.NotStarted,
                    WaitingForReview = x.WaitingForReview,
                    Completed = x.Completed,
                    Assigned = x.Assigned,
                    Autograding = x.Autograding,
                    Paused = x.Paused,
                    NotActive = x.InActive
                });
        }

        public IQueryable<QTITestClassAssignment> GetTestClassAssignmentsPassThrough(string assignmentCodes, bool onlyShowPedingReview, bool showActiveClassTestAssignment,
            int? userID, int? districtID)
        {
            //TODO: Update correct Name when deploy 67.
            return _context.GetTestClassAssignmentsPassThrough(assignmentCodes, onlyShowPedingReview, showActiveClassTestAssignment, userID, districtID).
                ToList().AsQueryable()
                .Select(x => new QTITestClassAssignment
                {
                    AssignmentDate = x.AssignmentDate,
                    ClassID = x.ClassID.Value,
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherName,
                    Code = x.Code,
                    CodeTime = x.CodeTime,
                    QTITestClassAssignmentID = x.QTITestClassAssignmentID.Value,
                    Started = x.Started,
                    NotStarted = x.NotStarted,
                    WaitingForReview = x.WaitingForReview,
                    Completed = x.Completed,
                    TestName = x.TestName,
                    VirtualTestID = x.VirtualTestID.Value,
                    DistrictID = x.DistrictID,
                    BankName = x.BankName,
                    GradeName = x.GradeName,
                    SubjectName = x.SubjectName,
                    Status = x.Status,
                    StudentNames = x.Students,
                    AssignmentType = x.AssignmentType,
                    BankIsLocked = x.BankIsLocked,
                    IsTeacherLed = x.IsTeacherLed,
                    IsHide = x.IsHide ?? false
                });
        }

        public IQueryable<QTITestClassAssignment> GetTestClassAssignmentsPassThroughExport(string assignmentCodes, bool onlyShowPedingReview, bool showActiveClassTestAssignment,
            int? userID, int? districtID)
        {
            //TODO: Update correct Name when deploy 67.
            return _context.GetTestClassAssignmentsPassThroughExport(assignmentCodes, onlyShowPedingReview, showActiveClassTestAssignment, userID, districtID).
                 ToList().AsQueryable()
                 .Select(x => new QTITestClassAssignment
                 {
                     AssignmentDate = x.AssignmentDate,
                     ClassID = x.ClassID.Value,
                     ClassName = x.ClassName,
                     TeacherName = x.TeacherName,
                     Code = x.Code,
                     CodeTime = x.CodeTime,
                     QTITestClassAssignmentID = x.QTITestClassAssignmentID.Value,
                     Started = x.Started,
                     NotStarted = x.NotStarted,
                     WaitingForReview = x.WaitingForReview,
                     Completed = x.Completed,
                     TestName = x.TestName,
                     VirtualTestID = x.VirtualTestID.Value,
                     DistrictID = x.DistrictID,
                     BankName = x.BankName,
                     GradeName = x.GradeName,
                     SubjectName = x.SubjectName,
                     Status = x.Status,
                     StudentNames = x.Students,
                     StudentIds = x.StudentIds,
                     AssignmentType = x.AssignmentType,
                     BankIsLocked = x.BankIsLocked,
                     IsTeacherLed = x.IsTeacherLed,
                     AssignmentFirstName = x.AssignmentFirstName,
                     AssignmentLastName = x.AssignmentLastName,
                     SchoolName = x.SchoolName
                 });
        }

        public List<GetProctorTestViewDataResult> GetProctorTestViewData(int qtiTestClassAssignmentID)
        {
            return _context.GetProctorTestViewData(qtiTestClassAssignmentID).
                ToList();
        }

        public IQueryable<QTITestStudentAssignment> GetTestStudentAssignments(int? qtiTestClassAssignmentID, int? districtID)
        {
            return _context.GetTestStudentAssignments(qtiTestClassAssignmentID, districtID).ToList().AsQueryable()
                .Select(x => new QTITestStudentAssignment
                {
                    AssignmentDate = x.AssignmentDate,
                    Code = x.Code,
                    QTITestClassAssignmentID = x.QTITestClassAssignmentID,
                    TestName = x.TestName,
                    Status = x.Status,
                    AssignmentState = x.AssignmentState,
                    PassCode = x.PassCode,
                    QTITestStudentAssignmentID = x.QTITestStudentAssignmentID,
                    StudentName = x.StudentName,
                    StudentID = x.StudentID ?? 0,
                    VirtualTestID = x.VirtualTestID,
                    QTIOnlineTestSessionID = x.QTIOnlineTestSessionID,
                    QTIOnlineStatusID = x.QTIOnlineStatusID,
                    TimeOver = x.TimeOver,
                    StartDate = x.StartDate,
                    CanBulkGrading = x.CanBulkGrading
                });
        }

        public IQueryable<QTIVirtualTest> GetQTIVirtualTest(int virtualTestID)
        {
            return _context.GetQTIVirtualTest(virtualTestID).ToList().AsQueryable()
                .Select(x => new QTIVirtualTest
                {
                    QTIItemID = x.QTIItemID,
                    VirtualQuestionID = x.VirtualQuestionID,
                    QTIItemSchemaID = x.QTISchemaID,
                    QuestionOrder = x.QuestionOrder,
                    PointsPossible = x.PointsPossible,
                    XmlContent = x.XmlContent,
                    SectionInstruction = x.SectionInstruction,
                    SectionTitle = x.SectionTitle,
                    BaseVirtualQuestionID = x.BaseVirtualQuestionID,
                    CorrectAnswer = x.CorrectAnswer,
                    ResponseProcessing = x.ResponseProcessing,
                    VirtualSectionMode = x.VirtualSectionMode,
                    ResponseProcessingTypeID = x.ResponseProcessingTypeID,
                    IsRubricBasedQuestion = x.IsRubricBasedQuestion
                });
        }

        public IQueryable<QTITestState> GetTestState(int qtiOnlineTestSessionID)
        {
            return _context.GetTestState(qtiOnlineTestSessionID).ToList().AsQueryable()
                .Select(x => new QTITestState
                {
                    QTIOnlineTestSessionAnswerID = x.QTIOnlineTestSessionAnswerID,
                    AnswerID = x.AnswerID,
                    QTIOnlineTestSessionID = x.QTIOnlineTestSessionID,
                    VirtualQuestionID = x.VirtualQuestionID,
                    Answered = x.Answered,
                    AnswerChoice = x.AnswerChoice,
                    AnswerText = x.AnswerText,
                    AnswerImage = x.AnswerImage,
                    PointsEarned = x.PointsEarned,
                    AnswerSubs = x.AnswerSubs,
                    XmlContent = x.XmlContent,
                    ResponseProcessingTypeID = x.ResponseProcessingTypeID,
                    QTISchemaID = x.QTISchemaID,
                    HighlightQuestion = x.HighlightQuestion,
                    HighlightPassage = x.HighlightPassage,
                    ScoreRaw = x.ScoreRaw,
                    AnswerTemp = x.AnswerTemp,
                    Overridden = x.Overridden,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate,
                    ItemFeedbackID = x.ItemFeedbackID,
                    ItemAnswerID = x.ItemAnswerID,
                    Feedback = x.Feedback,
                    UserIdFeedback = x.UserIdFeedback,
                    UserUpdatedFeedback = x.UserUpdatedFeedback,
                    DateUpdatedFeedback = x.DateUpdatedFeedback,
                    AnswerOrder = x.AnswerOrder,
                    VirtualSectionMode = x.VirtualSectionMode,
                    TimeSpent = x.TimeSpent ?? 0,
                    TimesVisited = x.TimesVisited ?? 0,
                    DrawingContent = x.DrawingContent
                });
        }

        public IQueryable<QTITestState> GetTestStateTOS(int qtiOnlineTestSessionID)
        {
            return _context.GetTestStateTOS(qtiOnlineTestSessionID).ToList().AsQueryable()
                .Select(x => new QTITestState
                {
                    QTIOnlineTestSessionAnswerID = x.QTIOnlineTestSessionAnswerID,
                    QTIOnlineTestSessionID = x.QTIOnlineTestSessionID,
                    VirtualQuestionID = x.VirtualQuestionID,
                    Answered = x.Answered,
                    AnswerChoice = x.AnswerChoice,
                    AnswerText = x.AnswerText,
                    AnswerImage = x.AnswerImage,
                    PointsEarned = x.PointsEarned,
                    AnswerSubs = x.AnswerSubs,
                    ResponseProcessingTypeID = x.ResponseProcessingTypeID,
                    HighlightQuestion = x.HighlightQuestion,
                    HighlightPassage = x.HighlightPassage,
                    HighlightQuestionGroupCommon = x.HighlightQuestionGroupCommon,
                    ScoreRaw = x.ScoreRaw,
                    Feedback = x.Feedback,
                    AnswerOrder = x.AnswerOrder,
                });
        }

        public AnswerViewer GetAnswerOfStudent(int testResultID, int virtualQuestionID)
        {
            return _context.GetAnswerOfStudent(testResultID, virtualQuestionID).AsQueryable()
                .Select(x => new AnswerViewer
                {
                    QTIOnlineTestSessionAnswerID = x.QTIOnlineTestSessionAnswerID,
                    AnswerID = x.AnswerID,
                    QTIOnlineTestSessionID = x.QTIOnlineTestSessionID,
                    VirtualQuestionID = x.VirtualQuestionID,
                    Answered = x.Answered,
                    AnswerChoice = x.AnswerChoice,
                    AnswerText = x.AnswerText,
                    AnswerImage = x.AnswerImage,
                    PointsEarned = x.PointsEarned,
                    AnswerSubs = x.AnswerSubs,
                    QTISchemaID = x.QTISchemaID,
                    XMLContent = x.XMLContent,
                    CorrectAnswer = x.CorrectAnswer,
                    PointsPossible = x.PointsPossible
                }).FirstOrDefault();
        }

        public void UpdateAnswerText(int? answerID, int? answerSubID, bool? saved)
        {
            _context.UpdateAnswerText(answerID, answerSubID, saved);
        }

        public void UpdateAnswerPointsEarned(int qtiOnlineTestSessionID, int answerID, int? answerSubID, int pointsEarned, int userID)
        {
            _context.UpdateAnswerPointsEarned(qtiOnlineTestSessionID, answerID, answerSubID, pointsEarned, userID);
        }

        public IQueryable<QTITestClassAssignment> Select()
        {
            throw new NotImplementedException();
            //return GetTestClassAssignments(DateTime.MinValue.AddYears(1800).ToShortDateString(), false, false, null, null, null, 0);
        }

        public string GetPreferencesForOnlineTest(int? qtiTestClassAssignmentID)
        {
            var result = _context.GetPreferencesForOnlineTest(qtiTestClassAssignmentID, null).FirstOrDefault();
            if (result != null) return result.Column1;
            return string.Empty;
        }

        public IEnumerable<GetPreferencesForOnlineTest_MultipleResult> GetPreferencesForOnlineTest(IEnumerable<int> qtiTestClassAssignmentIds)
        {
            string idsConcatenated = string.Join(",", qtiTestClassAssignmentIds);
            var result = _context.GetPreferencesForOnlineTest_Multiple(idsConcatenated).ToArray();
            return result;
        }

        public List<Preferences> GetPreferencesWithDefaultForOnlineTest(int? qtiTestClassAssignmentID, int userID)
        {
            var result = _context.GetPreferencesWithDefaultForOnlineTest(qtiTestClassAssignmentID, null, userID);
            return result.Select(o => new Preferences
            {
                Level = o.Level,
                Id = o.ID,
                Value = o.Value,
                BankLocked = o.BankLocked
            }).ToList();
        }

        public VirtualTestData GetVirtualTestByID(int virtualTestID)
        {
            var test = _context.TestEntities.FirstOrDefault(o => o.VirtualTestID == virtualTestID);
            if (test == null) return null;

            var result = new VirtualTestData
            {
                VirtualTestID = test.VirtualTestID,
                BankID = test.BankID,
                Name = test.Name,
                StateID = test.StateID,
                TestScoreMethodID = test.TestScoreMethodID,
                VirtualTestSubTypeID = test.VirtualTestSubTypeID
            };

            return result;
        }

        public IQueryable<StudentAssignSameTest> CheckAssignSameTest(int? assignmentTypeID, string studentIDs, string classIDs, int virtualTestID, bool? isRoster, int? qtiGroupID)
        {
            return _context.CheckAssignSameTest1(assignmentTypeID, studentIDs, classIDs, virtualTestID, isRoster, qtiGroupID).ToList().AsQueryable()
                .Select(s => new StudentAssignSameTest
                {
                    StudentId = s.StudentID,
                    SchoolName = s.SchoolName,
                    ClassName = s.ClassName,
                    TeacherName = s.TeacherName,
                    FirstName = s.FirstName,
                    LastName = s.LastName
                });
        }

        public List<VirtualTestForPrinting> GetVirtualTestForPrinting(int virtualTestID)
        {
            // _context.GetVirtualTestForPrinting
            var result = _context.GetVirtualTestForPrintingV2(virtualTestID).ToList().Select(o => new VirtualTestForPrinting
            {
                PointsPossible = o.PointsPossible,
                QTIGroupID = o.QTIGroupID,
                QTIItemID = o.QTIItemID,
                QuestionOrder = o.QuestionOrder,
                SectionInstruction = o.SectionInstruction,
                SectionOrder = o.SectionOrder,
                SectionReferenceID = o.SectionReferenceID,
                SectionTitle = o.SectionTitle,
                TestInstruction = o.TestInstruction,
                TestName = o.TestName,
                Title = o.Title,
                VirtualQuestionID = o.VirtualQuestionID,
                VirtualSectionID = o.VirtualSectionID,
                XmlContent = o.XmlContent,
                UrlPath = o.UrlPath,
                QuestionGroupId = o.QuestionGroupID,
                GroupQuestionCommon = o.GroupQuestionCommon,
                GroupQuestionTitle = o.GroupQuestionTitle,
                QuestionLabel = o.QuestionLabel,
                IsNumberQuestions = o.IsNumberQuestions
            }).ToList();

            return result;
        }

        public List<VirtualTestForPrinting> GetVirtualTestAnswerKeyForPrinting(int virtualTestID, int userID)
        {
            var result = _context.GetVirtualTestAnswerKeyForPrinting(virtualTestID, userID).ToList().Select(o => new VirtualTestForPrinting
            {
                PointsPossible = o.PointsPossible,
                QTIGroupID = o.QTIGroupID,
                QTIItemID = o.QTIItemID,
                QTISchemaID = o.QTISchemaID,
                QuestionOrder = o.QuestionOrder,
                SectionInstruction = o.SectionInstruction,
                SectionOrder = o.SectionOrder,
                SectionReferenceID = o.SectionReferenceID,
                SectionTitle = o.SectionTitle,
                TestInstruction = o.TestInstruction,
                TestName = o.TestName,
                Title = o.Title,
                VirtualQuestionID = o.VirtualQuestionID,
                VirtualSectionID = o.VirtualSectionID,
                XmlContent = o.XmlContent,
                UrlPath = o.UrlPath,
                CorrectAnswer = o.CorrectAnswer,
                Answers = o.Answers,
                Skills = o.Skills,
                Topics = o.Topics,
                Other = o.Other,
                Standards = o.Standards,
                DistrictTag = o.DistrictTag,
                QuestionGroupId = o.QuestionGroupID.GetValueOrDefault(), //TODO:
                GroupQuestionCommon = string.Empty,
                //ResponseProcessingTypeID = o.ResponseProcessingTypeID ?? 0,
                GroupQuestionTitle = string.Empty,
                QuestionLabel = o.QuestionLabel
            }).ToList();

            return result;
        }

        public PrintTestOfStudentDTO GetPrintTestOfStudent(int virtualTestID, bool manuallyGradedOnly)
        {
            //_context.GetPrintTestOfStudent
            var virtualTestForPrinting = _context.GetPrintTestOfStudentV2(virtualTestID, manuallyGradedOnly).Select(o => new VirtualTestForPrinting
            {
                PointsPossible = o.PointsPossible,
                QTIGroupID = o.QTIGroupID,
                QTIItemID = o.QTIItemID,
                QuestionOrder = o.QuestionOrder,
                SectionInstruction = o.SectionInstruction,
                SectionOrder = o.SectionOrder,
                SectionReferenceID = o.SectionReferenceID,
                SectionTitle = o.SectionTitle,
                TestInstruction = o.TestInstruction,
                TestName = o.TestName,
                Title = o.Title,
                VirtualQuestionID = o.VirtualQuestionID,
                QTISchemaID = o.QTISchemaID,
                VirtualSectionID = o.VirtualSectionID,
                XmlContent = o.XmlContent,
                UrlPath = o.UrlPath,
                QTIItemAnswerScoresStr = o.QTIITemAnswerScoresStr,
                QTIItemAnswerScoresDTO = TransformToQTIItemAnswerScoreDTOs(o.QTIITemAnswerScoresStr),
                VirtualSectionMode = o.VirtualSectionMode,
                QuestionGroupId = o.QuestionGroupID,
                GroupQuestionCommon = o.GroupQuestionCommon,
                GroupQuestionTitle = o.GroupQuestionTitle,
                QuestionLabel = o.QuestionLabel
            }).ToList();

            var result = new PrintTestOfStudentDTO();
            result.VirtualTestForPrinting = virtualTestForPrinting;

            return result;
        }

        private List<QTIItemAnswerScoreDTO> TransformToQTIItemAnswerScoreDTOs(string xmlData)
        {
            if (string.IsNullOrWhiteSpace(xmlData)) return new List<QTIItemAnswerScoreDTO>();
            var xdoc = XDocument.Parse(xmlData);
            var result = new List<QTIItemAnswerScoreDTO>();
            foreach (var item in xdoc.Element("QTIITemAnswerScores").Elements("QTIITemAnswerScore"))
            {
                var dto = new QTIItemAnswerScoreDTO
                {
                    QTIItemAnswerScoreID = Common.XmlUtils.GetIntValue(item.Element("QTIItemAnswerScoreID")),
                    QTIItemID = Common.XmlUtils.GetIntValue(item.Element("QTIItemID")),
                    ResponseIdentifier = Common.XmlUtils.GetStringValue(item.Element("ResponseIdentifier")),
                    Answer = Common.XmlUtils.GetStringValue(item.Element("Answer")),
                    Score = Common.XmlUtils.GetIntValue(item.Element("Score")),
                    VirtualQuestionAnswerScore = Common.XmlUtils.GetInt(Common.XmlUtils.GetStringValue(item.Element("VirtualQuestionAnswerScore")))
                };

                result.Add(dto);
            }

            return result;
        }

        public void InactiveQtiTestClassAssignment(int districtId, int bankId)
        {
            _context.InactiveQtiTestClassAssignment(districtId, bankId);
        }

        public AnswerSubInfo GetAnswerSubInfoByAnswerSubID(int qtiOnlineTestSessionID, int answerID, int answerSubID)
        {
            var data = _context.fnGetAnswerSubInfoByAnswerSubID(qtiOnlineTestSessionID, answerID, answerSubID).FirstOrDefault();
            if (data == null) return null;

            var result = new AnswerSubInfo
            {
                QTISchemaID = data.QTISchemaID ?? 0,
                ResponseProcessingTypeID = data.ResponseProcessingTypeID ?? 0
            };

            return result;
        }

        public AnswerInfo GetAnswerInfoByAnswerID(int qtiOnlineTestSessionID, int answerID)
        {
            var data = _context.fnGetAnswerInfoByAnswerID(qtiOnlineTestSessionID, answerID).FirstOrDefault();
            if (data == null) return null;

            var result = new AnswerInfo
            {
                QTISchemaID = data.QTISchemaID ?? 0,
                ResponseProcessingTypeID = data.ResponseProcessingTypeID ?? 0
            };

            return result;
        }

        public void GradingShortcutsCompleted(GradingShortcutsDTO dto)
        {
            _context.GradingShortcutsCompleted(dto.QTITestClassAssignmentID, dto.AnswerID, dto.AnswerSubID, dto.AssignPointsEarned,
                dto.GradeType, dto.UnAnswered, dto.Answered, dto.QTIOnlineTestSessionIDs, dto.UserId);
        }

        public void GradingShortcutsPendingReview(GradingShortcutsDTO dto)
        {
            _context.GradingShortcutsPendingReview(dto.QTITestClassAssignmentID, dto.AnswerID, dto.AnswerSubID, dto.AssignPointsEarned,
                dto.GradeType, dto.UnAnswered, dto.Answered, dto.QTIOnlineTestSessionIDs, dto.UserId);
        }

        public NextApplicableStudent GetNextApplicableStudent(int qTITestClassAssignmentId, int virtualQuestionId, bool isManuallyGradedOnly, int studentID, bool isOverrideAutoGraded)
        {
            return _context.GetNextApplicableStudent(qTITestClassAssignmentId, virtualQuestionId, isManuallyGradedOnly, studentID, isOverrideAutoGraded).Select(x => new NextApplicableStudent()
            {
                StudentID = x.StudentID ?? 0,
                //IsLastStudent = x.IsLastStudent ?? false
                VirtualQuestionID = x.VirtualQuestionID ?? 0
            }).FirstOrDefault();
        }

        public QTITestClassAssignmentData GetTestClassAssignmentDetails(int qtiTestClassAssignmentID)
        {
            var data = _context.GetTestClassAssignmentDetails(qtiTestClassAssignmentID).FirstOrDefault();

            if (data == null) return null;

            var result = new QTITestClassAssignmentData
            {
                AssignmentDate = data.AssignmentDate,
                AssignmentGuId = data.AssignmentGUID,
                ClassId = data.ClassID,
                ClassName = data.ClassName,
                Code = data.Code,
                CodeTimestamp = data.CodeTimestamp,
                ComparisonPasscodeLength = data.ComparisonPasscodeLength,
                ModifiedBy = data.ModifiedBy,
                ModifiedDate = data.ModifiedDate,
                ModifiedUserId = data.ModifiedUserID,
                QTITestClassAssignmentId = data.QTITestClassAssignmentID,
                Status = data.Status,
                TeacherName = data.TeacherName,
                TestName = data.TestName,
                TestSetting = data.TestSetting,
                TutorialMode = data.Mode ?? 1,
                Type = data.Type,
                VirtualTestId = data.VirtualTestID
            };

            return result;
        }

        public List<StudentTestStatus> GetStudentTestStatusByClassAssignmentId(int qtiTestClassAssignmentId)
        {
            var lstObj = _context.GetTestClassAssignmentsById(qtiTestClassAssignmentId).ToList();
            if (lstObj != null && lstObj.Count > 0)
            {
                return lstObj.Select(o => new StudentTestStatus()
                {
                    Completed = o.Completed,
                    NotStarted = o.NotStarted,
                    Started = o.Started,
                    WaitingForReview = o.WaitingForReview,
                    StudentFullName = o.StudentFullName,
                    StudentId = o.StudentId,
                    Status = o.Status,
                }).ToList();
            }
            return new List<StudentTestStatus>();
        }


        public List<StudentTestStatus> GetListStudentTestStatus(int qtiTestClassAssignmentId)
        {
            var data = new List<StudentTestStatus>();
            var student = _context.StudentTREntities.AsQueryable();
            var qTITestStudentAssignment = _context.QTITestStudentAssignmentEntities.Where(w => w.QTITestClassAssignmentID == qtiTestClassAssignmentId).AsQueryable();
            data = (from st in student
                    join qtisa in qTITestStudentAssignment on st.StudentID equals qtisa.StudentId
                    select new StudentTestStatus()
                    {
                        IsHide = qtisa.IsHide,
                        Status = qtisa.Status,
                        StudentFullName = st.FullName,
                        StudentId = st.StudentID
                    }).ToList();
            if (data != null)
            {
                return data;
            }
            return new List<StudentTestStatus>();
        }

        public NextApplicableQuestion GetNextApplicableQuestion(int qTITestClassAssignmentId, int studentId, bool isManuallyGradedOnly, bool isOverrideAutoGraded)
        {
            var result = _context.GetNextApplicableQuestion(qTITestClassAssignmentId, studentId, isManuallyGradedOnly, isOverrideAutoGraded).Select(o => new NextApplicableQuestion()
            {
                VirtualQuestionID = o.VirtualQuestionID.GetValueOrDefault(),
                QTIOnlineTestSessionAnswerID = o.QTIOnlineTestSessionAnswerID.GetValueOrDefault(),
                StudentID = o.StudentID.GetValueOrDefault()
            }).FirstOrDefault();

            return result;
        }

        public List<AlgorithmicQuestionExpression> GetAlgorithmicQuestionExpressions(int virtualTestID)
        {
            return _context.GetAlgorithmicQuestionExpression(virtualTestID)
               .Select(x => new AlgorithmicQuestionExpression
               {
                   VirtualQuestionID = x.VirtualQuestionID,
                   VirtualTestID = x.VirtualTestID,
                   QTIItemID = x.QTIItemID,
                   AlgorithmicExpression = x.AlgorithmicExpression,
                   PointsEarned = x.PointsEarned,
                   AlgorithmicOrder = x.AlgorithmicOrder
               }).ToList();
        }

        public IQueryable<QTITestClassAssignmentForStudent> GetTestClassAssignmentsForStudent(int studentId, int districtID)
        {
            //TODO: Update correct Name when deploy 67.
            return _context.GetTestAssignmentsForStudent(studentId, districtID).
                ToList().AsQueryable()
                .Select(x => new QTITestClassAssignmentForStudent
                {
                    AssignmentDate = x.AssignmentDate,
                    ClassId = x.ClassID.GetValueOrDefault(),
                    ClassName = x.ClassName,
                    TeacherName = x.TeacherName,
                    Code = x.Code,
                    RawCode = x.RawCode,
                    QTITestClassAssignmentID = x.QTITestClassAssignmentID.Value,
                    TestName = x.TestName,
                    Status = x.Status,
                    AssignmentModifiedUserID = x.AssignmentModifiedUserID,
                    AssignmentFirstName = x.AssignmentFirstName,
                    AssignmentLastName = x.AssignmentLastName,
                    AssignmentGUID = x.AssignmentGUID,
                    StartDate = x.StartDate,
                    QTIOnlineTestSessionID = x.QTIOnlineTestSessionID,
                    DistrictTermDateStart = x.DistrictTermDateStart,
                    DistrictTermDateEnd = x.DistrictTermDateEnd,
                    IsTutorialMode = x.Mode == (int)AssignmentModeEnum.Tutorial
                });
        }

        public GetTestClassAssignmentsOTTResponse GetTestClassAssignmentsOTT(GetTestClassAssignmentsOTTRequest request)
        {
            var result = new GetTestClassAssignmentsOTTResponse();

            var data = _context.GetTestClassAssignmentsOTTImprove(request.AssignDate, request.UserID, request.DistrictID, request.ShowActiveClassTestAssignment, request.SchoolID,
                    request.GradeName, request.SubjectName, request.BankName, request.ClassName, request.TeacherName,
                    request.StudentName, request.TestName, request.AssignmentCodes, request.GeneralSearch, request.SortColumn, request.SortDirection,
                    request.StartRow, request.PageSize)
                    .ToList();

            result.TotalRecord = data?.FirstOrDefault()?.TotalRecord ?? 0;
            result.Data = data.Select(x => Transform(x)).ToList();
            return result;
        }

        public bool CheckPermissionsEditQTITestClassAssignment(int roleId, int userId, string qtiTestClassAssignmentIds)
        {
            var data = _context.CheckPermissionsEditQTITestClassAssignment(roleId, userId, qtiTestClassAssignmentIds);
            return data.FirstOrDefault().Result.Value;
        }

        private QTITestClassAssignmentOTT Transform(GetTestClassAssignmentsOTTImproveResult x)
        {
            if (x == null) return null;

            var result = new QTITestClassAssignmentOTT
            {
                AssignmentDate = x.AssignmentDate,
                ClassID = x.ClassID ?? 0,
                ClassName = x.ClassName,
                TeacherName = x.TeacherName,
                Code = x.Code,
                CodeTime = x.CodeTime,
                QTITestClassAssignmentID = x.QTITestClassAssignmentID ?? 0,
                Started = x.Started,
                NotStarted = x.NotStarted,
                WaitingForReview = x.WaitingForReview,
                Completed = x.Completed,
                TestName = x.TestName,
                VirtualTestID = x.VirtualTestID ?? 0,
                DistrictID = x.DistrictID,
                BankName = x.BankName,
                GradeName = x.GradeName,
                SubjectName = x.SubjectName,
                Status = x.Status,
                StudentNames = x.Students,
                AssignmentType = x.AssignmentType,
                Autograding = x.Autograding,
                Paused = x.Paused,
                SchoolName = x.SchoolName,
            };

            return result;
        }
        public bool IsAllStudentTestSessionCompleted(int qtiTestClassAssignmentId)
        {
            return _context.IsAllStudentSessionCompleted(qtiTestClassAssignmentId) ?? true;
        }
        public bool IsTestHasManuallyQuestion(int qtiTestClassAssignmentId)
        {
            return _context.IsTestHasManuallyQuestion(qtiTestClassAssignmentId) ?? false;
        }
        public IQueryable<SurveyAssignmentResultsResponse> GetSurveyAssignmentResultsByType(int districtId, int districtTermId, int surveyId, int surveyAssignmentType, string sortBy, string searchText, int skip = 0, int take = 10, bool isShowInactive = true)
        {
            var result = _context.GetSurveyAssignmentResultsByType(districtId, districtTermId, surveyId, surveyAssignmentType, sortBy, searchText, skip, take, isShowInactive)
                .AsQueryable().Select(x => new SurveyAssignmentResultsResponse()
                {
                    AssignmentDate = x.AssignmentDate,
                    Id = x.Id,
                    Code = x.Code,
                    ShortLink = x.ShortLink,
                    Status = x.Status,
                    Email = x.Email,
                    ResponseDate = x.ResponseDate,
                    Respondent = x.Respondent,
                    StudentId = x.StudentId,
                    TotalRecords = x.TotalRecords
                });
            return result;
        }
        public void BatchSavePublicAssignSurvey(SurveyAssignParameter param)
        {
            BulkHelper bulkHelper = new BulkHelper(_connectionString);
            string tempTableName = "#SurveyCodeAndFullLinkAndKeyTemp";
            string tempTableCreateScript = $@"CREATE TABLE [{tempTableName}](Code varchar(20), ShortLinkKey varchar(20), FullLink varchar(1000), Email varchar(200), UserId int)";
            string batchSaveAssignSurveyProcedureName = "BatchSavePublicAssignSurvey";

            bulkHelper.BulkCopy(tempTableCreateScript, tempTableName, param.AssignCodeFullLinks, batchSaveAssignSurveyProcedureName,
                "@VirtualTestId", param.SurveyId, "@DistrictId", param.DistrictId, "@UserId", param.UserId, "@ClassId", param.ClassId, "@PreferenceValue", param.Preferences);
        }
        public List<AssignResult> BatchSavePrivateAssignSurvey(SurveyAssignParameter param)
        {
            BulkHelper bulkHelper = new BulkHelper(_connectionString);
            string tempTableName = "#SurveyCodeAndFullLinkAndKeyPrivateTemp";
            string tempTableCreateScript = $@"CREATE TABLE [{tempTableName}](Code varchar(20), ShortLinkKey varchar(20), FullLink varchar(1000), Email varchar(200), UserId int)";
            string batchSaveAssignSurveyProcedureName = "BatchSavePrivateAssignSurvey";

            using (DataSet data = bulkHelper.BulkCopy(tempTableCreateScript, tempTableName, param.AssignCodeFullLinks, batchSaveAssignSurveyProcedureName,
                "@VirtualTestId", param.SurveyId, "@DistrictId", param.DistrictId, "@UserId", param.UserId, "@ClassId", param.ClassId, "@SurveyAssignmentType", param.SurveyAssignmentType, "@PreferenceValue", param.Preferences))
            {
                if (data?.Tables?.Count > 0)
                {
                    return data.Tables[0].Rows.Cast<DataRow>()
                        .Select(c => new AssignResult()
                        {
                            Email = c[nameof(AssignResult.Email)].ToString(),
                            RoleId = int.Parse(c[nameof(AssignResult.RoleId)]?.ToString())
                        }).ToList();
                }
            }

            return new List<AssignResult>();
        }

        public List<QTITestClassAssignmentForSurveyDto> GetForSurvey(int userId, int studentId, int roleId)
        {
            var assignements = _context.GetTestAssignmentsForSurvey(userId, roleId, studentId)
                                    .Select(o => new QTITestClassAssignmentForSurveyDto
                                    {
                                        AssignmentDate = o.AssignmentDate,
                                        TestName = o.TestName,
                                        Code = o.Code,
                                        Status = o.Status,
                                        AssignmentGUID = o.AssignmentGUID,
                                        QTITestClassAssignmentId = o.QTITestClassAssignmentID.Value,
                                        QTIOnlineTestSessionId = o.QTIOnlineTestSessionID,
                                        SurveyAssignmentType = o.SurveyAssignmentType.GetValueOrDefault(),
                                        StudentId = o.StudentID.GetValueOrDefault(),
                                        RedirectUrl = o.ShortLink
                                    }).ToList();

            return assignements;
        }
        public bool CheckAllowAssignSurvey(int districtId, int districtTermId, int surveyId, int surveyAssignmentType)
        {
            var results = _context.CheckAllowAssignSurvey(districtId, districtTermId, surveyId, surveyAssignmentType).FirstOrDefault();
            return results?.Status == 1;
        }
        public List<AssignResult> GetSurveyPrivateAssignmentOfStudentAndParent(string assignmentIds)
        {
            return _context.GetSurveyPrivateAssignmentOfStudentAndParent(assignmentIds)
                .Select(x => new AssignResult()
                {
                    QTITestClassAssignmentId = x.QTITestClassAssignmentID,
                    RoleId = x.RoleId
                }).ToList();
        }

        public IEnumerable<AnswerAttachmentDto> GetAnswerAttachments(int qtiOnlineTestSessionId)
        {
            return _context.GetAnswerAttachments(qtiOnlineTestSessionId)
                .Select(a => new AnswerAttachmentDto
                {
                    QTIOnlineTestSessionAnswerID = a.QTIOnlineTestSessionAnswerID,
                    DocumentGuid = a.DocumentGUID,
                    FileName = a.FileName,
                    FileType = a.FileType,
                    FilePath = a.FilePath,
                    AttachmentType = a.AttachmentType
                }).ToArray();
        }

        public SelectTestCustomDto GetGradeSubjectBankTestForRetake(string guid)
        {
            var result = _context.GetTestRetakeInfor(guid).FirstOrDefault();
            if (result != null)
            {
                return  new SelectTestCustomDto()
                {
                    VirtualTestID = result.OriginalTestID,
                    TestName = result.OriginalTestName,
                    BankName = result.BankName,
                    SubjectName = result.SubjectName,
                    GradeName = result.GradeName,
                    CurrentVirtualTestID = result.CurrentTestID.HasValue ? result.CurrentTestID.Value : 0,
                    DistrictID = result.DistrictID.HasValue ? result.DistrictID.Value : 0
                };
            }                

            return null;
        }

        public List<StudentRetakeCustomDto> GetStudentStatusForTestRetake(int virtualTestId, string guid)
        {
            var result = _context.GetStudentStatusForTestRetake(virtualTestId, guid).ToList();
            if (result != null)
            {
                var studentStatus = result.Select(o => new StudentRetakeCustomDto()
                {
                    StudentID = o.StudentID,
                    FullName = o.FullName,
                    TestStatus = o.StudentStatus,                    
                    VirtualTestName = o.TestName,
                    VirtualTestDisplayName = string.Empty,
                    ClassID = o.ClassID,
                    VirtualTestID = o.VirtualTestID,
                    RetakeType = o.RetakeType
                }).ToList();
                return studentStatus;
            }
            return null;
        }

        public List<RetakeListOfDisplayQuestionsDto> GetRetakeListOfDisplayQuestions(int virtualTestId, string studentIds, string guid, string testName)
        {
            var result = _context.GetRetakeListOfDisplayQuestions(virtualTestId, studentIds, guid, testName).ToList();
            if (result != null)
            {
                var listOfDisplayQuestions = result.Select(o => new RetakeListOfDisplayQuestionsDto()
                {
                    StudentID = o.StudentID,
                    ClassID = o.ClassID,
                    ListOfDisplayQuestions = o.ListOfDisplayQuestions
                }).ToList();
                return listOfDisplayQuestions;
            }
            return null;
        }

        public LoadAssignmentRetakeResponse GetTestAssignResultForRetake(LoadAssignmentRetakeRequest request)
        {
            LoadAssignmentRetakeResponse assignmentRetakeResponse = new LoadAssignmentRetakeResponse();
            int? totalRecord = 0;

            var testAssignsResult = _context.GetTestAssignResultForRetake(request.RetakeRequestGuid,
                    request.GeneralSearch, request.SortColumn, request.SortDirection, request.StartRow, request.PageSize, ref totalRecord).ToList();

            assignmentRetakeResponse.TotalRecord = totalRecord.GetValueOrDefault();

            assignmentRetakeResponse.RetakeTestAssignResults = testAssignsResult.Select(x => new RetakeTestAssignResultViewModel
            {
                Assigned = x.Assigned,
                ClassName = x.ClassName,
                ID = x.Id,
                IsActive = x.Status == 1,
                ShortGUID = x.ShortGUID,
                StudentCode = x.StudentCode,
                StudentFirstName = x.StudentFirstName,
                StudentId = x.StudentID,
                StudentLastName = x.StudentLastName,
                StudentName = x.StudentName,
                Test = x.Test,
                TestCode = x.TestCode,
                AuthenticationCode = x.AuthenticationCode
            }).ToList();

            return assignmentRetakeResponse;
        }
    }
}
