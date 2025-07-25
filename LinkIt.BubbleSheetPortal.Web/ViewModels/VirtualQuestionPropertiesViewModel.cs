using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class VirtualQuestionPropertiesViewModel
    {
        public VirtualQuestionPropertiesViewModel()
        {
            VirtualQuestionId = 0;
            QtiItemId = 0;
            ItemType = string.Empty;
            Passages = string.Empty;
            Standards = string.Empty;
            Topics = string.Empty;
            ItemBank = string.Empty;
            ItemSet = string.Empty;
            EditPointPossibleDirectly = false;
            CanEditQTIITem = false;
            IsComplexItem = false;
            IsSingleCardinality = false;
            IsCustomItemNaming = false;
            ScoringMethod = string.Empty;
            ScoreName = string.Empty;
            HasTestResult = false;
        }

        public int VirtualTestId { get; set; }
        public int VirtualQuestionId { get; set; }
        public int QtiItemId { get; set; }
        public string XmlContent { get; set; }
        public string ItemType { get; set; }
        public int PointPossible { get; set; }

        public string Passages { get; set; }
        public string Standards { get; set; }
        public string Topics { get; set; }
        public string ItemBank { get; set; }
        public string ItemSet { get; set; }
        public string Skills { get; set; }
        public string Others { get; set; }
        public string ItemTags { get; set; }

        public int OringinalPointPossible { get; set; }

        public int QtiSchemaId { get; set; }

        public bool CanEditQTIITem { get; set; }
        public bool EditPointPossibleDirectly { get; set; }
        public bool IsComplexItem { get; set; }
        public List<ComplexVirtualQuestionAnswerScoreItemListViewModel> ComplexVirtualQuestionAnswerScore = new List<ComplexVirtualQuestionAnswerScoreItemListViewModel>();
        public int BaseVirtualQuestionId { get; set; }
        public bool HasChildQuestion { get; set; }
        public bool IsSingleCardinality { get; set; }
        public int ResponseProcessingTypeId { get; set; }
        public bool IsCustomItemNaming { get; set; }
        public string QuestionLabel { get; set; }
        public string QuestionNumber { get; set; }
        public bool IsRubricBasedQuestion { get; set; }
        public string ScoringMethod { get; set; }
        public int? NavigationMethodID { get; set; }
        public int VirtualSectionId { get; set; }
        public string ScoreName { get; set; }
        public bool IsSurvey { get; set; }
        public bool HasTestResult { get; set; }

        public AttachmentSettingViewModel AttachmentSetting { get; set; }
        public bool HasRetakeRequest { get; set; }
    }

    public class AttachmentSettingViewModel
    {
        public string SumaryValue
        {
            get
            {
                if (this.AllowStudentAttachment)
                {
                    if (this.RequireAttachment)
                    {
                        return "Required";
                    }
                    else
                    {
                        return "Optional";
                    }

                }

                return "Not Allowed";
            }
        }
        public bool AllowStudentAttachment { get; set; }
        public bool RequireAttachment { get; set; }
    }

}
