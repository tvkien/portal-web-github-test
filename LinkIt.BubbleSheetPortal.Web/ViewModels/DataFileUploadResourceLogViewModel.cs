using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class DataFileUploadResourceLogViewModel
    {        
        public int QuestionOrder { get; set; }
        public string ResourceFileName { get; set; }
        public string Content { get; set; }
        public int QTI3pItemId { get; set; }
    }
}