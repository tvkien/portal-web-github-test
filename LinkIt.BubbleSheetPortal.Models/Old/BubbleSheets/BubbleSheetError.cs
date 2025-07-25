using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BubbleSheetError
    {
        private string fileName = string.Empty;
        private string message = string.Empty;
        private string uploadedBy = string.Empty;
        private string relatedImage = string.Empty;

        public int? BubbleSheetId { get; set; }
        public int BubbleSheetErrorId { get; set; }
        public bool IsCorrected { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ErrorCode { get; set; }
        public int RosterPosition { get; set; }
        public int? UserId { get; set; }
        public int DistrictId { get; set; }
        public int? VirtualTestId { get; set; }

        public string UploadedBy
        {
            get { return uploadedBy; }
            set { uploadedBy = value.ConvertNullToEmptyString(); }
        }

        public string RelatedImage
        {
            get { return relatedImage; }
            set { relatedImage = value.ConvertNullToEmptyString(); }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value.ConvertNullToEmptyString(); }
        }

        public string Message
        {
            get { return message; }
            set { message = value.ConvertNullToEmptyString(); }
        }
        
        public string SafeRelatedImage
        {
            get { return RelatedImage.Replace('/', '-'); }
        }
    }
}