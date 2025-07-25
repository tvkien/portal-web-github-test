using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.DocumentManagement
{
    public class UpdatePathEtagsDto
    {
        public Guid DocumentGuid { get; set; }
        public string PartETags { get; set; }
    }
}
