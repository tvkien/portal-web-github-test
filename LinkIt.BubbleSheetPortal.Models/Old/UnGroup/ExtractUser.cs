using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ExtractUser
    {
        private string _userName = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _schoolName = string.Empty;

        public int UserId { get; set; }
        public int SchoolId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string UserName
        {
            get { return _userName; }
            set { _userName = value.ConvertNullToEmptyString(); }
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

        public string SchoolName
        {
            get { return _schoolName; }
            set { _schoolName = value.ConvertNullToEmptyString(); }
        }       
    }
}

