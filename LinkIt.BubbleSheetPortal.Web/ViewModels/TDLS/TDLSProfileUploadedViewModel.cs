using System;
using LinkIt.BubbleSheetPortal.Common;


namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TDLSProfileUploadViewModel
    {        
        public int ProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GenderId { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DateOfBirthString
        {
            get { return DateOfBirth.HasValue ? DateOfBirth.Value.DisplayDateWithFormat() : ""; }
            set {
                if (!string.IsNullOrEmpty(value))
                {
                    DateTime date = DateTime.MinValue;
                    value.TryParseDateWithFormat(out date);
                    DateOfBirth = date;
                } }
        }
        public string ECSName { get; set; }
        public bool? Section102IsCompleted { get; set; } 
        public int? EnrollmentYear { get; set; }
        public string PdfFileName { get; set; }

    }
}