using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class XliArea
    {
        private string name = string.Empty;
        private string code = string.Empty;
        private string displayTooltip = string.Empty;
        private string displayName = string.Empty;

        public int XliAreaId { get; set; }
        public bool Restrict { get; set; }
        public int AreaOrder { get; set; }

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value.ConvertNullToEmptyString(); }
        }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public string Code
        {
            get { return code; }
            set { code = value.ConvertNullToEmptyString(); }
        }

        public string DisplayTooltip
        {
            get { return displayTooltip; }
            set { displayTooltip = value.ConvertNullToEmptyString(); }
        }
    }
}
