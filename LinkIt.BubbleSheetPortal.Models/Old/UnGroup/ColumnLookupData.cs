using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ColumnLookupData
    {
        private string data;

        public int LookupDataID { get; set; }
        public int ColumnID { get; set; }

        public string Data
        {
            get { return data; }
            set { data = value.ConvertNullToEmptyString(); }
        }
    }
}