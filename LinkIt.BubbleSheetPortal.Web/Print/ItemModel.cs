using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Print
{
    public class ItemModel
    {
        public int QuestionOrder { get; set; }

        public string Title { get; set; }

        public string XmlContent { get; set; }
        public string UrlPath { get; set; }

        public List<string> PassageTexts { get; set; }
    }
}