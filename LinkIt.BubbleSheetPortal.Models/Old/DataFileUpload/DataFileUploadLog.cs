using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DataFileUpload
{
    public class DataFileUploadLog
    {
        public int DataFileUploadLogId { get; set; }
        public int CurrentUserId { get; set; }
        public int DataFileUploadTypeId { get; set; }
        public string FileName { get; set; }
        public int QtiGroupId { get; set; }
        public string ExtractedFoler { get; set; }
        public string ItemSetPath { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }
        public int QTI3pSourceId { get; set; }
        public int? Status { get; set; }

    }
}
