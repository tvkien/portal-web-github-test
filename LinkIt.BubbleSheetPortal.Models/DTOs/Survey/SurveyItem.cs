using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.Survey
{
    public class SurveyItem
    {
        public int VirtualTestId { get; set; }
        public string Items { get; set; }
        public List<VirtualQuestionItemNumber> VirtualQuestionItemNumbers { get; set; }
    }
    public class VirtualQuestionItemNumber
    {
        public int VirtualQuestionId { get; set; }
        public string ItemNumber { get; set; }
    }
}
