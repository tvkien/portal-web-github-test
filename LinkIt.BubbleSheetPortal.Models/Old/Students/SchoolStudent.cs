using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class SchoolStudent
    {
        private string firstName = string.Empty;
        private string middleName = string.Empty;
        private string lastName = string.Empty;
        private string code = string.Empty;
        private string altCode = string.Empty;
        private string stateCode = string.Empty;
        private string gender = string.Empty;
        private string grade = string.Empty;

        public int StudentID { get; set; }
        public int SchoolID { get; set; }
        public bool? Active { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        
        public String Grade
        {
            get { return grade; }
            set { grade = value.ConvertNullToEmptyString(); }
        }

        public String Gender
        {
            get { return gender; }
            set { gender = value.ConvertNullToEmptyString(); }
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

        public string Code
        {
            get { return code; }
            set { code = value.ConvertNullToEmptyString(); }
        }

        public string AltCode
        {
            get { return altCode; }
            set { altCode = value.ConvertNullToEmptyString(); }
        }

        public string StateCode
        {
            get { return stateCode; }
            set { stateCode = value.ConvertNullToEmptyString(); }
        }
    }
}