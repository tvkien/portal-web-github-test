using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class PassageItem3pViewModel
    {
        public int QTI3pPassageID { get; set; }
        public string Source { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Subject { get; set; }        
        public string GradeName { get; set; }
        public string TextType { get; set; }
        public string TextSubType { get; set; }
        public string WordCount { get; set; }
        public string FleschKinkaidName { get; set; }
        public string PassageType { get; set; }
        public string PassageGenre { get; set; }       
        public string Spache { get; set; }
        public string DaleChall { get; set; }
        public string RMM { get; set; }
        public string Lexile { get; set; }
        public int ItemsMatchCount { get; set; }
        public int ItemsAllCount { get; set; }
        public string ItemsMatchXml { get; set; }
        public string ItemsAllXml { get; set; }
    }
}
