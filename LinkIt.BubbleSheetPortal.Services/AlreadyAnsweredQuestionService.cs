using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class AlreadyAnsweredQuestionService
    {
        private readonly IReadOnlyRepository<AlreadyAnsweredQuestion> repository;

        public AlreadyAnsweredQuestionService(IReadOnlyRepository<AlreadyAnsweredQuestion> repository)
        {
            this.repository = repository;
        }

        public IQueryable<AlreadyAnsweredQuestion> GetAlreadyAnsweredQuestionsForStudentBubbleSheet(int studentId, int bubbleSheetId)
        {
            return repository.Select().Where(x => x.StudentId.Equals(studentId) && x.BubbleSheetId.Equals(bubbleSheetId));
        }
    }
}