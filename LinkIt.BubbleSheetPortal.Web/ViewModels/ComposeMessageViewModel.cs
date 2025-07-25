using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ComposeMessageViewModel
    {

        public ComposeMessageViewModel()
        {
        }

        private int roleId = 0;
        public int RoleId
        {
            get { return roleId; }
            set { roleId = value; }
        }
        public int DistrictId { get; set; }
        
        public bool IsDistrictAdmin
        {
            get
            {
                return RoleId.Equals((int)Permissions.DistrictAdmin);
            }
        }
        public bool IsPublisher
        {
            get
            {
                return RoleId.Equals((int)Permissions.Publisher);
            }
        }
        public bool IsNetworkAdmin
        {
            get
            {
                return RoleId.Equals((int)Permissions.NetworkAdmin);
            }
        }
        public bool IsTeacher
        {
            get
            {
                return RoleId.Equals((int)Permissions.Teacher);
            }
        }
        public bool IsSchoolAdmin
        {
            get
            {
                return RoleId.Equals((int)Permissions.SchoolAdmin);
            }
        }

        public int CurrentUserId { get; set; }
        public string CurrentUserName { get; set; }
        public int CurrentSelectedDistrictId { get; set; }
        public int CurrentSelectedSchoolId { get; set; }
        public int CurrentSelectedTeacherId { get; set; }
        public int CurrentSelectedClassId { get; set; }
        public string From { get; set; }
        public string RoleName { get; set; }

    }
}