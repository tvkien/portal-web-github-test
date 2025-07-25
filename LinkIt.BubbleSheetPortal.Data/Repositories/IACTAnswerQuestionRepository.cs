using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IACTAnswerQuestionRepository
    {
        List<ACTUnansweredQuestion> GetUnansweredQuestion(int virtualTestId, int bubbleSheetId, int studentId);
        List<ACTAlreadyAnsweredQuestion> GetAnsweredQuestion(int virtualTestId, int bubbleSheetId, int studentId);
         
        List<ACTAlreadyAnsweredQuestion> GetExistAnswerForResubmit(int virtualtestId, int bubbleSheetId, int studentId);

        List<ACTStudentStatus> GetListAnswerCoverPageAllStudent(int virtualTestId, string ticket);

        List<ACTStudentStatus> SATGetListAnswerCoverPageAllStudent(int virtualTestId, string ticket, int essaySectionID);

        List<ACTUnansweredQuestion> GetUnansweredQuestionError(int virtualTestId, int bubbleSheetId, int studentId);

        List<ACTAlreadyAnsweredQuestion> GetAllQuestions(int virtualtestId);
    }
}
