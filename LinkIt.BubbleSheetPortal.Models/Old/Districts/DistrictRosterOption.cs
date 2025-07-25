using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class DistrictRosterOption
    {
        public int DistrictRosterOptionId { get; set; }
        public int RosterTypeId { get; set; }
        public int? DistrictId { get; set; }
        public int DisplayOrder { get; set; }
        public int IsEnabled { get; set; }

        private string displayName = string.Empty;

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value.ConvertNullToEmptyString(); }
        }
    }
}