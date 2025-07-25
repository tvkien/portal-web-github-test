using System.Diagnostics;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AuthorGroupList
    {
        public int AuthorGroupId { get; set; }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public int Level { get; set; }
        public int? SchoolId { get; set; }
        public int? DistrictId { get; set; }
        public int? StateId { get; set; }
        public int? UserId { get; set; }

        private string districtName;

        public string DistrictName
        {
            get { return districtName; }
            set { districtName = value.ConvertNullToEmptyString(); }
        }

        private string schoolName;

        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value.ConvertNullToEmptyString(); }
        }

        private string userNameList;

        public string UserNameList
        {
            get { return userNameList; }
            set { userNameList = value.ConvertNullToEmptyString(); }
        }
    }
}