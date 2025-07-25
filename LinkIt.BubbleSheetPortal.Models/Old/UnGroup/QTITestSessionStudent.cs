using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTITestSessionStudent
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string StudentLocalID { get; set; }

        public string StudentAltCode { get; set; }

        public string StudentStateID { get; set; }

        public string Gender { get; set; }

        public string Race { get; set; }
        
        public int StatusID { get; set; }
        public int StudentId { get; set; }
    }
}
