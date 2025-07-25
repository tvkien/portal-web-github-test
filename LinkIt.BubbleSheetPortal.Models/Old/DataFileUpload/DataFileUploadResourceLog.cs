using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DataFileUpload
{
    public class DataFileUploadResourceLog
    {
        public int DataFileUploadResourceLogId { get; set; }
        public int DataFileUploadLogId { get; set; }
        public string ResourceFileName { get; set; }
        public int QtiSchemaID { get; set; }//the schema of the resource file type 
        public string OriginalContent { get; set; }
        public int QtiSchemaId { get; set; }
        public string XmlContent { get; set; }
        public string InteractionType { get; set; }
        public string Error { get; set; }
        public bool? IsValidQuestionResourceFile { get; set; }
        public int? QtiItemId { get; set; }
        public string ErrorDetail { get; set; }
        public string ProcessingStep { get; set; }
        public int? QTI3pItemId { get; set; }
    }
}
