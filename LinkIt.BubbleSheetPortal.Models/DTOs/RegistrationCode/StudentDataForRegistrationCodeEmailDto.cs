using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.RegistrationCode
{
    public class StudentDataForRegistrationCodeEmailDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public int DistrictId { get; set; }
        public int? AdminSchoolId { get; set; }
        public string Email { get; set; }
        public string SharedSecret { get; set; }
    }
}
