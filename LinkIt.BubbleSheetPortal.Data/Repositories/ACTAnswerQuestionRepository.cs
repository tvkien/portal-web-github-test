using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ACTAnswerQuestionRepository : IACTAnswerQuestionRepository
    {
        private readonly BubbleSheetDataContext _dbContext;

        public ACTAnswerQuestionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _dbContext = BubbleSheetDataContext.Get(connectionString);
        }

        public List<ACTUnansweredQuestion> GetUnansweredQuestion(int virtualTestId, int bubbleSheetId, int studentId)
        {
            return _dbContext.ACTUnansweredQuestionsProc(bubbleSheetId, studentId, virtualTestId).Select(x => new ACTUnansweredQuestion()
            {
                QuestionId = x.VirtualQuestionID,
                QuestionOrder = x.QuestionOrder,
                PointsPossible = x.PointsPossible,
                QTISchemaId = x.QTISchemaID,
                AnswerIdentifiers = x.AnswerIdentifiers,
                StudentId = x.StudentID,
                BubbleSheetId = x.BubbleSheetID,
                Ticket = x.Ticket,
                XmlContent = x.XmlContent,
                OrderSection = x.SectionOrder,
                OrderSectionQuestion = x.SectionQuestionOrder,
                SectionTitle = x.Title,
                VirtualSectionId = x.VirtualSectionID,
                OrderSectionIndex = x.SectionIndex,
                OrderSectionQuestionIndex = x.SectionQuestionIndex,
                IsMultiMarkQuestion = x.BubbleSheetErrorType == "M"
            }).ToList();
        }

        public List<ACTAlreadyAnsweredQuestion> GetAnsweredQuestion(int virtualTestId, int bubbleSheetId, int studentId)
        {
            return _dbContext.ACTAlreadyAnsweredQuestionProc(bubbleSheetId, studentId, virtualTestId).Select(x => new ACTAlreadyAnsweredQuestion()
            {
                QuestionId = x.VirtualQuestionID,
                QuestionOrder = x.QuestionOrder,
                AnswerIdentifiers = x.AnswerIdentifiers,
                QTISchemaId = x.QTISchemaID,
                PointsPossible = x.PointsPossible,
                AnswerLetter = x.AnswerLetter,
                StudentId = x.StudentID,
                BubbleSheetId = x.BubbleSheetID,
                Ticket = x.Ticket,
                XmlContent = x.XmlContent,
                OrderSection = x.SectionOrder,
                OrderSectionQuestion = x.SectionQuestionOrder,
                SectionTitle = x.Title,
                VirtualSectionId = x.VirtualSectionID,
                OrderSectionIndex = x.SectionIndex,
                OrderSectionQuestionIndex = x.SectionQuestionIndex
            }).ToList();
        }

        /// <summary>
        /// Get All Answers by Virtualtest
        /// </summary>
        /// <param name="virtualTestId"></param>
        /// <returns></returns>
        public List<ACTAlreadyAnsweredQuestion> GetExistAnswerForResubmit(int virtualTestId, int bubblesheetId, int studentid)
        {
            return _dbContext.ACTGetExistAnswers(virtualTestId, studentid, bubblesheetId).Select(o => new ACTAlreadyAnsweredQuestion()
            {
                QuestionId = o.VirtualQuestionID,
                AnswerLetter = o.AnswerLetter,
                QuestionOrder = o.QuestionOrder,
                OrderSectionIndex = o.SectionIndex ?? 0,
                OrderSectionQuestionIndex = o.SectionQuestionIndex ?? 0
            }).ToList();
        }

        /// <summary>
        /// Get answer coverpage all student
        /// </summary>
        /// <param name="virtualTestId"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public List<ACTStudentStatus> GetListAnswerCoverPageAllStudent(int virtualTestId, string ticket)
        {
            return _dbContext.ACTGetListAnswerCoverPage(virtualTestId, ticket).Select(o => new ACTStudentStatus()
            {
                StudentId = o.studentid,
                PointsEarned = o.PointsEarned,
                TestResultId = o.testresultid,
                VirtualQuestionId = o.VirtualQuestionID,
                VirtualTestId = o.virtualtestid
            }).ToList();
        }

        public List<ACTStudentStatus> SATGetListAnswerCoverPageAllStudent(int virtualTestId, string ticket, int essaySectionID)
        {
            return _dbContext.SATGetListAnswerCoverPage(virtualTestId, ticket, essaySectionID).Select(o => new ACTStudentStatus()
            {
                StudentId = o.studentid,
                PointsEarned = o.PointsEarned,
                TestResultId = o.testresultid,
                VirtualQuestionId = o.VirtualQuestionID,
                VirtualTestId = o.virtualtestid
            }).ToList();
        }

        public List<ACTUnansweredQuestion> GetUnansweredQuestionError(int virtualTestId, int bubbleSheetId,
            int studentId)
        {
            var v = _dbContext.ACTUnansweredQuestionsErrorProc(bubbleSheetId, studentId, virtualTestId).ToList();
            if (v.Any())
            {
                return v.Select(x => new ACTUnansweredQuestion()
                {
                    QuestionId = x.VirtualQuestionID,
                    QuestionOrder = x.QuestionOrder,
                    PointsPossible = x.PointsPossible,
                    QTISchemaId = x.QTISchemaID,
                    AnswerIdentifiers = x.AnswerIdentifiers,
                    StudentId = x.StudentID,
                    BubbleSheetId = x.BubbleSheetID,
                    Ticket = x.Ticket,
                    XmlContent = x.XmlContent,
                    OrderSection = x.SectionOrder,
                    OrderSectionQuestion = x.SectionQuestionOrder,
                    SectionTitle = x.Title,
                    VirtualSectionId = x.VirtualSectionID,
                    OrderSectionIndex = x.SectionIndex,
                    OrderSectionQuestionIndex = x.SectionQuestionIndex
                }).ToList();
            }
            return new List<ACTUnansweredQuestion>();
        }

        public List<ACTAlreadyAnsweredQuestion> GetAllQuestions(int virtualtestId)
        {
            return _dbContext.ACTGetAllQuestion(virtualtestId).Select(o => new ACTAlreadyAnsweredQuestion()
            {
                QuestionId = o.VirtualQuestionID,
                AnswerLetter = o.AnswerLetter,
                QuestionOrder = o.QuestionOrder,
                OrderSectionIndex = o.SectionIndex ?? 0,
                OrderSectionQuestionIndex = o.SectionQuestionIndex ?? 0
            }).ToList();
        }
    }
}
