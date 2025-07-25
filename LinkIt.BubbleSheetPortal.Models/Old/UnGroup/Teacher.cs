using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class Teacher : ValidatableEntity<Teacher>
    {
        private string name = string.Empty;
        
        public int Id { get; set; }
        public int DistrictId { get; set; }
        public int? UserId { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
    }
}
