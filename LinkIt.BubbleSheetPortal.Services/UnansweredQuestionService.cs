using System;
using System.Linq;
using System.Linq.Expressions;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class UnansweredQuestionService
    {
        private readonly IUansweredQuestionsRepository<UnansweredQuestion> repository;

        public UnansweredQuestionService(IUansweredQuestionsRepository<UnansweredQuestion> repository)
        {
            this.repository = repository;
        }

        public bool VerifyUnansweredQuestionsExistByStudentIdAndBubbleSheetId(int studentId, int bubbleSheetId, int classId)
        {
            return repository.SelectQuestionsWithResults().Any(x => x.StudentId.Equals(studentId) && x.BubbleSheetId.Equals(bubbleSheetId) && x.ClassId.Equals(classId));
        }

        public int GetQuestionOrderByQuestionId(int questionId)
        {
            var question = repository.SelectQuestionsWithResults().FirstOrDefault(x => x.QuestionId.Equals(questionId));
            return question.IsNull() ? 0 : question.QuestionOrder;
        }

        public IQueryable<UnansweredQuestion> GetUnansweredQuestionsByTicketAndStudentId(string ticket, int studentId, int classId)
        {
            return repository.SelectQuestionsWithResults().Where(GetByTicketAndStudentId(ticket, studentId, classId)).OrderBy(x => x.QuestionOrder);
        }

        public IQueryable<UnansweredQuestion> GetQuestionsWithAnswerChoicesForMissingSheets(string ticket, int studentId, int classId)
        {
            return repository.SelectChoicesForQuestions().Where(GetByTicketAndStudentId(ticket, studentId, classId)).OrderBy(x => x.QuestionOrder);
        }

        public IQueryable<UnansweredQuestion> GetAllQuestionOfBubbleSheet(int studentid, string ticket, int classId)
        {
            return repository.GetAllQuestionOfBubbleSheet(studentid, ticket, classId);
        }

        private Expression<Func<UnansweredQuestion, bool>> GetByTicketAndStudentId(string ticket, int studentId, int classId)
        {
            return x => x.Ticket.Equals(ticket) && x.StudentId.Equals(studentId) && x.ClassId.Equals(classId);
        }

        private Expression<Func<UnansweredQuestion, bool>> GetByTicketAndStudentId(string ticket, int studentId)
        {
            return x => x.Ticket.Equals(ticket) && x.StudentId.Equals(studentId);
        }


    }
}