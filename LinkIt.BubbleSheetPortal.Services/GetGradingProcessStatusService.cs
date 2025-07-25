using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public static class GetGradingProcessStatusService
    {
        public static GradingProcessStatusEnum GetGradingProcessStatus(QTIOnlineTestSession qtiOnlineTestSession, AutoGradingQueueData autoGradingQueue)
        {
            if(SuccessAndInCompleted(qtiOnlineTestSession, autoGradingQueue))
            {
                return GradingProcessStatusEnum.SuccessAndInCompleted;
            }

            if (SuccessAndInPendingReview(qtiOnlineTestSession, autoGradingQueue))
            {
                return GradingProcessStatusEnum.SuccessAndInPendingReview;
            }

            if (FailedAndWaitingRetry(qtiOnlineTestSession, autoGradingQueue))
            {
                return GradingProcessStatusEnum.FailedAndWaitingRetry;
            }

            if (NotStartedHaveSubmitedTest(qtiOnlineTestSession, autoGradingQueue))
            {
                return GradingProcessStatusEnum.NotStartedHaveSubmitedTest;
            }

            if (NotStartedHaveNotYetSubmitedTest(qtiOnlineTestSession, autoGradingQueue))
            {
                return GradingProcessStatusEnum.NotStartedHaveNotYetSubmitedTest;
            }

            if (FailedAndNotWaitingRetry(qtiOnlineTestSession, autoGradingQueue))
            {
                return GradingProcessStatusEnum.FailedAndNotWaitingRetry;
            }

            return GradingProcessStatusEnum.NotStartedHaveNotYetSubmitedTest;
        }

        public static bool SuccessAndInCompleted(QTIOnlineTestSession qtiOnlineTestSession, AutoGradingQueueData autoGradingQueue)
        {
            if (qtiOnlineTestSession == null) return false;
            if (qtiOnlineTestSession.StatusId != (int)QTIOnlineTestSessionStatusEnum.Completed) return false;
            if (autoGradingQueue == null) return false;
            if (autoGradingQueue.Status != 1) return false;

            return true;
        }

        public static bool SuccessAndInPendingReview(QTIOnlineTestSession qtiOnlineTestSession, AutoGradingQueueData autoGradingQueue)
        {
            if (qtiOnlineTestSession == null) return false;
            if (qtiOnlineTestSession.StatusId != (int)QTIOnlineTestSessionStatusEnum.PendingReview) return false;
            if (autoGradingQueue == null) return false;
            if (autoGradingQueue.Status != 1) return false;

            return true;
        }

        public static bool FailedAndWaitingRetry(QTIOnlineTestSession qtiOnlineTestSession, AutoGradingQueueData autoGradingQueue)
        {
            if (qtiOnlineTestSession == null) return false;
            if (qtiOnlineTestSession.StatusId != (int)QTIOnlineTestSessionStatusEnum.PendingReview
                && qtiOnlineTestSession.StatusId != (int)QTIOnlineTestSessionStatusEnum.Completed) return false;
            if (autoGradingQueue == null) return false;
            if (autoGradingQueue.Status != -1) return false;
            if (!autoGradingQueue.IsAwaitingRetry) return false;

            return true;
        }

        public static bool FailedAndNotWaitingRetry(QTIOnlineTestSession qtiOnlineTestSession, AutoGradingQueueData autoGradingQueue)
        {
            if (qtiOnlineTestSession == null) return false;
            if (qtiOnlineTestSession.StatusId != (int)QTIOnlineTestSessionStatusEnum.PendingReview
                && qtiOnlineTestSession.StatusId != (int)QTIOnlineTestSessionStatusEnum.Completed) return false;
            if (autoGradingQueue == null) return false;
            if (autoGradingQueue.Status != -1) return false;
            if (autoGradingQueue.IsAwaitingRetry) return false;

            return true;
        }

        public static bool NotStartedHaveSubmitedTest(QTIOnlineTestSession qtiOnlineTestSession, AutoGradingQueueData autoGradingQueue)
        {
            if (qtiOnlineTestSession == null) return false;
            if (qtiOnlineTestSession.StatusId != (int)QTIOnlineTestSessionStatusEnum.PendingReview
                && qtiOnlineTestSession.StatusId != (int)QTIOnlineTestSessionStatusEnum.Completed) return false;
            if (autoGradingQueue == null) return false;
            if (autoGradingQueue.Status != 0) return false;

            return true;
        }

        public static bool NotStartedHaveNotYetSubmitedTest(QTIOnlineTestSession qtiOnlineTestSession, AutoGradingQueueData autoGradingQueue)
        {
            if (qtiOnlineTestSession == null) return false;
            if (qtiOnlineTestSession.StatusId == (int)QTIOnlineTestSessionStatusEnum.PendingReview
                || qtiOnlineTestSession.StatusId == (int)QTIOnlineTestSessionStatusEnum.Completed) return false;
            if (autoGradingQueue == null) return true;

            if (autoGradingQueue.Status == 0) return false;

            return true;
        }
    }
}
