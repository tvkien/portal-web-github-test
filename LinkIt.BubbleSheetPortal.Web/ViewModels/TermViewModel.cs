using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class TermViewModel
    {
        private string name = string.Empty;
        
        public int TermID { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        
        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
    }
}