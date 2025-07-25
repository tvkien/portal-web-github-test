using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class PreviouslyAnsweredQuestionsService
    {
        private readonly IReadOnlyRepository<PreviouslyAnsweredQuestion> repository;

        public PreviouslyAnsweredQuestionsService(IReadOnlyRepository<PreviouslyAnsweredQuestion> repository)
        {
            this.repository = repository;
        }

        public IQueryable<PreviouslyAnsweredQuestion> GetPreviouslyAnsweredQuestionsByStudentIdAndBubbleSheetId(int studentId, int bubbleSheetId)
        {
            //return repository.Select().Where(x => x.StudentId.Equals(studentId) && x.BubbleSheetId.Equals(bubbleSheetId) && x.WasAnswered);
            return repository.Select().Where(x => x.StudentId.Equals(studentId) && x.BubbleSheetId.Equals(bubbleSheetId));
        }
    }
}