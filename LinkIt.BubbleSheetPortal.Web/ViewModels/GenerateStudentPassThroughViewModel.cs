using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class GenerateStudentPassThroughViewModel : StudentPassThroughViewModel
    {
        public string AccessKey { get; set; }
        public string PrivateKey { get; set; }
        public string RawData { get; set; }
    }
}