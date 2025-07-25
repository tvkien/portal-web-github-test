using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Services
{
    public class SGOManagerLogService : ISGOManagerLogService
    {
        private ISGOManagerLogDynamo _sgoManagerLogDynamo;
        public SGOManagerLogService(ISGOManagerLogDynamo sgoManagerLogDynamo)
        {
            _sgoManagerLogDynamo = sgoManagerLogDynamo;
        }

        public SGOManagerLog GetByID(string sgoManagerLogId)
        {
            var sgoManagerLog = _sgoManagerLogDynamo.GetByID(sgoManagerLogId);
            return sgoManagerLog;            
        }

        public void PutItem(SGOManagerLog sgoManagerLog)
        {
            _sgoManagerLogDynamo.PutItem(sgoManagerLog);
        }

        public void UpdateItem(SGOManagerLog sgoManagerLog)
        {
            _sgoManagerLogDynamo.UpdateItem(sgoManagerLog);
        }
    }
}
