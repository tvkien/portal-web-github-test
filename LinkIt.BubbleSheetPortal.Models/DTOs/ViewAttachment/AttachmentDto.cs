namespace LinkIt.BubbleSheetPortal.Models.DTOs.ViewAttachment
{
    public class AttachmentDto
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string ViewDsServerLink { get; set; }

        public static AttachmentDto CreateFileDownload(string fileName, byte[] fileContent)
        {
            return new AttachmentDto
            {
                FileName = fileName,
                FileContent = fileContent,
            };
        }

        public static AttachmentDto CreateFileViewOnDSSever(string fileName, string viewDsServerLink)
        {
            return new AttachmentDto
            {
                FileName = fileName,
                ViewDsServerLink = viewDsServerLink,
            };
        }
    }
}
