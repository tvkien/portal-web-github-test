using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.GenericSheets
{
    public class GenericSheetNewStudentViewModel : ValidatableEntity<GenericSheetNewStudentViewModel>
    {
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string middleName = string.Empty;
        private string password = string.Empty;
        private string confirmPassword = string.Empty;
        
        private string studentLocalId = string.Empty;
        private string studentStateId = string.Empty;

        private string className = string.Empty;
        private string schoolName = string.Empty;

        public string InputFileName { get; set; }
        public int GenericBubbleSheetId { get; set; }
        public int BubbleSheetFileId { get; set; }
        
        public GenericSheetNewStudentViewModel()
        {
            Genders = new List<SelectListItem>();
            Races = new List<SelectListItem>();
            Grades = new List<SelectListItem>();
        }
     
        public int StudentId { get; set; }
        public int? SchoolId { get; set; }
        public int? ClassId { get; set; }
        public int GenderId { get; set; }
        public int RaceId { get; set; }
        public int DistrictId { get; set; }
        public int? GradeId { get; set; }
        
        public List<SelectListItem> Genders { get; set; }
        public List<SelectListItem> Races { get; set; }
        public List<SelectListItem> Grades { get; set; }

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
    }
}