using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.DistrictReferenceData
{
    public class GradeDistrict : ValidatableEntity<GradeDistrict>
    {
        private string name = string.Empty;

        public int GradeID { get; set; }
        public int Order { get; set; }
        public int DistrictID { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
    }
}