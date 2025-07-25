using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualTestProperty
    {
        public int VirtualTestId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Author { get; set; }
        public int AuthorUserId { get; set; }
        public int TotalQuestion { get; set; }
        public int TotalTestResult { get; set; }
        public DateTime MinResultDate { get; set; }
        public DateTime MaxResultDate { get; set; }
        public string Instruction { get; set; }

    }
}
