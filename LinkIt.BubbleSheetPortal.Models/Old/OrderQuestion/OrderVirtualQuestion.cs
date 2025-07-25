using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.OrderQuestion
{
    public class OrderVirtualQuestion
    {
        public int VirtualQuestionID { get; set; }
        public int QuestionOrder { get; set; }
        public int? BaseVirtualQuestionID { get; set; }
        public bool IsBaseVirtualQuestion { get; set; }
        public bool IsGhostVirtualQuestion { get; set; }
        public OrderVirtualQuestion PreviousQuestion { get; set; }
        public OrderVirtualQuestion NextQuestion { get; set; }
        public OrderVirtualQuestion BaseVirtualQuestion { get; set; }
        public List<OrderVirtualQuestion> GhostVirtualQuestions { get; set; }
        public int? VirtualSectionID { get; set; }
    }
}
