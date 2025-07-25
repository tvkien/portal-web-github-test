using System;

namespace LinkIt.BubbleSheetPortal.Models.PDFGenerator
{
    public class PdfGeneratorModel
    {
        public string Html { get; set; }
        public string FileName { get; set; }
        public string Folder { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool IsArtifactFile { get; set; }
        public Guid DocumentGuid { get; set; }
    }
}
