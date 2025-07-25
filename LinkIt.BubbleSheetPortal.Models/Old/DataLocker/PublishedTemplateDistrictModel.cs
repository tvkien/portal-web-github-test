using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DataLocker
{
    public class PublishTemplateDistrictModel
    {
        public int VirtualTestCustomScoreDistrictShareID { get; set; }
        public int VirtualTestCustomScoreID { get; set; }
        public int DistrictId { get; set; }
        public int CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string DistrictName { get; set; }
    }
}
