using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class DistrictState
    {
        private string districtNameCustom = string.Empty;
        public int DistrictId { get; set; }

        public string DistrictNameCustom
        {
            get { return districtNameCustom; }
            set { districtNameCustom = value.ConvertNullToEmptyString(); }
        }
    }
}
