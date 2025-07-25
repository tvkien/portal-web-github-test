using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class EditStudentParentViewModel : ValidatableEntity<EditStudentParentViewModel>
    {
        private string firstName = string.Empty;
        private string lastName = string.Empty;
     
        public int StudentId { get; set; }
        public int DistrictId { get; set; }
        public bool CanAccess { get; set; }
        public int GenderId { get; set; }
        public int? GradeId { get; set; }
        public string GenderName { get; set; }
        public string GradeName { get; set; }
        public bool IsSISsystem { get; set; }
        
                
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value.ConvertNullToEmptyString(); }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value.ConvertNullToEmptyString(); }
        }
    }
}