using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class DistrictSettings
    {
        public int DistrictSettingId { get; set; }
        public int DistrictId { get; set; }
        private string defaultARSettings { get; set; }
        private string testSettings { get; set; }

        public string DefaultARSettings
        {
            get { return defaultARSettings; }
            set { defaultARSettings = value.ConvertNullToEmptyString(); }
        }

        public string TestSettings
        {
            get { return testSettings; }
            set { testSettings = value.ConvertNullToEmptyString(); }
        }
    }
}
