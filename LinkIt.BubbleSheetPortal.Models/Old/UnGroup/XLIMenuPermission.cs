using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class XLIMenuPermission
    {
        private string iconCode;
        private string subItemCode;

        public bool IconRestrict { get; set; }
        public bool SubItemRestrict { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public bool Expires { get; set; }
        public int DistrictId { get; set; }
        public DateTime? ModuleEndDate { get; set; }
        public DateTime? ModuleStartDate { get; set; }
        public bool ModuleExpires { get; set; }

        public string IconCode
        {
            get { return iconCode; }
            set { iconCode = value.ConvertNullToEmptyString(); }
        }
        public string SubItemCode
        {
            get { return subItemCode; }
            set { subItemCode = value.ConvertNullToEmptyString(); }
        }
    }
}
