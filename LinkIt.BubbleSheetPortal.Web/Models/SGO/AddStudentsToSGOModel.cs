using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.Models.SGO
{
    public class AddStudentsToSGOModel
    {
        public int? SGOID { get; set; }
        public List<string> StudentIDs { get; set; } 
    }
}