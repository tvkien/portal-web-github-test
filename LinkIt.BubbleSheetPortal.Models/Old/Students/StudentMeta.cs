using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class StudentMeta : ValidatableEntity<StudentMeta>
    {
        private string name = string.Empty;
        private string data = string.Empty;

        public int StudentMetaID { get; set; }
        public int StudentID { get; set; }
        public string Code { get; set; }
        
        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public string Data
        {
            get { return data; }
            set { data = value.ConvertNullToEmptyString(); }
        }
    }
}