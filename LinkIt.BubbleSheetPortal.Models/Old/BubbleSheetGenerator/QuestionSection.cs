using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.BubbleSheetGenerator
{
    [Serializable]
    public class QuestionSection
    {
        public int Index { get; set; }
        public string SectionName { get; set; }
        public bool IsPageBreak { get; set; }
        public List<Question> ListQuestions { get; set; }
    }
}
