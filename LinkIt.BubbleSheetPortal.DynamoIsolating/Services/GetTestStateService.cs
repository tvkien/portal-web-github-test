using System;
using LinkIt.BubbleSheetPortal.DynamoIsolating.BusinessRules;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories;
using LinkIt.BubbleSheetPortal.Models.Isolating;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Services
{
    public class GetTestStateService : IGetTestStateService
    {
        private QTIOnlineTestSessionAnswerDynamo _qtiOnlineTestSessionAnswerDynamo;
        private QTIOnlineTestSessionAnswerTimeTrackDynamo _qTIOnlineTestSessionAnswerTimeTrackDynamo;
        public GetTestStateService(QTIOnlineTestSessionAnswerDynamo qtiOnlineTestSessionAnswerDynamo,
                                    QTIOnlineTestSessionAnswerTimeTrackDynamo qTIOnlineTestSessionAnswerTimeTrackDynamo)
        {
            _qtiOnlineTestSessionAnswerDynamo = qtiOnlineTestSessionAnswerDynamo;
            _qTIOnlineTestSessionAnswerTimeTrackDynamo = qTIOnlineTestSessionAnswerTimeTrackDynamo;
        }

        public IQueryable<IsolatingTestSessionAnswerDTO> GetTestState(int qtiOnlineTestSessionID)
        {
            var data = _qtiOnlineTestSessionAnswerDynamo.Search(qtiOnlineTestSessionID);
            var allTimeTracks =
           _qTIOnlineTestSessionAnswerTimeTrackDynamo.GetQTIOnlineTestSessionAnswerTimeTrack(qtiOnlineTestSessionID);
            foreach (var answer in data)
            {
                var timeTracks = allTimeTracks.Where(x => x.VirtualQuestionID == answer.VirtualQuestionID).ToList();           
                var timesVisited = timeTracks.Count();
                var timeSpent =
                    timeTracks.Where(x => x.StartTimeUTC != null && x.EndTimeUTC != null)
                        .Sum(x => (x.EndTimeUTC.Value - x.StartTimeUTC.Value).TotalSeconds);
                if (timesVisited > 0)
                {
                    answer.TimeSpent = Convert.ToInt32(timeSpent);
                    answer.TimesVisited = timesVisited;
                }
            }
            var result = Transform(data);

            if (result == null) result = new List<IsolatingTestSessionAnswerDTO>();
           
            return result.AsQueryable();
        }

        public static IsolatingTestSessionAnswerDTO Transform(QTIOnlineTestSessionAnswer x)
        {
            if (x == null) return null;

            var result = new IsolatingTestSessionAnswerDTO
            {
                QTIOnlineTestSessionAnswerID = x.QTIOnlineTestSessionAnswerID,
                Answered = DetectAnswerdQTIOnlineTestSessionAnswer.Answered(x),
                AnswerChoice = x.AnswerChoice,
                AnswerText = x.AnswerText,
                AnswerImage = x.AnswerImage,
                PointsEarned = x.PointsEarned,
                QTIOTSessionAnswerSubs = Transform(x.QTIOnlineTestSessionAnswerSubs),
                HighlightQuestion = x.HighlightQuestion,
                HighlightPassage = x.HighlightPassage,
                HighlightQuestionGroupCommon = x.HighlightQuestionGroupCommon,
                AnswerTemp = x.AnswerTemp,
                AnswerOrder = x.AnswerOrder,
                TimesVisited = x.TimesVisited,
                TimeSpent = x.TimeSpent,
                DrawingContent = x.DrawingContent
            };

            return result;
        }

        public static List<IsolatingTestSessionAnswerDTO> Transform(List<QTIOnlineTestSessionAnswer> source)
        {
            if (source == null) return null;

            var result = new List<IsolatingTestSessionAnswerDTO>();
            foreach (var item in source)
            {
                var dto = Transform(item);
                result.Add(dto);
            }

            return result;
        }

        public static IsolatingTestSessionAnswerSubDTO Transform(QTIOnlineTestSessionAnswerSub x)
        {
            if (x == null) return null;

            var result = new IsolatingTestSessionAnswerSubDTO
            {
                QTIOnlineTestSessionAnswerSubID = x.QTIOnlineTestSessionAnswerSubID,
                QTIOnlineTestSessionAnswerID = x.QTIOnlineTestSessionAnswerID,
                VirtualQuestionSubID = x.VirtualQuestionSubID,
                PointsEarned = x.PointsEarned,
                Answered = !x.Answered.HasValue ? false : x.Answered.Value,
                AnswerChoice = x.AnswerChoice,
                AnswerText = x.AnswerText,
                AnswerTemp = x.AnswerTemp,
                Timestamp = x.Timestamp
            };

            return result;
        }

        public static List<IsolatingTestSessionAnswerSubDTO> Transform(List<QTIOnlineTestSessionAnswerSub> source)
        {
            if (source == null) return null;

            var result = new List<IsolatingTestSessionAnswerSubDTO>();
            foreach(var item in source)
            {
                var dto = Transform(item);
                result.Add(dto);
            }

            return result;
        }

    }
}
