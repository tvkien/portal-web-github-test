using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSFormSection3
    {
        public int TLDSFormSection3ID { get; set; }

        public Guid TLDSProfileLinkID { get; set; }

        public string GuardianName { get; set; }

        public string Relationship { get; set; }

        public string PreferredLanguage { get; set; }

        public System.Nullable<bool> IsAborigial { get; set; }

        public System.Nullable<bool> HaveSiblingInSchool { get; set; }

        public string NameAndGradeOfSibling { get; set; }

        public string Wishes { get; set; }

        public string InformationSchool { get; set; }

        public string HelpInformation { get; set; }

        public string Interested { get; set; }

        public string ConditionImprovement { get; set; }

        public string OtherInformation { get; set; }

        public System.Nullable<bool> IsSubmitted { get; set; }
    }
}
