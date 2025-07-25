using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.BusinessObjects;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class ShowQtiItemControllerParameters
    {
        public QTIITemService QTIITemServices { get; set; }
        public VirtualQuestionService VirtualQuestionServices { get; set; }
        public DistrictDecodeService DistrictDecodeService { get; set; }
        public RestrictionBO RestrictionBO { get; set; }
        public VirtualTestService VirtualTestService { get; set; }
        public QuestionGroupService QuestionGroupService { get; set; }
    }
}