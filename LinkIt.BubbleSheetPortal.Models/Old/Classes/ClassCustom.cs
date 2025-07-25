using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ClassCustom
    {
        private string name = string.Empty;
        private string code = string.Empty;
        private string section = string.Empty;
        private string course = string.Empty;
        private string period = string.Empty;
        private string courseNumber = string.Empty;
        private string schoolName = string.Empty;
        private string districtTermName = string.Empty;
        private string teacherFirstName = string.Empty;
        private string teacherLastName = string.Empty;
        private string teacherUserName = string.Empty;

        public int Id { get; set; }
        public int? TermId { get; set; }
        public int? UserId { get; set; }
        public int? DistrictTermId { get; set; }
        public int? SchoolId { get; set; }
        public bool Locked { get; set; }
        public int? GradeId { get; set; }
        public int? SISID { get; set; }
        public int? DistrictId { get; set; }

        public string TeacherUserName
        {
            get { return teacherUserName; }
            set { teacherUserName = value.ConvertNullToEmptyString(); }
        }
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
        public string Period
        {
            get { return period; }
            set { period = value.ConvertNullToEmptyString(); }
        }

        public string Course
        {
            get { return course; }
            set { course = value.ConvertNullToEmptyString(); }
        }
        public string Section
        {
            get { return section; }
            set { section = value.ConvertNullToEmptyString(); }
        }
        public string Code
        {
            get { return code; }
            set { code = value.ConvertNullToEmptyString(); }
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
    }
}
