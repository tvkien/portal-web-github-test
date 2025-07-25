using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.BubbleSheetGenerator
{
    [Serializable]
    public class Question
    {
        public int Index { get; set; }
        public List<string> Characters { get; set; }
        public bool IsTextEntry { get; set; }

        public bool IsPageBreak { get; set; }

        public bool IsOpenEndedQuestion { get; set; }
        public bool IsGhostQuestion { get; set; }

        public int? PointsPossible { get; set; }

        public string TagName { get; set; }

        public static Question Default
        {
            get { return new Question { Characters = new List<string> { "A", "B", "C", "D" } }; }
        }
        
        public int SectionID { get; set; }

        public int? QuestionGroupID { get; set; }

        public int? VirtualSectionID { get; set; }
    }
}
