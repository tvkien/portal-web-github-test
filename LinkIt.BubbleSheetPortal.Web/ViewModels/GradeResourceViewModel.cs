using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class GradeResourceViewModel
    {
        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
        public int Id { get; set; }
        public int Order { get; set; }

        
    }
}