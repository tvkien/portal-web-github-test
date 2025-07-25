using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class SGOLoadCandidateClassForReplacementViewModel
    {
        public List<SGOGetCandidateClass> SGOCandidateClasses { get; set; }
        public List<ListItemExtra> SGOStudents { get; set; }
    }
}