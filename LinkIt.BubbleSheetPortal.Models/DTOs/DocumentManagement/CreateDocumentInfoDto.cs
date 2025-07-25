using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.DocumentManagement
{
    public class CreateDocumentInfoDto
    {
        public Guid DocumentGuid { get; set; } = Guid.NewGuid();
        public int DocumentTypeId { get; set; }
        public string DocumentName { get; set; }
        public string Extension { get; set; }
        public int DistrictId { get; set; }
        public int Author { get; set; }
    }
}
