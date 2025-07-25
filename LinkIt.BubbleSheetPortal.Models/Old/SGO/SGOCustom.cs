using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOCustom
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Teacher { get; set; }
        public string School { get; set; }
        public string Grade { get; set; }
        public string Course { get; set; }
        public int? TotalStudent { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int EffectiveStatus { get; set; }
        public DateTime? EffectiveStatusDate { get; set; }
    }
}
