using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class QTIItemTagControllerParameters
    {
        public TopicService TopicService { get; set; }
        public QTIItemTopicService QTIItemTopicService { get; set; }
        public LessonOneService LessonOneService { get; set; }
        public QTIItemLessonOneService QTIItemLessonOneService { get; set; }
        public LessonTwoService LessonTwoService { get; set; }
        public QTIItemLessonTwoService QTIItemLessonTwoService { get; set; }
        public QtiItemItemTagService QtiItemItemTagService { get; set; }
        public ItemTagService ItemTagService { get; set; }
        public QTIITemService QTIITemService { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }
        
    }
}