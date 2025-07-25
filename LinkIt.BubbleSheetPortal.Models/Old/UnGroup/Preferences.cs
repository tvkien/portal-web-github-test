using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    [Serializable]
    public class Preferences 
    {
        public int PreferenceId { get; set; }
        public int Id { get; set; }

        private string _level = string.Empty;
        private string _label = string.Empty;
        private string _value = string.Empty;

        public string Level
        {
            get { return _level; }
            set { _level = value.ConvertNullToEmptyString(); }
        }

        public string Label
        {
            get { return _label; }
            set { _label = value.ConvertNullToEmptyString(); }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value.ConvertNullToEmptyString(); }
        }

        public bool? BankLocked { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
