using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class S3PortalLink
    {
        private string _bucketName = string.Empty;
        private string _filePath = string.Empty;
        private string _PortalKey = string.Empty;

        public int S3PortalLinkId { get; set; }
        public int ServiceType { get; set; }
        public int DistrictId { get; set; }
        public DateTime? DateCreated { get; set; }

        public string BucketName
        {
            get { return _bucketName; }
            set { _bucketName = value.ConvertNullToEmptyString(); }
        }

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value.ConvertNullToEmptyString(); }
        }

        public string PortalKey
        {
            get { return _PortalKey; }
            set { _PortalKey = value.ConvertNullToEmptyString(); }
        }
    }
}
