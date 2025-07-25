using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class StudentProfileViewModel
    {
        private string studentFirstName = string.Empty;
        private string studentLastName = string.Empty;
        private string studentEmail = string.Empty;
        private string homeZipCode = string.Empty;
        private string schoolZipCode = string.Empty;
        private string schoolName = string.Empty;

        private string parentFirstName = string.Empty;
        private string parentLastName = string.Empty;
        private string parentEmail = string.Empty;
        private string phone = string.Empty;

        public int StudentId { get; set; }
        public int ParentId { get; set; }
        public int SchoolId { get; set; }
        public int? GradeId { get; set; }
        public int ProgramId { get; set; }
        public bool CreatedByStudent { get; set; }
        public string StudentFirstName
        {
            get { return studentFirstName; }
            set { studentFirstName = value.ConvertNullToEmptyString(); }
        }

        public string StudentLastName
        {
            get { return studentLastName; }
            set { studentLastName = value.ConvertNullToEmptyString(); }
        }

        public string StudentEmail
        {
            get { return studentEmail; }
            set { studentEmail = value.ConvertNullToEmptyString(); }
        }

        public string HomeZipCode
        {
            get { return homeZipCode; }
            set { homeZipCode = value.ConvertNullToEmptyString(); }
        }

        public string SchoolZipCode
        {
            get { return schoolZipCode; }
            set { schoolZipCode = value.ConvertNullToEmptyString(); }
        }

        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value.ConvertNullToEmptyString(); }
        }

        public string ParentFirstName
        {
            get { return parentFirstName; }
            set { parentFirstName = value.ConvertNullToEmptyString(); }
        }

        public string ParentLastName
        {
            get { return parentLastName; }
            set { parentLastName = value.ConvertNullToEmptyString(); }
        }

        public string ParentEmail
        {
            get { return parentEmail; }
            set { parentEmail = value.ConvertNullToEmptyString(); }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value.ConvertNullToEmptyString(); }
        }        
    }
}