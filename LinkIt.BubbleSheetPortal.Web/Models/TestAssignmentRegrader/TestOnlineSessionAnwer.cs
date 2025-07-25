using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Web.Models.TestAssignmentRegrader
{
    public class TestOnlineSessionAnwer
    {
        public TestOnlineSessionAnwer()
        {
            PostAnswerLogs = new List<PostAnswerLogModel>();
            AnswerAttachments = new List<AnswerAttachment>();
        }

        public int QTIOnlineTestSessionAnswerID { get; set; }
        public int AnswerID { get; set; }
        public int QTIOnlineTestSessionID { get; set; }
        public int VirtualQuestionID { get; set; }
        public string AnswerChoice { get; set; }
        public bool Answered { get; set; }
        public string AnswerText { get; set; }
        public string AnswerImage { get; set; }
        public string HighlightQuestion { get; set; }
        public string HighlightPassage { get; set; }
        public string AnswerTemp { get; set; }
        public int? PointsEarned { get; set; }
        public int? ResponseProcessingTypeID { get; set; }
        public int QTISchemaID { get; set; }
        public bool IsRequiredAttachment { get; set; }
        public bool IsReviewed
        {
            get
            {
                if (TestOnlineSessionAnswerSubs != null)
                {
                    if (TestOnlineSessionAnswerSubs.Any(testOnlineSessionAnswerSub => !testOnlineSessionAnswerSub.PointsEarned.HasValue && testOnlineSessionAnswerSub.ResponseProcessingTypeID != 3))
                    {
                        return false;
                    }
                }

                return PointsEarned.HasValue || ResponseProcessingTypeID == 3;
            }
        }

        public string ResponseIdentifier { get; set; }
        public bool? Overridden { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public List<TestOnlineSessionAnswerSub> TestOnlineSessionAnswerSubs { get; set; }
        public int? ItemFeedbackID { get; set; }
        public int? ItemAnswerID { get; set; }
        public string Feedback { get; set; }
        public string UserUpdatedFeedback { get; set; }
        public string DateUpdatedFeedback { get; set; }
        public int AnswerOrder { get; set; }

        //public List<TestOnlineSessionAnswerTimeTrack> TestOnlineSessionAnswerTimeTracks { get; set; }
        public int TimesVisited { get; set; }

        public int TimeSpent { get; set; }

        public string DrawingContent { get; set; }

        public List<PostAnswerLogModel> PostAnswerLogs { get; set; }

        public List<BubbleSheetPortal.Models.DTOs.RubricQuestionCategoryDto> RubricQuestionCategories { get; set; }
        public List<AnswerAttachment> AnswerAttachments { get; set; }
        public AnswerAttachment TeacherFeebackAttachment { get; set; }
    }

    public class AnswerAttachment
    {
        public Guid? DocumentGuid { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public byte[] FileContent { get; set; }
    }
}
