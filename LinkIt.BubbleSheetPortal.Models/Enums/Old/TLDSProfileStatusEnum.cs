using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.Enum
{
    public enum TLDSProfileStatusEnum
    {
        [Description("Draft")]
        Draft = 10,

        [Description("Created/Unsubmitted")]
        CreatedUnsubmitted = 20,

        [Description("Submitted to School")]
        SubmittedToSchool = 30,

        [Description("Associated with Student")]
        AssociatedWithStudent = 40,

        [Description("Returned By School")]
        ReturnedBySchool = 50,

        [Description("Uploaded By School")]
        UploadedBySchool = 70,

        [Description("Recalled")]
        Recalled = 60
    }


}
