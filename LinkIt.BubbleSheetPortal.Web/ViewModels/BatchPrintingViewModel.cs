using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class BatchPrintingViewModel
    {
        public string StudentName { get; set; }
        public DateTime PrintDate { get; set; }
        public string ProcessingStatus { get; set; }
        public string DownloadPdfUrl { get; set; }
    }
}