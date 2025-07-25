using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.Models.PrintTestOfStudent
{
    public class TOSVirtualSection
    {
        public int SectionOrder { get; set; }
        public string SectionTitle { get; set; }
        public string SectionInstruction { get; set; }
        public List<TOSVirtualQuestion> Questions { get; set; }
        public int VirtualSectionID { get; set; }
    }
}