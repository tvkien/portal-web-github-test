using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class Class : ValidatableEntity<Class>
    {
        private string name = string.Empty;
        private string courseNumber = string.Empty;
        private string _modifiedBy = string.Empty;

        public int Id { get; set; }
        public int? TermId { get; set; }
        public int? UserId { get; set; }
        public int? TeacherId { get; set; }
        public int? DistrictTermId { get; set; }
        public int? SchoolId { get; set; }
        public bool Locked { get; set; }
        public int? ClassType { get; set; }

        public int? DistrictId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedUser { get; set; }
        
        public string ModifiedBy
        {
            get { return _modifiedBy; }
            set { _modifiedBy = value.ConvertNullToEmptyString(); }
        }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public string CourseNumber
        {
            get { return courseNumber; }
            set { courseNumber = value.ConvertNullToEmptyString(); }
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
    }
}