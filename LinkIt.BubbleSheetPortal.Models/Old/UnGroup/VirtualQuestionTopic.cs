namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualQuestionTopic
    {
        public int VirtualQuestionTopicId { get; set; }
        public int VirtualQuestionId { get; set; }
        public int VirtualTestId { get; set; }
        public int TopicId { get; set; }
        public string Name { get; set; }
    }
}
