using System.Collections.Generic;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class AddNewStudentAssignClassFilter
    {
        public AddNewStudentAssignClassFilter()
        {
            IsPublisher = false;
            IsNetworkAdmin = false;
            IsTeacher = false;
            IsSchoolAdmin = false;
        }
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public bool IsPublisher { get; set; }
        public bool IsNetworkAdmin { get; set; }
        public bool IsDistrictAdmin { get; set; }
        public bool IsTeacher { get; set; }
        public bool IsSchoolAdmin { get; set; }
    }
}