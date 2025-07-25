using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class Configuration
    {
        private string _Name;
        private string _value;
        public int Type { get; set; }

        public string Name
        {
            get { return _Name; }
            set { _Name = value.ConvertNullToEmptyString(); }
        }
        public string Value
        {
            get { return _value; }
            set { _value = value.ConvertNullToEmptyString(); }
        }
    }
}
