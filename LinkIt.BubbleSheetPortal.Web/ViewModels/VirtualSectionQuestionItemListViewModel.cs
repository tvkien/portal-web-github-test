using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ComplexVirtualQuestionAnswerScoreItemListViewModel
    {
        public string ResponseIdentifier { get; set; }
        public string CorrectAnswer { get; set; }
        public string QtiItemScore { get; set; }
        public int TestScore { get; set; }
        public int QTISchemaID { get; set; }
        public int ResponseProcessingTypeId { get; set; }

    }
}
