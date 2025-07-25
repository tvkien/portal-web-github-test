using Envoc.Core.Shared.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TLDSDigital
{
    public class TldsFormSection2ViewModel
    {
        public int? TLDSFormSection2ID { get; set; }
        public string FullName { get; set; }
        public Guid TLDSProfileLinkID { get; set; }

        public string GuardianName { get; set; }

        public string Relationship { get; set; }

        public string Favourite { get; set; }

        public string Strengths { get; set; }

        public string Weaknesses { get; set; }

        public string Interested { get; set; }

        public string Expected { get; set; }

        public string Drawing { get; set; }

        public bool IsSubmitted { get; set; }

        public string DrawingWithAbsolutePath { get; set; }
        public int RotatePhoto { get; set; }
    }
}
