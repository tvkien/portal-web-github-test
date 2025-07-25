using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DataFileUpload
{
    public class DataFileUploadPassage
    {
        public int DataFileUploadPassageID { get; set; }
        public int DataFileUploadTypeID { get; set; }
        public string FileName { get; set; }
        public string Fullpath { get; set; }
        public int? DataFileUploadLogID { get; set; }
        public string PassageTitle { get; set; }
    }
}
