using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class RosterType
    {
        private string rosterTypeName = string.Empty;        

        public int RosterTypeId { get; set; }
        
        public string RosterTypeName
        {
            get { return rosterTypeName; }
            set { rosterTypeName = value.ConvertNullToEmptyString(); }
        }        
    }
}