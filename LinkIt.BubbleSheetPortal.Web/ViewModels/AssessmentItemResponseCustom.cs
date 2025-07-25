using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class AssessmentItemResponseCustom
    {
        public string DistrictCode { get; set; }
        public string Resultdate { get; set; }
        public string TestName { get; set; }
        public string QuestionOrder { get; set; }
        public string Number { get; set; }
        public string QTISchemaId { get; set; }
        public string PoinsPossible { get; set; }
        public string CorrectAnswer { get; set; }
        public string ITEM_RESPONSE_TYPE_LONG { get; set; }
    }
}