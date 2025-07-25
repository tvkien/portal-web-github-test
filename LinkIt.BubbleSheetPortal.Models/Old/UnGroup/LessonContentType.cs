using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class LessonContentType : ValidatableEntity<LessonContentType>
    {
        public int Id { get; set; } //LessonContentTypeID
        public string Code { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string Name { get; set; }
        public int? SortOrder { get; set; }
    }
}