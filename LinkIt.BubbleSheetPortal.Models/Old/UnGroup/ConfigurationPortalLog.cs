using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ConfigurationPortalLog
    {
        public int Id { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Method { get; set; }
        public bool? IsLogInput { get; set; }
        public bool? IsLogOutput { get; set; }
        public bool? IsDisable { get; set; }
    }
}
