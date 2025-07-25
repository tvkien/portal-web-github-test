using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class PrintingGroupData
    {
        private string groupName = string.Empty;
        private string className = string.Empty;
        private string schoolName = string.Empty;
        private string districtTermName = string.Empty;
        private string teacherName = string.Empty;

        public int CreatedUserID { get; set; }
        public int ClassID { get; set; }
        public int GroupID { get; set; }
        public int UserID { get; set; }

        public string GroupName
        {
            get { return groupName; }
            set { groupName = value.ConvertNullToEmptyString(); }
        }
        public string ClassName
        {
            get { return className; }
            set { className = value.ConvertNullToEmptyString(); }
        }
        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value.ConvertNullToEmptyString(); }
        }
        public string DistrictTermName
        {
            get { return districtTermName; }
            set { districtTermName = value.ConvertNullToEmptyString(); }
        }
        public string TeacherName
        {
            get { return teacherName; }
            set { teacherName = value.ConvertNullToEmptyString(); }
        }
    }
}