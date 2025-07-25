using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class SchoolMeta : ValidatableEntity<SchoolMeta>
    {
        private string name = string.Empty;
        private string data = string.Empty;

        public int SchoolMetaID { get; set; }
        public int SchoolID { get; set; }

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
