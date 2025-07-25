using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BatchPrintingQueue
    {
        public int BatchPrintingQueueID { get; set; }
        public int QTITestClassAssignmentID { get; set; }
        public string QTIOnlineTestSessionIDs { get; set; }
        public int VirtualTestID { get; set; }

        public bool ManuallyGradedOnly { get; set; }
        public string ManuallyGradedOnlyQuestionIds { get; set; }

        public bool TeacherFeedback { get; set; }
        public bool TheCoverPage { get; set; }
        public bool TheCorrectAnswer { get; set; }
        public bool Passages { get; set; }
        public bool GuidanceAndRationale { get; set; }
        public bool TheQuestionContent { get; set; }

        public int NumberOfColumn { get; set; }
        public bool ShowQuestionPrefix { get; set; }
        public bool ShowBorderAroundQuestions { get; set; }
        public bool ExcludeTestScore { get; set; }
        public bool IncorrectQuestionsOnly { get; set; }
        public bool IncludeAttachment { get; set; }
        public int StudentType { get; set; }
        public int QuestionType { get; set; }
        public string PrintQuestionIDs { get; set; }

        public int UserID { get; set; }
        public string UserName { get; set; }
        public string StudentName { get; set; }
        public int? DistrictID { get; set; }
        public int ProcessingStatus { get; set; }
        public Guid? DownloadPdfID { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
