using Envoc.Core.Shared.Extensions;
using  System;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualTestFile : ValidatableEntity<VirtualTestFile>
    {
        private string fileKey = string.Empty;
        private string fileUrl = string.Empty;
        private string fileName = string.Empty;

        public int VirtualTestFileId { get; set; }
        public int VirtualTestId { get; set; }
        public int? FileType { get; set; }
        public int? UploadByUserId { get; set; }
        public DateTime? UploadDate { get; set; }

        public string FileKey
        {
            get { return fileKey; }
            set { fileKey = value.ConvertNullToEmptyString(); }
        }

        public string FileUrl
        {
            get { return fileUrl; }
            set { fileUrl = value.ConvertNullToEmptyString(); }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value.ConvertNullToEmptyString(); }
        }
    }
}