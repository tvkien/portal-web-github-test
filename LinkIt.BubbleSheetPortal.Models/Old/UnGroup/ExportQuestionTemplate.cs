using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ExportQuestionTemplate
    {

        public string Name { get; set; }

        public int PointsPossible { get; set; }

        public int QuestionOrder { get; set; }

        public string Standard { get; set; }

        public string Topic { get; set; }

        public string Skill { get; set; }

        public string Other { get; set; }
    }
}
