using LinkIt.BubbleSheetPortal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public class BBSClassViewSplitModel
    {
        public BBSClassViewSplitModel()
        {
            Students = new List<BBSStudent>();
            Questions = new List<BBSQuestion>();
        }
        public string Ticket { get; set; }
        public int ClassId { get; set; }
        public string TestName { get; set; }
        public List<BBSStudent> Students { get; set; }
        public List<BBSQuestion> Questions { get; set; }


        public class BBSQuestion
        {
            public int VirtualQuestionId { get; set; }
            public int QuestionOrder { get; set; }
            public int PointsPossible { get; set; }
            public int QTISchemaId { get; set; }
            public string AnswerIdentifiers { get; set; }
            public string CorrectAnswer { get; set; }
            public int MaxChoice { get; set; }
        }

        public class BBSStudent
        {
            public BBSStudent()
            {
                AnswerDatas = new List<BBSAnswer>();
            }
            public string Name { get; set; }
            public string Status { get; set; }
            public string Graded { get; set; }
            public string PointsEarned { get; set; }
            public int StudentId { get; set; }
            public int BubbleSheetId { get; set; }
            public bool IsChanged { get; set; } //answerletter is difference from original
            public int RosterPosition { get; set; }
            public string ArtifactFileName { get; set; }
            public BubbleSheetFileViewModel BubbleSheetFileViewModel { get; set; }
            public List<BBSAnswer> AnswerDatas { get; set; }
        }

        public class BBSAnswer
        {
            public int VirtualQuestionId { get; set; }
            public string Status { get; set; }
            public string AnswerLetter { get; set; }
            public bool WasAnswered { get; set; }
        }



        public static BBSClassViewSplitModel Split(BubbleSheetClassViewViewModel model)
        {
            var result = new BBSClassViewSplitModel() {
                Ticket = model.Ticket,
                ClassId = model.ClassId,
                TestName = model.TestName
            };

            if(model.BubbleSheetStudentDatas.Count > 0)
            {
                foreach (var item in model.BubbleSheetStudentDatas[0].BubbleSheetAnswers)
                {
                    result.Questions.Add(new BBSQuestion
                    {
                        VirtualQuestionId = item.VirtualQuestionId,
                        QuestionOrder = item.QuestionOrder,
                        QTISchemaId = item.QTISchemaId,
                        AnswerIdentifiers = item.AnswerIdentifiers,
                        CorrectAnswer = item.CorrectAnswer,
                        MaxChoice = item.MaxChoice,
                        PointsPossible = item.PointsPossible
                    });
                }

                foreach (var item in model.BubbleSheetStudentDatas)
                {
                    result.Students.Add(new BBSStudent
                    {
                        StudentId = item.StudentId,
                        BubbleSheetId = item.BubbleSheetId,
                        BubbleSheetFileViewModel = item.BubbleSheetFileViewModel,
                        IsChanged = item.IsChanged,
                        Name = item.Name,
                        PointsEarned = item.PointsEarned,
                        RosterPosition = item.RosterPosition,
                        ArtifactFileName = item.ArtifactFileName,
                        Status = item.Status,
                        Graded = item.Graded,
                        AnswerDatas = item.BubbleSheetAnswers.Select(m=> new BBSAnswer
                        {
                            VirtualQuestionId = m.VirtualQuestionId,
                            AnswerLetter = m.AnswerLetter,
                            Status = m.Status,
                           WasAnswered = m.WasAnswered
                        }).ToList()
                    });
                }
            }

            return result;
        }
    }

    
}
