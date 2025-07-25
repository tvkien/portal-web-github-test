using System.Collections.Generic;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class AddClassViewModel : ValidatableEntity<AddClassViewModel>
    {
        private string name = string.Empty;
        
        public int? TeacherId { get; set; }
        public int DistrictTermId { get; set; }
        public int ClassTypeId { get; set; }
        public int SchoolId { get; set; }
        public bool IsUserTeacher { get; set; }
        public int CurrentUserRoleId { get; set; }
        public int CurrentUserId { get; set; }
        public int? DistrictId { get; set; }

        public IEnumerable<SelectListItem> DistrictTerms { get; set; }
        public IEnumerable<SelectListItem> ClassTypes { get; set; }
        public IEnumerable<SelectListItem> Schools { get; set; }

        public AddClassViewModel()
        {
            DistrictTerms = new List<SelectListItem>();
            ClassTypes = new List<SelectListItem>();
            Schools = new List<SelectListItem>();
            FromManageSchools = false;
        }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        private string course;
        public string Course
        {
            get { return course; }
            set { course = value.ConvertNullToEmptyString(); }
        }

        private string section;
        public string Section
        {
            get { return section; }
            set { section = value.ConvertNullToEmptyString(); }
        }

        private string courseNumber;
        public string CourseNumber
        {
            get { return courseNumber; }
            set { courseNumber = value.ConvertNullToEmptyString(); }
        }

        public bool FromManageSchools { get; set; }
        private string zipCode;
        public string ZipCode
        {
            get { return zipCode; }
            set { zipCode = value.ConvertNullToEmptyString(); }
        }

        private string schoolName;
        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value.ConvertNullToEmptyString(); }
        }

        public string StateCode { get; set; }
    }
}
