using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.DocumentManagement
{
    public class CreateDocumentDto
    {
        public Guid DocumentGuid { get; set; }
        public string DocumentName { get; set; }
        public string Extension { get; set; }
        public int DocumentTypeId { get; set; }
        public string DistrictId { get; set; }
        public int? Author { get; set; }
        public long FileSize { get; set; }
    }
}
