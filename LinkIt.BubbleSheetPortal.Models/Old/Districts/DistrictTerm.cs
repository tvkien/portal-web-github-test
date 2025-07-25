using System;
using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class DistrictTerm : ValidatableEntity<DistrictTerm>
    {
        private string name = string.Empty;
        private string code = string.Empty;

        public int DistrictTermID { get; set; }
        public int? CreatedByUserID { get; set; }
        public int? UpdatedByUserID { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime? DateStart { set; get; }
        public DateTime? DateEnd { get; set; }
        public int? SISID { get; set; }
        public int DistrictID { get; set; }
        public bool? Active { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
        
        public string Code
        {
            get { return code; }
            set { code = value.ConvertNullToEmptyString(); }
        }

        public int? ModifiedUser { get; set; }
        public string ModifiedBy { get; set; }
    }
}