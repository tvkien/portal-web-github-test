using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.DocumentManagement
{
    public class DocumentByIdsRequestDto
    {
        public IEnumerable<Guid?> DocumentIds { get; set; }
    }
}
