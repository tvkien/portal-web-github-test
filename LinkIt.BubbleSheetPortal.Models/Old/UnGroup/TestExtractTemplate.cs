using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestExtractTemplate
    {
        private string name = string.Empty;
        

        public int ID { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreateDate { get; set; }
        
        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
    }
}
