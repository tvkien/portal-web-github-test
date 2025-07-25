namespace LinkIt.BubbleSheetPortal.Common.Enum
{
    public enum GradingProcessStatusEnum
    {
        SuccessAndInPendingReview = 1,
        SuccessAndInCompleted = 2,
        FailedAndWaitingRetry = 3,
        FailedAndNotWaitingRetry = 4,
        NotStartedHaveNotYetSubmitedTest = 5,
        NotStartedHaveSubmitedTest = 6
    }
}
