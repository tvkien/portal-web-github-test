using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ExtractTestResultParamCustom
    {
        public ExtractLocalCustom ExtractLocalCustom { get; set; }
        public string ListTemplates { get; set; }    
        public string ListIdsUncheck { get; set; }
        public int TimeZoneOffset { get; set; }
        public bool IsCheckAll { get; set; }
        public string ListId { get; set; }
        public string BaseHostUrl { get; set; }
        public string ExtractType { get; set; }
    }
}

