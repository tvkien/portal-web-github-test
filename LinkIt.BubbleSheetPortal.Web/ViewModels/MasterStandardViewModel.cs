namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class MasterStandardViewModel
    {
        public MasterStandardViewModel()
        {
            ParentId = 0;
        }
        public string GUID { get; set; }
        public int MasterStandardID { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public int Children { get; set; }
        public string ParentGUID { get; set; }
        public int DescendantItemCount { get; set; }
        public int? ParentId { get; set; }
        public int RubricQuestionCategoryID { get; set; }
    }
}
