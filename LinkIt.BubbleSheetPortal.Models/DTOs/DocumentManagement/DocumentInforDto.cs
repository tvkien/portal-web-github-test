using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.DocumentManagement
{
    public class DocumentInforDto
    {
        public Guid DocumentGuid { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }

        public bool CanBeViewedOnline { get; set; }
    }
}
