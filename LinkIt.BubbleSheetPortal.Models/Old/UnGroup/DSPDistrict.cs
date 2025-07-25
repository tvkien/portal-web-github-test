using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class DSPDistrict : ValidatableEntity<DSPDistrict>, IIdentifiable
    {
        public int Id { get; set; }
        public int OrganizationDistrictID { get; set; }
        public int MemberDistrictID { get; set; }
        public string Type { get; set; }

    }
}