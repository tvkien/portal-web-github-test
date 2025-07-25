using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.StudentPreferenceDTOs
{
    public class TestForStudentPreferenceResponseDto
    {
        public int TotalRecord { get; set; }
        public List<TestForStudentPreferenceDto> Data { get; set; }
    }
}
