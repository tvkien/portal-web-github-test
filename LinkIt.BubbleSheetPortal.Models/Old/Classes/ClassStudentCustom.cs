using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ClassStudentCustom
    {
        private string _code = string.Empty;
        private string _fullName = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        
        public int ClassId { get; set; }
        public int StudentId { get; set; }
        public int ClassStudentId { get; set; }
        
        public bool? Active { get; set; }
        public int? SISId { get; set; }
        public int? DistrictId { get; set; }
         public int? Status { get; set; }
        
        public string Code
        {
            get { return _code; }
            set { _code = value.ConvertNullToEmptyString(); }
        }

        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value.ConvertNullToEmptyString(); }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value.ConvertNullToEmptyString(); }
        }
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value.ConvertNullToEmptyString(); }
        }
    }
}
