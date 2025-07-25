using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.StudentLookup
{
    public class StudentRegistrationCodeEmailModel
    {
        public string HTTPProtocal { get; set; }
        public IEnumerable<StudentAndRegistrationCode> StudentEmailModels { get; set; }
    }
    public class StudentAndRegistrationCode
    {
        public int StudentId { get; set; }
        public string RegistrationCode { get; set; }
        public string UserName { get; set; }
    }

}
