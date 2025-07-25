using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class Program : ValidatableEntity<Program>
    {
        private string name = string.Empty;
        private string code = string.Empty;

        public int Id { get; set; }
        public int DistrictID { get; set; }
        public int? AccessLevelID { get; set; }

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
    }
}