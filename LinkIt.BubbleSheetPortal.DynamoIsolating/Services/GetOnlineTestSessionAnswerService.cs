using System;
using LinkIt.BubbleSheetPortal.DynamoIsolating.BusinessRules;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories;
using LinkIt.BubbleSheetPortal.Models.Isolating;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Services
{
    public class GetOnlineTestSessionAnswerService : IGetOnlineTestSessionAnswerService
    {
        private readonly QTIOnlineTestSessionAnswerDynamo _qtiOnlineTestSessionAnswerDynamo;
        private readonly QTIOnlineTestSessionDynamo _qtiOnlineTestSessionDynamo;
        private readonly QTIOnlineTestSessionAnswerTimeTrackDynamo _qTIOnlineTestSessionAnswerTimeTrackDynamo;
        public GetOnlineTestSessionAnswerService(
            QTIOnlineTestSessionAnswerDynamo qtiOnlineTestSessionAnswerDynamo,
            QTIOnlineTestSessionDynamo qtiOnlineTestSessionDynamo,
            QTIOnlineTestSessionAnswerTimeTrackDynamo qTIOnlineTestSessionAnswerTimeTrackDynamo)
        {
            _qtiOnlineTestSessionAnswerDynamo = qtiOnlineTestSessionAnswerDynamo;
            _qtiOnlineTestSessionDynamo = qtiOnlineTestSessionDynamo;
            _qTIOnlineTestSessionAnswerTimeTrackDynamo = qTIOnlineTestSessionAnswerTimeTrackDynamo;
        }

        public IQueryable<OnlineTestSessionAnswerDTO> GetOnlineTestSessionAnswer(string qtiOnlineTestSessionIDs)
        {
            var result = new List<OnlineTestSessionAnswerDTO>();
            if (string.IsNullOrWhiteSpace(qtiOnlineTestSessionIDs)) return result.AsQueryable();

            var ids = qtiOnlineTestSessionIDs.Split(',');
            foreach (var id in ids)
            {
                var qtiOnlineTestSessionID = int.Parse(id);
                var qtiOnlineTestSession = _qtiOnlineTestSessionDynamo.GetByID(qtiOnlineTestSessionID);
                var timeTracks = _qTIOnlineTestSessionAnswerTimeTrackDynamo.GetQTIOnlineTestSessionAnswerTimeTrack(qtiOnlineTestSessionID);
                DateTime? lastLoginDate = null;
                int? statusId = null;
                if (qtiOnlineTestSession != null)
                {
                    lastLoginDate = qtiOnlineTestSession.LastLoginDate;
                    statusId = qtiOnlineTestSession.StatusID;
                }

                var qtiOnlineTestSessionAnswers = _qtiOnlineTestSessionAnswerDynamo.Search(qtiOnlineTestSessionID);
                result.AddRange(Transform(qtiOnlineTestSessionAnswers, timeTracks, lastLoginDate, statusId));
            }

            return result.AsQueryable();
        }
        //public OnlineTestSessionAnswerDTO GetOnlineTestSessionAnswerOfStudent(int qtiOnlineTestSessionID, int virtualQuestionID)
        //{
        //    var result = new List<OnlineTestSessionAnswerDTO>();

        //    var qtiOnlineTestSessionAnswers = _qtiOnlineTestSessionAnswerDynamo.SearchAnswerOfStudent(qtiOnlineTestSessionID, virtualQuestionID);
        //    result.AddRange(Transform(qtiOnlineTestSessionAnswers));
        //    return result.FirstOrDefault();
        //}
        public List<OnlineTestSessionAnswerDTO> Transform(List<QTIOnlineTestSessionAnswer> src, List<QTIOnlineTestSessionAnswerTimeTrack> allTimeTracks, DateTime? lastLoginDate = null, int? statusId = null)
        {
            if (src == null) return null;
            var result = new List<OnlineTestSessionAnswerDTO>();
            foreach (var item in src)
            {
                var data = Transform(item, lastLoginDate, statusId);
                var timeTracks = allTimeTracks.Where(x => x.VirtualQuestionID == item.VirtualQuestionID).ToList();
                var timesVisited = timeTracks.Count;
                var timeSpent =
                    timeTracks.Where(x => x.StartTimeUTC != null && x.EndTimeUTC != null)
                        .Sum(x => (x.EndTimeUTC.Value - x.StartTimeUTC.Value).TotalSeconds);
                if (timesVisited > 0)
                {
                    data.TimeSpent = Convert.ToInt32(timeSpent);
                    data.TimesVisited = timesVisited;
                }
                result.Add(data);
            }

            return result;
        }

        public OnlineTestSessionAnswerDTO Transform(QTIOnlineTestSessionAnswer src, DateTime? lastLoginDate = null, int? statusId = null)
        {
            if (src == null) return null;
            var dest = new OnlineTestSessionAnswerDTO
            {
                Answered = DetectAnswerdQTIOnlineTestSessionAnswer.Answered(src),
                AnswerOrder = src.AnswerOrder,
                ManualReview = false,
                QTIOnlineTestSessionID = src.QTIOnlineTestSessionID,
                QuestionOrder = src.QuestionOrder,
                VirtualQuestionID = src.VirtualQuestionID,
                TimeStamp = src.Timestamp,
                LastLoginDate = lastLoginDate,
                StatusID = statusId
            };

            return dest;
        }
    }
}
