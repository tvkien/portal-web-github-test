namespace LinkIt.BubbleSheetPortal.Models.TestMaker.S3VirtualTest
{
    public class S3VirtualQuestion
    {
        public string xmlContent { get; set; }
        public int qtiItemID { get; set; }
        public int qtiSchemaID { get; set; }
        public int virtualQuestionID { get; set; }
        public int pointsPossible { get; set; }
        public int questionOrder { get; set; }
        public int? baseVirtualQuestionID { get; set; }
        public int? questionGroupID { get; set; }
        public string questionLabel { get; set; }
    }
}