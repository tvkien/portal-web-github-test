using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class Role : ValidatableEntity<Class>
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Display { get; set; }
    }
}
