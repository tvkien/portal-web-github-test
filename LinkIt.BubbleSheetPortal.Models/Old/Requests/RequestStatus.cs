namespace LinkIt.BubbleSheetPortal.Models.Requests
{
    public enum RequestStatus
    {
        Pending,
        ValidationFailedContainsErrors,
        ValidationFailedWarnings,
        ValidationPassed,
        ProcessedWithWarnings,
        Processed
    }
}