using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.QuestionGroup;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class VirtualSectionViewModel
    {
        public int VirtualSectionId { get; set; }
        public int VirtualTestId { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        
        public List<VirtualSectionQuestionQtiItem> SectionQuestionQtiItemList = new List<VirtualSectionQuestionQtiItem>();
        public List<QuestionGroupData> QuestionGroupList = new List<QuestionGroupData>();
    }
}