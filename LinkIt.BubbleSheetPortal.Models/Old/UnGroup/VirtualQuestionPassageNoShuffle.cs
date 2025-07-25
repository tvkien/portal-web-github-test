using System.Collections.Generic;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualQuestionPassageNoShuffle : ValidatableEntity<VirtualQuestionPassageNoShuffle>
    {
        public int VirtualQuestionPassageNoShuffleID { get; set; }
        public int VirtualQuestionID { get; set; }
        public int QTIRefObjectID { get; set; }
        public int QTI3pPassageID { get; set; }
        public int DataFileUploadPassageID { get; set; }
        public string PassageURL { get; set; }
    }
}