using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.Models.PrintTestOfStudent
{
    public class TOSVirtualQuestion
    {
        public List<string> PassageTexts { get; set; }
        public string XmlContent { get; set; }
        public int VirtualQuestionID { get; set; }
        public int QTISchemaID { get; set; }
        public int PointsPossible { get; set; }
        public int QuestionOrder { get; set; }

        public List<VirtualQuestionAnswerScoreDTO> VirtualQuestionAnswerScoresDTO { get; set; }
        public List<QTIItemAnswerScoreDTO> QTIItemAnswerScoresDTO { get; set; }
        public bool StartNewPassage { get; set; }
        public string ResponseRubric { get; set; }
    }
}