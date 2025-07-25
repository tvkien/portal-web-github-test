using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models.Requests
{
    public class RequestParameter : ValidatableEntity<RequestParameter>
    {
        private string name = string.Empty;
        private string value = string.Empty;

        public int RequestParameterID { get; set; }
        public int RequestId { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value.ConvertNullToEmptyString(); }
        }
    }
}
