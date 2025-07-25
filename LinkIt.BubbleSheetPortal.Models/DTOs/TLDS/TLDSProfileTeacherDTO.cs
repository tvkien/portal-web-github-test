using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.TLDS
{
    public class TLDSProfileTeacherDTO
    {
        public int TLDSProfileTeacherID { get; set; }

        public int TLDSUserMetaID { get; set; }

        public string EducatorName { get; set; }

        public int TLDSLevelQualificationID { get; set; }

        public string TLDSLevelQualificationName { get; set; }

        public string Position { get; set; }
    }
}
