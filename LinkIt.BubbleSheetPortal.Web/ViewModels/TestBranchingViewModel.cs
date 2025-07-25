using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class TestBranchingViewModel
    {
        public int VirtualTestId { get; set; }
        public int VirtualQuestionId { get; set; }
        public List<VirtualSectionViewModel> VirtualSectionList = new List<VirtualSectionViewModel>();
        public List<VirtualQuestionBranching> VirtualQuestionBranchingList = new List<VirtualQuestionBranching>();
    }
}