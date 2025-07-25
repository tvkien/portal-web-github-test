using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ClassExtra
    {
        private string name = string.Empty;
        public int ClassId { get; set; }
        public int? UserId { get; set; }
        public int? DistrictTermId { get; set; }
        public int? SchoolId { get; set; }

        public int? ClassUserLOEId { get; set; }

        public int UserIdClassUser { get; set; }
        
        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
    }
}
