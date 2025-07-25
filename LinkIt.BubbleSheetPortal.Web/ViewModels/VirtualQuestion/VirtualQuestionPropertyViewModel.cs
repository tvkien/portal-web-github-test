namespace LinkIt.BubbleSheetPortal.Web.ViewModels.VirtualTest
{
    public class VirtualQuestionPropertyViewModel
    {
        public int VirtualQuestionId { get; set; }
        public int QtiSchemaId { get; set; }
        public string XmlPossiblePoints { get; set; }
        public bool IsGhostQuestion { get; set; }
        public int? BaseQuestionId { get; set; }
        public int? ResponseProcessingTypeId { get; set; }
        public string QuestionLabel { get; set; }
        public string QuestionNumber { get; set; }
        public bool? IsCustomLevelNaming { get; set; } = false;
        public int PointPossible { get; set; } = 0;
        public int? VirtualSectionId { get; set; }
        public string ScoreName { get; set; }
        public string ItemNumberLabel { get; set; }
    }
}
