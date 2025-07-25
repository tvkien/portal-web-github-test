using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class SGOInstanceViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
        public string Teacher { get; set; }
        public string School { get; set; }
        public string GradeIDs { get; set; }
        public string Course { get; set; }
        public int TotalStudent { get; set; }

        public string StartDate { get; set; }
        public string CreatedDate { get; set; }
        public string EffectiveStatus { get; set; }
        public string EffectiveStatusDate { get; set; }
        public string ApproverName { get; set; }
        public bool IsArchived { get; set; }
        public bool IsOwner { get; set; }
        public bool NotEdit { get; set; }
    }
}