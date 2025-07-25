using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class UploadDataFileListViewModel
    {      
        public DateTime ImportedDate { get; set; }
        public string ZipFileName { get; set; }
        public string UploadUser { get; set; }
        public int DataFileUploadLogID { get; set; }
    }
}