using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSFormSection2
    {
        public int TLDSFormSection2ID { get; set; }

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
    }
}
