using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ClassCustomForGroup
    {
        private string name = string.Empty;
        private string schoolName = string.Empty;
        private string districtTermName = string.Empty;
        private string teacherFirstName = string.Empty;
        private string teacherLastName = string.Empty;

        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? DistrictTermId { get; set; }
        public int? SchoolId { get; set; }

        public string TeacherLastName
        {
            get { return teacherLastName; }
            set { teacherLastName = value.ConvertNullToEmptyString(); }
        }

        public string TeacherFirstName
        {
            get { return teacherFirstName; }
            set { teacherFirstName = value.ConvertNullToEmptyString(); }
        }

        public string DistrictTermName
        {
            get { return districtTermName; }
            set { districtTermName = value.ConvertNullToEmptyString(); }
        }

        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value.ConvertNullToEmptyString(); }
        }
        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
    }
}
