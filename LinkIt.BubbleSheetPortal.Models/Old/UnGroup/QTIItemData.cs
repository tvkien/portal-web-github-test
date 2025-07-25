using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTIItemData
    {
        public int QTIItemID { get; set; }

        public int QuestionOrder { get; set; }

        public string Title { get; set; }

        //public int VirtualQuestionCount { get; set; }

        public string XmlContent { get; set; }

        public int QTISchemaID { get; set; }

        public string CorrectAnswer { get; set; }

        public string FilePath { get; set; }

        public string UrlPath { get; set; }

        public int? UserID { get; set; }

        public int? SourceID { get; set; }

        public int QTIGroupID { get; set; }

        public int? ParentID { get; set; }

        public string OldMasterCode { get; set; }

        public string AnswerIdentifiers { get; set; }

        public int PointsPossible { get; set; }

        public string ResponseProcessing { get; set; }

        public int? InteractionCount { get; set; }

        public DateTime? Updated { get; set; }

        public string ResponseIdentifier { get; set; }

        public int? ResponseProcessingTypeID { get; set; }
        public int NumberOfChoices { get; set; }
        public string Tests { get; set; }//xml
        public string StandardNumberList { get; set; }//xml
        public string TopicList { get; set; }//xml
        public string SkillList { get; set; }//xml
        public string OtherList { get; set; }//xml
        public string ItemTagList { get; set; }//xml
        public int? DataFileUploadTypeId { get; set; }
        public int? QtiItemIdSource { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedByUserID { get; set; }
        public int? RevertedFromQTIItemHistoryID { get; set; }
        public string Description { get; set; }
    }
}
