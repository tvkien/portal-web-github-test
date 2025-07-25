namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Services
{
    public interface IUpdateAnswerTextService
    {
        void UpdateAnswerText(int? answerID, int? answerSubID, bool? saved);

        void UpdateAnswerText(string answerText, int answerId, int? answerSubID);
    }
}
