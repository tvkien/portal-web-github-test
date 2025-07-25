using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace S3Library
{
    public class S3Result
    {
        public bool IsSuccess { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ReturnValue { get; set; }
    }

    public class S3DownloadResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public byte[] ReturnStream { get; set; }
    }

    public enum ServiceTypeEnum
    {
        [Description("StaffLoader")]
        StaffLoader = 1,

        [Description("RosterLoader")]
        RosterLoader = 2,

        [Description("TestDataUpdate")]
        TestDataUpdate = 3,

        [Description("StudentProgramLoader")]
        StudentProgramLoader = 4,

        [Description("StudentStatusUpdate")]
        StudentStatusUpdate = 5,

        [Description("UserStatusUpdate")]
        UserStatusUpdate = 6,
    }

    public class S3PortalLinkDTO {
        public int S3PortalLinkID { get; set; }
        public int ServiceType { get; set; }
        public int? DistrictID { get; set; }
        public string BucketName { get; set; }
        public string FilePath { get; set; }
        public DateTime DateCreated { get; set; }
        public string PortalKey { get; set; }
    }

    public class ConfigurationDTO
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int? Type { get; set; }
    }
}
