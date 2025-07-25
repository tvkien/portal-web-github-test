using Envoc.Core.Shared.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TLDSDigital
{
    public class TldsFormSection3ViewModel
    {
        public int? TLDSFormSection3ID { get; set; }

        public Guid TLDSProfileLinkID { get; set; }

        [Required(ErrorMessage = "Please enter Guardian name.")]
        public string GuardianName { get; set; }

        [Required(ErrorMessage = "Please enter {0}.")]
        public string Relationship { get; set; }

        public string PreferredLanguage { get; set; }

        public Nullable<bool> IsAborigial { get; set; }

        public Nullable<bool> HaveSiblingInSchool { get; set; }

        public string NameAndGradeOfSibling { get; set; }

        public string Wishes { get; set; }

        public string InformationSchool { get; set; }

        public string HelpInformation { get; set; }

        public string Interested { get; set; }

        public string ConditionImprovement { get; set; }

        public string OtherInformation { get; set; }

        public bool? IsSubmitted { get; set; }
    }
}
