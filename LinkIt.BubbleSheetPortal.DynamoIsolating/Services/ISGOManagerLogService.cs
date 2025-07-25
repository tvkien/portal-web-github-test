using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Services
{
    public interface ISGOManagerLogService
    {
        SGOManagerLog GetByID(string sgoManagerLogId);
        void PutItem(SGOManagerLog sgoManagerLog);
        void UpdateItem(SGOManagerLog sgoManagerLog);
    }
}
