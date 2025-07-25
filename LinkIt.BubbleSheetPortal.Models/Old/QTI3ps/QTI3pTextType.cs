using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTI3pTextType
    {
        private string name = string.Empty;
        public int TextTypeID { get; set; }
        public int TypeID { get; set; }
        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
    }
}
