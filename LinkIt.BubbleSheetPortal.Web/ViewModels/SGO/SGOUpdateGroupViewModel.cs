using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class SGOUpdateGroupViewModel
    {
        public int SGOID { get; set; }
        public string StrGroups { get; set; }
        public IEnumerable<UploadStudentInGroupViewModel> StudentInGroups { get; set; }
    }
}