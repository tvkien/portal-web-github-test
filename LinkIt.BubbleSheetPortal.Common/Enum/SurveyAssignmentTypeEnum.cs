using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Common.Enum
{
    public enum SurveyAssignmentTypeEnum
    {
        [Description("Public Anonymous")]
        PublicAnonymous = 1,

        [Description("Public Individualized")]
        PublicIndividualized = 2,

        [Description("Private Anonymous")]
        PrivateAnonymous = 3,

        [Description("Private Individualized")]
        PrivateIndividualized = 4,
        Preview = 5
    }
}
