using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualSectionQuestionQtiItem
    {
        public int VirtualSectionID { get; set; }
        public int VirtualQuestionID { get; set; }
        public int Order { get; set; }
        public int QTIItemID { get; set; }
        public string XmlContent { get; set; }
        public int QTIGroupID { get; set; }
        public int? VirtualSectionOrder { get; set; }
        public string VirtualSectionTitle { get; set; }
        public int VirtualSectionQuestionID { get; set; }
        public int QuestionOrder { get; set; }
        public int BaseVirtualQuestionId { get; set; }
        public int ItemNumber { get; set; }
        public int QTISchemaID { get; set; }
        public string AnswerIdentifiers { get; set; }
        public int? QuestionGroupID { get; set; }
        public bool IsRubricBasedQuestion { get; set; }
        public List<RubricQuestionCategoryItem> RubricQuestionCategoryies { get; set; }

        //Support Display
        public string DisplayOrder { get; set; }
        public int GroupOrder { get; set; }
    }
}
