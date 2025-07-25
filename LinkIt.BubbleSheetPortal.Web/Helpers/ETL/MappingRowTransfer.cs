using LinkIt.BubbleSheetPortal.Models.ETL;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.ETL
{
    public class MappingRowTransfer : BaseMapping
    {
        public string MappingType { get; set; }
        public string MappingTypeName { get; set; }
        public string MappingValue { get; set; }
    }
}