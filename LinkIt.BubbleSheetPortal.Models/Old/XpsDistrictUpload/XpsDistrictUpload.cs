using Envoc.Core.Shared.Model;
using System;

namespace LinkIt.BubbleSheetPortal.Models.Old.XpsDistrictUpload
{
    public class XpsDistrictUpload : ValidatableEntity<XpsDistrictUpload>
    {
        public int xpsDistrictUploadID { get; set; }
        public int DistrictID { get; set; }
        public bool Run { get; set; }
        public int? UploadTypeID { get; set; }
        public int? ClassNameType { get; set; }
        public TimeSpan ScheduledTime { get; set; }
    }
}
