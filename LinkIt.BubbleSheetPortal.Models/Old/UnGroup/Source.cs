using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class Source : ValidatableEntity<Source>
    {
        public int SourceId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string ThumbnailPath { get; set; }
        public string Webpage { get; set; }
        public int? ReferenceId { get; set; }
        public int? SourceTypeId { get; set; }
        public int? AllDistrict { get; set; }
    }
}