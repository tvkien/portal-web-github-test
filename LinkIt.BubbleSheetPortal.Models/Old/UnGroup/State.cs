using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class State : ValidatableEntity<State>, IIdentifiable
    {
        private string name = string.Empty;
        private string code = string.Empty;

        public int Id { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public string Code
        {
            get { return code; }
            set { code = value.ConvertNullToEmptyString(); }
        }

        public string ZipRange { get; set; }

        public bool? International { get; set; }
        public string TimeZoneId { get; set; }
    }
}
