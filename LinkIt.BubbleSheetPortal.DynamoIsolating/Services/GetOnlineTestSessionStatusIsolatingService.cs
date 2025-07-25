using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories;
using LinkIt.BubbleSheetPortal.Models.Isolating;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Services
{
    public class GetOnlineTestSessionStatusIsolatingService : IGetOnlineTestSessionStatusIsolatingService
    {
        private IQTIOnlineTestSessionAnswerDynamo _qtiOnlineTestSessionAnswerDynamo;
        private IQTITestClassAssignmentDynamo _qtiTestClassAssignmentDynamo;
        private IQTIOnlineTestSessionDynamo _qtiOnlineTestSessionDynamo;

        public GetOnlineTestSessionStatusIsolatingService(IQTIOnlineTestSessionAnswerDynamo qtiOnlineTestSessionAnswerDynamo
            , IQTITestClassAssignmentDynamo qtiTestClassAssignmentDynamo
            , IQTIOnlineTestSessionDynamo qtiOnlineTestSessionDynamo)
        {
            _qtiOnlineTestSessionAnswerDynamo = qtiOnlineTestSessionAnswerDynamo;
            _qtiTestClassAssignmentDynamo = qtiTestClassAssignmentDynamo;
            _qtiOnlineTestSessionDynamo = qtiOnlineTestSessionDynamo;
        }

        public IQueryable<OnlineTestSessionDTO> GetOnlineTestSessionStatusIsolating(string qtiTestClassAssignmentIDs)
        {
            var result = new List<OnlineTestSessionDTO>();
            if (string.IsNullOrWhiteSpace(qtiTestClassAssignmentIDs)) return result.AsQueryable();

            var ids = qtiTestClassAssignmentIDs.Split(',');
            foreach (var id in ids)
            {
                var qtiTestClassAssignmentID = int.Parse(id);
                var qtiTestClassAssignment = _qtiTestClassAssignmentDynamo.GetByID(qtiTestClassAssignmentID);
                if (qtiTestClassAssignment == null) continue;
                var qtiOnlineTestSessions = _qtiOnlineTestSessionDynamo.Search(qtiTestClassAssignment.AssignmentGUID);
                if (qtiOnlineTestSessions == null) continue;
                
                foreach (var qtiOnlineTestSession in qtiOnlineTestSessions)
                {
                    var onlineTestSessionDTO = new OnlineTestSessionDTO
                    {
                        QTITestClassAssignmentId = qtiTestClassAssignmentID,
                        QTIOnlineTestSessionId = qtiOnlineTestSession.QTIOnlineTestSessionID,
                        StatusId = qtiOnlineTestSession.StatusID,
                        LastLoginDate = qtiOnlineTestSession.LastLoginDate
                    };
                    result.Add(onlineTestSessionDTO);

                    var qtiOnlineTestSessionAnswers = _qtiOnlineTestSessionAnswerDynamo.Search(qtiOnlineTestSession.QTIOnlineTestSessionID);
                    if (qtiOnlineTestSessionAnswers != null && qtiOnlineTestSessionAnswers.Count > 0) onlineTestSessionDTO.Timestamp = qtiOnlineTestSessionAnswers.Max(o => o.Timestamp);
                }
            }

            return result.AsQueryable();
        }
        public List<int> GetSectionSubmiteds(int qtiOnlineTestSessionId)
        {
            var qtiOnlineTestSession = _qtiOnlineTestSessionDynamo.GetByID(qtiOnlineTestSessionId);
            if (qtiOnlineTestSession == null) return new List<int>();

            return SectionFlagData.ParseFromJson(qtiOnlineTestSession.SectionFlag).Where(x => x.Status).Select(x => x.VirtualSectionID).ToList();
        }
    }
}
