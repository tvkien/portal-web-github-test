using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload
{
    public class DataFileUploaderResult
    {
        public string Result { get; set; }//a message of error which will be presented to user
        public string Error { get; set; }//detail of the error
        public List<DataFileUploaderResource> Resources { get; set; }

        public DataFileUploaderResult()
        {
            Result = string.Empty;
            Error = string.Empty;
            Resources = new List<DataFileUploaderResource>();
        }
        public int DataFileUploadTypeId { get; set; }
        public int QTI3pSourceId { get; set; }
    }
}
