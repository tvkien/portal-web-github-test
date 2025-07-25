using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class ViewableNavigatorReportAttributesDTO
    {
        public string Type { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public double? Ord { get; set; }
    }
}
