using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.MessageQueue
{
    public class MessageQueueDto
    {
        public MessageQueueDto()
        {
            BinaryAttachments = new List<BinaryAttachment>();
        }

        public int DistrictId { get; set; }
        public string ServiceType { get; set; }
        public string Key { get; set; }
        public EmailData Data { get; set; }
        public string EmailTo { get; set; }
        public string EmailCC { get; set; }
        public string EmailBCC { get; set; }
        public string Attachments { get; set; }
        public IList<BinaryAttachment> BinaryAttachments { get; set; }
    }
    public class EmailData
    {
        public Dictionary<string, string> Subjects { get; set; }
        public Dictionary<string, string> Contents { get; set; }
    }
    public class BinaryAttachment
    {
        public string FileName { get; set; }
        public byte[] BinaryFile { get; set; }
    }
}
