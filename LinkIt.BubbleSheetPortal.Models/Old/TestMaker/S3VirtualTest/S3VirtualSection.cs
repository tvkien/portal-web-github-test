using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.TestMaker.S3VirtualTest
{
    public class S3VirtualSection
    {
        public List<S3VirtualQuestion> items { get; set; }
        public int sectionID { get; set; }
        public int qtiGroupID { get; set; }
        public S3SectionData sectionData { get; set; }

        public List<S3QuestionGroup> questionGroups { get; set; }
        
    }
}