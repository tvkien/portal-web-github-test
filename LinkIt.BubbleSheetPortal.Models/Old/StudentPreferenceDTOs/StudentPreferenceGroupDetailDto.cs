using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.StudentPreferenceDTOs
{
    public class StudentPreferenceGroupDetailDto
    {
        public string GroupName { get; set; }
        public List<StudentPreferenceDetailDto> List { get; set; }
    }
}
