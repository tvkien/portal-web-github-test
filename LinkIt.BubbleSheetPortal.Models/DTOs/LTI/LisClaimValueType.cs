using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.LTI
{
    public class LisClaimValueType
    {
        [JsonProperty("course_offering_sourcedid")]
        public string CourseOfferingSourcedId { get; set; }

        [JsonProperty("course_section_sourcedid")]
        public string CourseSectionSourcedId { get; set; }

        [JsonProperty("person_sourcedid")]
        public string PersonSourcedId { get; set; }
    }
}
