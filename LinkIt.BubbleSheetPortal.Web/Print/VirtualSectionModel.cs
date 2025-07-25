using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.Print
{
    public class VirtualSectionModel
    {
        public int SectionOrder { get; set; }
        public string SectionTitle { get; set; }
        public string SectionInstruction { get; set; }
        public List<VirtualQuestionModel> Items { get; set; }
    }
}
