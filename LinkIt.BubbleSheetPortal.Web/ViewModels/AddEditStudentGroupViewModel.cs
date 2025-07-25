using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class AddEditStudentGroupViewModel
    {
       
        private int roleId = 0;
        public AddEditStudentGroupViewModel()
        {
        }

        public int GroupId { get; set; }
        public int DistrictId { get; set; }
        public int CreatedUserId { get; set; }
        
        private string name = string.Empty;

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

       
        public int RoleId { 
            get { return roleId; }
            set { roleId = value; }
        }
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

        public int CurrentSelectedDistrictId { get; set; }
        private List<string> studentIdList = new List<string>();
        private List<int> teacherIdList = new List<int>();
        
        public List<string> StudentIdList
        {
            get { return studentIdList; }
            set { studentIdList = value ?? new List<string>(); }
        }

        public List<int> TeacherIdList
        {
            get { return teacherIdList; }
            set { teacherIdList = value ?? new List<int>(); }
        }

        public int CurrentUserId { get; set; }
        public string CurrentUserName { get; set; }
    }
}