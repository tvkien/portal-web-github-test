using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.DistrictReferenceData
{
    public class RequestParameterDistrict : ValidatableEntity<RequestParameterDistrict>
    {
        private string value = string.Empty;        
        private string name = string.Empty;

        public int RequestParameterID { get; set; }
        public int RequestID { get; set; }
        public int? DistrictID { get; set; }
        
        public string Value
        {
            get { return value; }
            set { this.value = value.ConvertNullToEmptyString(); }
        }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
    }
}