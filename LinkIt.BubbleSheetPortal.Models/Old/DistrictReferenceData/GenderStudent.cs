using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.DistrictReferenceData
{
    public class GenderStudent: ValidatableEntity<GenderStudent>
    {
        private string name = string.Empty;

        public char Code { get; set; }
        public int DistrictID { get; set; }

        public string Name
        { 
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
    }
}