using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ACTAnswerQuestionService
    {
        private readonly IACTAnswerQuestionRepository _repository;

        public ACTAnswerQuestionService(IACTAnswerQuestionRepository repository)
        {
            _repository = repository;
        }

         /// <summary>
        /// Get List Question Unanswered include quenstion had answer and not answer yet
         /// </summary>
         /// <param name="virtualTestId"></param>
         /// <param name="bubbleSheetId"></param>
         /// <param name="studentId"></param>
         /// <returns></returns>
        public List<ACTUnansweredQuestion> GetUnansweredQuestions(int virtualTestId, int bubbleSheetId, int studentId)
        {
            return _repository.GetUnansweredQuestion(virtualTestId, bubbleSheetId, studentId);
        }

        /// <summary>
        /// Get Answers have WasAnswered = 1
        /// </summary>
        /// <param name="virtualTestId"></param>
        /// <param name="bubbleSheetId"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
 
        public List<ACTAlreadyAnsweredQuestion> GetAnsweredQuestions(int virtualTestId, int bubbleSheetId, int studentId)
        {
            return _repository.GetAnsweredQuestion(virtualTestId, bubbleSheetId, studentId);
        }


        /// <summary>
        /// Get All Answers by VirtualTestId
        /// </summary>
        /// <param name="virtualTestId"></param>
        /// <returns></returns>
        public List<ACTAlreadyAnsweredQuestion> GetExistAnswerForResubmit(int virtualTestId, int bubbleSheetid, int studentId)
        {
            return _repository.GetExistAnswerForResubmit(virtualTestId, bubbleSheetid, studentId);
        }

        /// <summary>
        /// Get List Answer CoverPage for All Students
        /// </summary>
        /// <param name="virtualtestId"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public List<ACTStudentStatus> GetActStudentStatuses(int virtualtestId, string ticket)
        {
            return _repository.GetListAnswerCoverPageAllStudent(virtualtestId, ticket);
        }

        public List<ACTStudentStatus> GetSATStudentStatuses(int virtualtestId, string ticket, int essaySectionID)
        {
            return _repository.SATGetListAnswerCoverPageAllStudent(virtualtestId, ticket, essaySectionID);
        }

        public List<ACTUnansweredQuestion> GetUnansweredQuestionError(int virtualTestId, int bubbleSheetId,
            int studentId)
        {
            return _repository.GetUnansweredQuestionError(virtualTestId, bubbleSheetId, studentId);
        }

        public List<ACTAlreadyAnsweredQuestion> GetAllQuestions(int virtualTestId)
        {
            return _repository.GetAllQuestions(virtualTestId);
        }
    }
}
