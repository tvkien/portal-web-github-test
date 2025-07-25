using System.Linq;

namespace LinkIt.BubbleSheetPortal.Models.Interfaces
{
    public interface IUansweredQuestionsRepository<T> where T : class
    {
        IQueryable<T> SelectQuestionsWithResults();
        IQueryable<T> SelectChoicesForQuestions();
        IQueryable<T> GetAllQuestionOfBubbleSheet(int studentID, string ticket, int classId);
    }
}