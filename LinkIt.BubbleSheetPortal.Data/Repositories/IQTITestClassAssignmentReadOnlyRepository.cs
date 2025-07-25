using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.GradingShorcuts;
using LinkIt.BubbleSheetPortal.Models.Monitoring;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using LinkIt.BubbleSheetPortal.Models.DTOs.Survey;
using LinkIt.BubbleSheetPortal.Models.DTOs.TeacherReview;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting;
using LinkIt.BubbleSheetPortal.Models.DTOs.Retake;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IQTITestClassAssignmentReadOnlyRepository : IReadOnlyRepository<QTITestClassAssignment>
    {
        GetTestClassAssignmentsResponse GetTestClassAssignments(GetTestClassAssignmentsRequest request);

        IQueryable<QTITestClassAssignment> GetTestClassAssignments_V1(string assignDate, bool onlyShowPedingReview, bool showActiveClassTestAssignment,
                                                                    int? userID, int? districtID, int? qtiTestClassAssignmentId, int schoolID);

        IQueryable<QTITestClassAssignment> GetTestClassAssignmentsExport(string assignDate, bool onlyShowPedingReview,
            bool showActiveClassTestAssignment,
            int? userID, int? districtID, int? schoolId);
        IQueryable<QTITestClassAssignment> GetTestClassAssignmentsPassThrough(string assignmentCodes,
            bool onlyShowPedingReview, bool showActiveClassTestAssignment,
            int? userID, int? districtID);

        IQueryable<QTITestClassAssignment> GetTestClassAssignmentsPassThroughExport(string assignmentCodes,
            bool onlyShowPedingReview, bool showActiveClassTestAssignment,
            int? userID, int? districtID);
        IQueryable<QTITestStudentAssignment> GetTestStudentAssignments(int? qtiTestClassAssignmentID, int? districtID);

        IQueryable<QTIVirtualTest> GetQTIVirtualTest(int virtualTestID);
        IQueryable<QTITestState> GetTestState(int qtiOnlineTestSessionID);
        IQueryable<QTITestState> GetTestStateTOS(int qtiOnlineTestSessionID);
        void UpdateAnswerPointsEarned(int qtiOnlineTestSessionID, int answerID, int? answerSubID, int pointsEarned, int UserID);
        string GetPreferencesForOnlineTest(int? qtiTestClassAssignmentID);
        IEnumerable<GetPreferencesForOnlineTest_MultipleResult> GetPreferencesForOnlineTest(IEnumerable<int> qtiTestClassAssignmentIds);
        List<Preferences> GetPreferencesWithDefaultForOnlineTest(int? qtiTestClassAssignmentID, int userID);
        VirtualTestData GetVirtualTestByID(int virtualTestID);

        IQueryable<StudentAssignSameTest> CheckAssignSameTest(int? assignmentTypeID, string studentIDs, string classIDs, int virtualTestID, bool? isRoster, int? qtiGroupID);
        List<VirtualTestForPrinting> GetVirtualTestForPrinting(int virtualTestID);

        List<VirtualTestForPrinting> GetVirtualTestAnswerKeyForPrinting(int virtualTestID, int userID);
        void UpdateAnswerText(int? answerID, int? answerSubID, bool? saved);
        PrintTestOfStudentDTO GetPrintTestOfStudent(int virtualTestID, bool manuallyGradedOnly);

        void InactiveQtiTestClassAssignment(int districtId, int bankId);
        IQueryable<QTITestClassAssignmentOTT> GetTestClassAssignmentsOTT(string assignDate, int? userID, int? districtID, bool showActiveClassTestAssignment, int schoolID);

        List<GetProctorTestViewDataResult> GetProctorTestViewData(int qtiTestClassAssignmentID);

        IQueryable<QTITestClassAssignmentOTTRefresh> GetTestClassAssignmentsOTTRefresh(string qtiTestClassAssignmentIDs);

        AnswerSubInfo GetAnswerSubInfoByAnswerSubID(int qtiOnlineTestSessionID, int answerID, int answerSubID);
        AnswerInfo GetAnswerInfoByAnswerID(int qtiOnlineTestSessionID, int answerID);
        void GradingShortcutsCompleted(GradingShortcutsDTO dto);
        void GradingShortcutsPendingReview(GradingShortcutsDTO dto);
        AnswerViewer GetAnswerOfStudent(int testResultID, int virtualQuestionID);
        NextApplicableStudent GetNextApplicableStudent(int qTITestClassAssignmentId, int virtualQuestionId, bool isManuallyGradedOnly, int studentID, bool isOverrideAutoGraded);
        QTITestClassAssignmentData GetTestClassAssignmentDetails(int qtiTestClassAssignmentID);
        NextApplicableQuestion GetNextApplicableQuestion(int qTITestClassAssignmentId, int studentId, bool isManuallyGradedOnly, bool isOverrideAutoGraded);
        List<StudentTestStatus> GetStudentTestStatusByClassAssignmentId(int qtiTestClassAssignmentId);
        List<StudentTestStatus> GetListStudentTestStatus(int qtiTestClassAssignmentId);
        IQueryable<QTITestClassAssignmentForStudent> GetTestClassAssignmentsForStudent(int studentId, int districtID);

        List<AlgorithmicQuestionExpression> GetAlgorithmicQuestionExpressions(int virtualTestID);
        GetTestClassAssignmentsOTTResponse GetTestClassAssignmentsOTT(GetTestClassAssignmentsOTTRequest request);
        bool CheckPermissionsEditQTITestClassAssignment(int roleId, int userId, string qtiTestClassAssignmentIds);
        IQueryable<SurveyAssignmentResultsResponse> GetSurveyAssignmentResultsByType(int districtId, int districtTermId, int surveyId, int surveyAssignmentType, string sortBy, string searchText, int skip = 0, int take = 10, bool isShowInactive = true);
        void BatchSavePublicAssignSurvey(SurveyAssignParameter param);
        List<AssignResult> BatchSavePrivateAssignSurvey(SurveyAssignParameter param);
        List<QTITestClassAssignmentForSurveyDto> GetForSurvey(int userId, int studentId, int roleId);
        bool CheckAllowAssignSurvey(int districtId, int districtTermId, int surveyId, int surveyAssignmentType);
        List<AssignResult> GetSurveyPrivateAssignmentOfStudentAndParent(string assignmentIds);
        bool IsAllStudentTestSessionCompleted(int qtiTestClassAssignmentId);
        bool IsTestHasManuallyQuestion(int qtiTestClassAssignmentId);
        IEnumerable<AnswerAttachmentDto> GetAnswerAttachments(int qtiOnlineTestSessionId);
        SelectTestCustomDto GetGradeSubjectBankTestForRetake(string guid);
        List<StudentRetakeCustomDto> GetStudentStatusForTestRetake(int virtualTestId, string studentIds);
        List<RetakeListOfDisplayQuestionsDto> GetRetakeListOfDisplayQuestions(int virtualTestId, string studentIds, string guid, string testName);
        LoadAssignmentRetakeResponse GetTestAssignResultForRetake(LoadAssignmentRetakeRequest request);
    }
}
