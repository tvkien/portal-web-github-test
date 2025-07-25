using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ReportGroup : ValidatableEntity<ReportGroup>
    {
        private string name = string.Empty;
        private int groupType = 1;

        public int ReportGroupId { get; set; }
        public int UserId { get; set; }        
        public DateTime CreatedDateTime { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
        public int GroupType
        {
            get { return groupType; }
            set { groupType = value; }
        }
    }
}