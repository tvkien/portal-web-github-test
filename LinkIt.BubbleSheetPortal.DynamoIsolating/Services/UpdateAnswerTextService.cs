using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Services
{
    public class UpdateAnswerTextService : IUpdateAnswerTextService
    {
        private IQTIOnlineTestSessionAnswerDynamo _qtiOnlineTestSessionAnswerDynamo;
        public UpdateAnswerTextService(IQTIOnlineTestSessionAnswerDynamo qtiOnlineTestSessionAnswerDynamo)
        {
            _qtiOnlineTestSessionAnswerDynamo = qtiOnlineTestSessionAnswerDynamo;
        }
        public void UpdateAnswerText(int? answerID, int? answerSubID, bool? saved)
        {
            if (!answerID.HasValue) return;

            var qtiOnlineTestSessionAnswer = _qtiOnlineTestSessionAnswerDynamo.GetByID(answerID.Value);
            if (qtiOnlineTestSessionAnswer == null) return;

            if (!answerSubID.HasValue)
            {
                UpdateAnswerText(saved, qtiOnlineTestSessionAnswer);
                return;
            }

            UpdateAnswerSubText(saved, answerSubID.Value, qtiOnlineTestSessionAnswer);
        }

        public void UpdateAnswerText(bool? saved, QTIOnlineTestSessionAnswer qtiOnlineTestSessionAnswer)
        {
            if (qtiOnlineTestSessionAnswer == null) return;

            if (saved.HasValue && saved.Value)
            {
                _qtiOnlineTestSessionAnswerDynamo.UpdateAnswerText(qtiOnlineTestSessionAnswer.QTIOnlineTestSessionID, qtiOnlineTestSessionAnswer.QTIOnlineTestSessionAnswerID, qtiOnlineTestSessionAnswer.AnswerText);
            }
            else
            {
                _qtiOnlineTestSessionAnswerDynamo.UpdateAnswerTemp(qtiOnlineTestSessionAnswer.QTIOnlineTestSessionID, qtiOnlineTestSessionAnswer.QTIOnlineTestSessionAnswerID, null);
            }
        }

        public void UpdateAnswerSubText(bool? saved, int answerSubID, QTIOnlineTestSessionAnswer qtiOnlineTestSessionAnswer)
        {
            if (qtiOnlineTestSessionAnswer.QTIOnlineTestSessionAnswerSubs == null || qtiOnlineTestSessionAnswer.QTIOnlineTestSessionAnswerSubs.Count == 0) return;
            var qtiOnlineTestSessionAnswerSub = qtiOnlineTestSessionAnswer.QTIOnlineTestSessionAnswerSubs.FirstOrDefault(o => o.QTIOnlineTestSessionAnswerSubID == answerSubID);
            if (qtiOnlineTestSessionAnswerSub == null) return;
            if (saved.HasValue && saved.Value)
            {
                qtiOnlineTestSessionAnswerSub.AnswerText = qtiOnlineTestSessionAnswerSub.AnswerTemp;
                qtiOnlineTestSessionAnswerSub.Answered = true;
                qtiOnlineTestSessionAnswerSub.AnswerTemp = null;
            }
            else
            {
                qtiOnlineTestSessionAnswerSub.AnswerTemp = null;
            }

            _qtiOnlineTestSessionAnswerDynamo.UpdateQTIOnlineTestSessionAnswerSubs(qtiOnlineTestSessionAnswer.QTIOnlineTestSessionID, qtiOnlineTestSessionAnswer.QTIOnlineTestSessionAnswerID, qtiOnlineTestSessionAnswer.QTIOnlineTestSessionAnswerSubs);
        }

        /// <summary>
        /// Support recover answer from Post Answer Log
        /// </summary>
        /// <param name="answerText"></param>
        /// <param name="answerId"></param>
        /// <param name="answerSubID"></param>
        public void UpdateAnswerText(string answerText, int answerId, int? answerSubID)
        {
            var answer = _qtiOnlineTestSessionAnswerDynamo.GetByID(answerId);
            if (answer == null) return;

            // If single question, update table answer
            if(answerSubID.HasValue == false)
            {
                //_qtiOnlineTestSessionAnswerDynamo.UpdateAnswerText(qtiOnlineTestSessionAnswer.QTIOnlineTestSessionID, qtiOnlineTestSessionAnswer.QTIOnlineTestSessionAnswerID, qtiOnlineTestSessionAnswer.AnswerText);
                _qtiOnlineTestSessionAnswerDynamo.UpdateAnswerText(answer.QTIOnlineTestSessionID, answerId, answerText);
            }
            else // multipart question, update table answer sub
            {
                // update answer sub text
                //if (qtiOnlineTestSessionAnswer.QTIOnlineTestSessionAnswerSubs == null || qtiOnlineTestSessionAnswer.QTIOnlineTestSessionAnswerSubs.Count == 0) return;
                if (answer.QTIOnlineTestSessionAnswerSubs == null || answer.QTIOnlineTestSessionAnswerSubs.Count == 0) return;
                var answerSub = answer.QTIOnlineTestSessionAnswerSubs.FirstOrDefault(x => x.QTIOnlineTestSessionAnswerSubID == answerSubID.Value);
                if (answerSub != null)
                {
                    answerSub.AnswerText = answerText;
                    _qtiOnlineTestSessionAnswerDynamo.UpdateQTIOnlineTestSessionAnswerSubs(
                        qtiOnlineTestSessionID: answer.QTIOnlineTestSessionID,
                        answerID: answerId,
                        answerSubs: answer.QTIOnlineTestSessionAnswerSubs);
                } 
            }
        }
    }
}
