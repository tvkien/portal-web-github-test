using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AnswerViewer
    {        
        public int QTIOnlineTestSessionAnswerID { get; set; }
        public int AnswerID { get; set; }
        public int? QTIOnlineTestSessionID { get; set; }
        public int VirtualQuestionID { get; set; }
        public string AnswerChoice { get; set; }
        public bool? Answered { get; set; }
        public string AnswerText { get; set; }
        public string AnswerImage { get; set; }
        public int? PointsEarned { get; set; }
        public int QTISchemaID { get; set; }
        public string AnswerSubs { get; set; }
        public string XMLContent { get; set; }
        public string CorrectAnswer { get; set; }
        public int PointsPossible { get; set; }

    }
}
