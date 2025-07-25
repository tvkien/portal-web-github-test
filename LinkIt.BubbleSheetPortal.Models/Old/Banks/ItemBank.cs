using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ItemBank : ValidatableEntity<ItemBank>
    {
        private string name = string.Empty;

        public int Id { get; set; }      
        public int UserID { get; set; }
        public int SchoolID { get; set; }
        public int DistrictID { get; set; }
        public int StateID { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
    }
}
