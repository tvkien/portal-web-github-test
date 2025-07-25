using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class DistrictConfiguration
    {
        private string _name;
        private string _value;

        public string Name
        {
            get { return _name; }
            set { _name = value.ConvertNullToEmptyString(); }
        }
        public string Value
        {
            get { return _value; }
            set { _value = value.ConvertNullToEmptyString(); }
        }

        public int DistrictId { get; set; }
    }
}
