using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class SubjectOrder
    {
        private string name = string.Empty;
        private string shortName = string.Empty;

        public int Id { get; set; }
        public int GradeId { get; set; }
        public int StateId { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public string ShortName
        {
            get { return shortName; }
            set { shortName = value.ConvertNullToEmptyString(); }
        }

        public override bool Equals(object obj)
        {
            var subject = obj as Subject;
            return subject != null && Id.Equals(subject.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public int Order { get; set; }
    }

    public class SubjectOrderDistrict
    {
        public int SubjectId { get; set; }
        public int Order { get; set; }
    }
}