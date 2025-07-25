using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.TestMaker.S3VirtualTest
{
    public class S3VirtualTest
    {
        public int virtualTestID { get; set; }
        public bool isCustomItemNaming { get; set; }
        public S3TestData testData { get; set; }
        public List<S3VirtualSection> sections { get; set; }
    }
}