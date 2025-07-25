using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageStudent;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class AddEditStudentViewModel : ValidatableEntity<AddEditStudentViewModel>
    {
        private string firstName = string.Empty;
        private string middleName = string.Empty;
        private string lastName = string.Empty;
        private string password = string.Empty;
        private string confirmPassword = string.Empty;
        private string studentLocalId = string.Empty;
        private string studentStateId = string.Empty;

        public AddEditStudentViewModel()
        {
            Genders = new List<SelectListItem>();
            Races = new List<SelectListItem>();
            Grades = new List<SelectListItem>();
            FromManageStudent = false;
            FilterDistrictId = 0;
            HasPortalAccount = false;
        }

        public int StudentId { get; set; }
        public int? AdminSchoolId { get; set; }
        public int GenderId { get; set; }
        public int RaceId { get; set; }
        public int DistrictId { get; set; }
        public int? GradeId { get; set; }
        public bool IsSISsystem { get; set; }
        public bool CanAccess { get; set; }

        public int CurrentUserRoleId { get; set; }
        public int CurrentUserId { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public int? UserId { get; set; }
        public string StudentNameWithSameLocalId { get; set; }
        public string StudentNameWithSameStateId { get; set; }

        public List<SelectListItem> Genders { get; set; }
        public List<SelectListItem> Races { get; set; }
        public List<SelectListItem> Grades { get; set; }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value.ConvertNullToEmptyString(); }
        }

        public string MiddleName
        {
            get { return middleName; }
            set { middleName = value.ConvertNullToEmptyString(); }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value.ConvertNullToEmptyString(); }
        }

        public string Password
        {
            get { return password; }
            set { password = value.ConvertNullToEmptyString(); }
        }

        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set { confirmPassword = value.ConvertNullToEmptyString(); }
        }

        public string StudentLocalId
        {
            get { return studentLocalId; }
            set { studentLocalId = value.ConvertNullToEmptyString(); }
        }

        public string StudentStateId
        {
            get { return studentStateId; }
            set { studentStateId = value.ConvertNullToEmptyString(); }
        }
        public bool FromManageStudent { get; set; }
        public int FilterDistrictId { get; set; }
        public string ClassIdString { get; set; }

        public string StudentMetaDatas { get; set; }

        public bool HasPortalAccount { get; set; }
        public string ReturnUrl { get; set; }

        public int ParentUserId { get; set; } = 0;

        public string SharedSecret { get; set; }
        public bool HasGenerateLogin { get; set; }
    }
}
