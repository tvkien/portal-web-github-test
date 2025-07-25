using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories
{
    public interface ISGOManagerLogDynamo
    {
        SGOManagerLog GetByID(string sgoManagerLogId);
        void PutItem(SGOManagerLog sgoManagerLog);
        void UpdateItem(SGOManagerLog sgoManagerLog);
    }
}
