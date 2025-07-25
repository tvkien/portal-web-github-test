using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestMaker;
using LinkIt.BubbleSheetPortal.Services.Isolating;

namespace LinkIt.BubbleSheetPortal.Services.TeacherReviewer
{
    public class TeacherReviewerService : ITeacherReviewerService
    {
        private QTIOnlineTestSessionService _qtiOnlineTestSessionService;
        private QTITestClassAssignmentService _qtiTestClassAssignmentService;
        private PreferencesService _preferenceService;
        private IsolatingTestTakerService _isolatingTestTakerService;

        public TeacherReviewerService(QTIOnlineTestSessionService qtiOnlineTestSessionService,
            QTITestClassAssignmentService qtiTestClassAssignmentService,
            PreferencesService preferenceService,
            IsolatingTestTakerService isolatingTestTakerService)
        {
            _qtiOnlineTestSessionService = qtiOnlineTestSessionService;
            _qtiTestClassAssignmentService = qtiTestClassAssignmentService;
            _preferenceService = preferenceService;
            _isolatingTestTakerService = isolatingTestTakerService;
        }

        public bool CanGrading(CanGradingModel model)
        {
            var qtiOnlineTestSession =
                _qtiOnlineTestSessionService.Select()
                    .FirstOrDefault(o => o.QTIOnlineTestSessionId == model.QTIOnlineTestSessionID);
            if (qtiOnlineTestSession == null) return false;

            var status = _isolatingTestTakerService.GetQTIOnlineTestSessionStatus(model.QTIOnlineTestSessionID);
            if (status.HasValue) qtiOnlineTestSession.StatusId = status.Value;

            var qtiTestClassAssignment =
                _qtiTestClassAssignmentService.GetQtiTestClassAssignmentByAssignmentGUID(
                    qtiOnlineTestSession.AssignmentGUId);
            if (qtiTestClassAssignment == null) return false;

            var overridePreferenceOn = GetOverrideAutoGradedOfAssignment(qtiTestClassAssignment.QTITestClassAssignmentId, model.RoleID);
            var gradingProcessStatus = _qtiTestClassAssignmentService.GetGradingProcessStatus(model.QTIOnlineTestSessionID);
            var canotGrading = DetectCanotGrading(gradingProcessStatus);
            if (canotGrading) return false;


            var expired = false;
            if (qtiOnlineTestSession.StatusId == 2 || qtiOnlineTestSession.StatusId == 3)
            {
                var preference =
                            _qtiTestClassAssignmentService.GetPreferencesForOnlineTest(
                                qtiTestClassAssignment.QTITestClassAssignmentId);
                expired = CheckExpired(qtiOnlineTestSession.StartDate, preference);
            }

            var answerInfo = _qtiTestClassAssignmentService.GetAnswerInfoByAnswerID(
                            model.QTIOnlineTestSessionID, model.AnswerID);
            if (answerInfo == null) return false;

            AnswerSubInfo answerSubInfo = null;
            if (model.AnswerSubID.HasValue)
            {
                answerSubInfo = _qtiTestClassAssignmentService.GetAnswerSubInfoByAnswerSubID(model.QTIOnlineTestSessionID,
                                   model.AnswerID, model.AnswerSubID.Value);
            }

            if (qtiOnlineTestSession.StatusId == 1) return false;
            if ((qtiOnlineTestSession.StatusId == 2 || qtiOnlineTestSession.StatusId == 3) && !expired) return false;

            var isAlgorithmic = answerInfo.ResponseProcessingTypeID == (int)LinkIt.BubbleSheetPortal.Common.Enum.ResponseProcessingTypeEnum.AlgorithmicScoring;

            var result = true;
            var overrideQTISchemaIDs = GetOverrideItems(model.RoleID, overridePreferenceOn, isAlgorithmic);
            if (qtiOnlineTestSession.StatusId == 5 || ((qtiOnlineTestSession.StatusId == 3 || qtiOnlineTestSession.StatusId == 2) && expired))
            {
                if (model.AnswerSubID.HasValue)
                {
                    if (answerSubInfo == null) return false;
                    if (answerSubInfo.QTISchemaID == (int)QTISchemaEnum.ExtendedText) return true;
                    if (answerSubInfo.QTISchemaID == (int)QTISchemaEnum.TextEntry)
                    {
                        if (answerSubInfo.ResponseProcessingTypeID == 2) return true;
                        return overridePreferenceOn;
                    }

                    return overridePreferenceOn;
                }

                if (!overridePreferenceOn)
                {
                    result = result && answerInfo.QTISchemaID == 9 && answerInfo.ResponseProcessingTypeID == 2;
                    result = result || answerInfo.QTISchemaID == 10;
                }
                else
                {
                    var existItem = overrideQTISchemaIDs.Any(o => o == answerInfo.QTISchemaID);
                    result = result && existItem;
                }

                return result;
            }

            if (qtiOnlineTestSession.StatusId == 4)
            {

                bool isManualQuestion = false;
                if (answerSubInfo == null)
                {
                    isManualQuestion = answerInfo.QTISchemaID == 9 && answerInfo.ResponseProcessingTypeID == 2;
                    isManualQuestion = isManualQuestion || answerInfo.QTISchemaID == 10;
                }
                else
                {
                    isManualQuestion = answerSubInfo.QTISchemaID == 9 && answerSubInfo.ResponseProcessingTypeID == 2;
                    isManualQuestion = isManualQuestion || answerSubInfo.QTISchemaID == 10;
                }


                if (!overridePreferenceOn && !isManualQuestion)
                    return false;

                result = result || isManualQuestion;

                var existItem = overrideQTISchemaIDs.Any(o => o == answerInfo.QTISchemaID);
                result = result && existItem;
            }

            return result;
        }

        public bool DetectCanotGrading(GradingProcessStatusEnum gradingStatus)
        {
            if (gradingStatus == GradingProcessStatusEnum.NotStartedHaveNotYetSubmitedTest
                || gradingStatus == GradingProcessStatusEnum.NotStartedHaveSubmitedTest
                || gradingStatus == GradingProcessStatusEnum.FailedAndNotWaitingRetry
                || gradingStatus == GradingProcessStatusEnum.FailedAndWaitingRetry) return true;

            return false;
        }



        public bool GetOverrideAutoGradedOfAssignment(int qtiTestClassAssignmentID, int roleID)
        {
            if (RoleUtil.IsPublisher(roleID) || RoleUtil.IsNetworkAdmin(roleID) ||
                RoleUtil.IsDistrictAdmin(roleID))
            {
                return true;
            }

            var preference = _preferenceService.GetPreferenceByAssignmentLeveAndID(qtiTestClassAssignmentID);
            if (preference != null)
            {
                var obj = new ETLXmlSerialization<TestSettingsMapDTO>();
                var tsmap = obj.DeserializeXmlToObject(preference.Value);
                if (tsmap != null && tsmap.TestSettingViewModel != null)
                {
                    if (tsmap.TestSettingViewModel.OverrideAutoGradedTextEntry == "1") return true;
                }
            }

            return false;
        }

        public List<int> GetOverrideItems(int roleID, bool overridePreferenceOn, bool isAlgorithmic = false)
        {
            return PreferenceUtil.GetOverrideItems(roleID, overridePreferenceOn, isAlgorithmic);
        }

        public bool CheckExpired(string startDate, PreferenceOptions preference)
        {
            var startDateTime = GetDateTime(startDate);
            var expired = CheckExpired(startDateTime, preference);

            return expired;
        }

        public bool CheckExpired(DateTime? startDateTime, PreferenceOptions preference)
        {
            var expired = false;
            if (preference != null && preference.TimeLimit.HasValue && preference.TimeLimit.Value)
            {
                if (startDateTime.HasValue && preference.Duration.HasValue && preference.Duration.Value > 0)
                {
                    var endDate = startDateTime.Value.AddMinutes(preference.Duration.Value);
                    expired = DateTime.Compare(DateTime.UtcNow, endDate) > 0;
                }
                else if (preference.Deadline.HasValue)
                {
                    expired = DateTime.Compare(DateTime.UtcNow, preference.Deadline.Value) > 0;
                }
            }
            return expired;
        }

        private DateTime? GetDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            DateTime result;
            if (DateTime.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }
    }
}
