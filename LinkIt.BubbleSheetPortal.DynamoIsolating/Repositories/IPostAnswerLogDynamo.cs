using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories
{
    public interface IPostAnswerLogDynamo
    {
        List<PostAnswerLog> GetPostAnswerLogs(int qtiOnlineTestSessionID);
    }
}
