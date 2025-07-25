using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ClassUserLOE : IIdentifiable
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}