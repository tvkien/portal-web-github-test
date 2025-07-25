using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualQuestionProperties
    {
        public int VirtualTestID { get; set; }
        public int VirtualQuestionID { get; set; }
        public string XmlContent { get; set; }
        public int QtiSchemaId { get; set; }
        public string QtiSchemaDes { get; set; }
        public int PointsPossible { get; set; }

        public string ItemBank { get; set; }
        public string ItemSet { get; set; }
        public string StandardNumberList { get; set; }
        public string TopicList { get; set; }
        public string SkillList { get; set; }
        public string OtherList { get; set; }
        public string ItemTagList { get; set; }
        public int QtiPointsPossible { get; set; }
        public int QTIGroupId { get; set; }
        public int BaseVirtualQuestionId { get; set; }

        public int QTIBankId { get; set; }
        public int ResponseProcessingTypeId { get; set; }
        public string QuestionLabel{ get; set; }
        public string QuestionNumber { get; set; }
        public bool IsRubricBasedQuestion { get; set; }
        public string ScoreName { get; set; }
    }
}
