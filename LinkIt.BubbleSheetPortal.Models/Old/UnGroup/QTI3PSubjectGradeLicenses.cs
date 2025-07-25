using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTI3PSubjectGradeLicenses
    {
        public int QTI3PSubjectGradeLicensesID { get; set; }
        public int QTI3pLicensesID { get; set; }
        public int DistrictID { get; set; }
        public string Subject { get; set; }
        public int? GradeID { get; set; }
        public int Status { get; set; }
    }
}