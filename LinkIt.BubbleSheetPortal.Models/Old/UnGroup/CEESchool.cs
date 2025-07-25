using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class CEESchool : ValidatableEntity<CEESchool>
    {
        public int CEESchoolId { get; set; }
        public int? SchoolId  { get; set; }

        private string name = string.Empty;
        private string locationCode = string.Empty;

        public string StateCode { get; set; }
        
        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
        
        public string LocationCode
        {
            get { return locationCode; }
            set { locationCode = value.ConvertNullToEmptyString(); }
        }

    }
}