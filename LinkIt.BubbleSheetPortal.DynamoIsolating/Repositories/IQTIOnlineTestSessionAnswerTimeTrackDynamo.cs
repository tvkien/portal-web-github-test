using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories
{
    public interface IQTIOnlineTestSessionAnswerTimeTrackDynamo
    {
        List<QTIOnlineTestSessionAnswerTimeTrack> GetQTIOnlineTestSessionAnswerTimeTrack(int qtiOnlineTestSessionId);
        int GetTotalSpentTimeByQTIOnlineTestSessionID(int qtiOnlineTestSessionId);
    }
}
