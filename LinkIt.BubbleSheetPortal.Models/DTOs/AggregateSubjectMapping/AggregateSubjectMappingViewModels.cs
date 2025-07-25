using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models
{ 
    public class AggregateSubjectMappingViewModels
    {
        public int AggregateSubjectMappingID { get; set; }
        public string AggregateSubjectName { get; set; }
        public string Keywords { get; set; }
        public int? DistrictID { get; set; }
    }
}
