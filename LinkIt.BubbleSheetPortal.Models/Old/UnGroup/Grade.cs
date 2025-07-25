using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class Grade : ValidatableEntity<Grade>, IIdentifiable
    {
        private string name = string.Empty;

        public int Id { get; set; }
        public int Order { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public override bool Equals(object obj)
        {
            var grade = obj as Grade;
            return grade != null && Id.Equals(grade.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}