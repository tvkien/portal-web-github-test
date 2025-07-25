using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ReportGroupStudent : ValidatableEntity<ReportGroupStudent>
    {
        public int ReportGroupStudentId { get; set; }
        public int ReportGroupId { get; set; }
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get { return LastName + ", " + FirstName; }
        }
    }
}