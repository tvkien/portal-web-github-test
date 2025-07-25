using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ClassStudent : ValidatableEntity<ClassStudent>
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? Active { get; set; }
        public string Code { get; set; }

        public string FullName
        {
            get { return LastName + ", " + FirstName; }
        }
    }
}