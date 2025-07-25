namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class EditTestPropertiesViewModel
    {
        public int VirtualTestId { get; set; }
        public string Name { get; set; }
        public string Instruction { get; set; }
        public int? TestScoreMethodID { get; set; }
        public int? NavigationMethodID { get; set; }
        public bool IsBranchingTest { get; set; }
        public bool IsTeacherLed { get; set; }
        public string XmlContent { get; set; }
        public bool IsSectionBranchingTest { get; set; }
        public bool IsCustomItemNaming { get; set; }
        public bool IsNumberQuestions { get; set; }
        public int CurrentVirtualTestSubTypeID { get; set; }
        public bool IsOverwriteTestResults { get; set; }

        public int? DatasetCategoryID { get; set; }
    }
}
