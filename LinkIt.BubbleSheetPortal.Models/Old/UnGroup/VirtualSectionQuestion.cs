using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualSectionQuestion
    {
        public int VirtualSectionQuestionId { get; set; }
        public int VirtualSectionId { get; set; }
        public int VirtualQuestionId { get; set; }
        public int Order { get; set; }
        public int QuestionOrder { get; set; }
        public int VirtualTestId { get; set; }
        public int PointsPossible { get; set; }
        public int QtiItemId { get; set; }
        public string XmlContent { get; set; }
    }
}