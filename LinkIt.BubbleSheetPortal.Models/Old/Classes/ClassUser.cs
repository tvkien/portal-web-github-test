using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ClassUser : ValidatableEntity<ClassUser>, IIdentifiable
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int UserId { get; set; }
        public int ClassUserLOEId { get; set; }

        public ClassUserLOE ClassUserLOE { get; set; }
        public User User { get; set; }
    }
}