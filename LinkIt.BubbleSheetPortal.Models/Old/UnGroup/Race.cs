using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class Race : ValidatableEntity<Race>
    {
        public int Id { get; set; }
        private string name = string.Empty;
        private string code = string.Empty;
        private string altCode = string.Empty;
        public int? DistrictID { get; set; }

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

        public string AltCode
        {
            get { return altCode; }
            set { altCode = value.ConvertNullToEmptyString(); }
        }
    }
}