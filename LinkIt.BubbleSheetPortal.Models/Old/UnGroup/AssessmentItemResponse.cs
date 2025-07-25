using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AssessmentItemResponse
    {
        public string DistrictCode { get; set; }
        //public DateTime ResultDate { get; set; }
        public string TestName { get; set; }
        public int QuestionOrder { get; set; }
        public string Number { get; set; }
        //public int QTISchemaId { get; set; }
        public int PointsPossible { get; set; }
        public string CorrectAnswer { get; set; }
        //public int TestResultId { get; set; }
        public int Year { get; set; }
        public string QtiSchemaId { get; set; }
        public string ResponseLongType { get; set; }
    }
}
