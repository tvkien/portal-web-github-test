using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ConversionSet
    {
        public int ConverstionSetID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ConversionMethod { get; set; }
        public int VirtualTestSubTypeID { get; set; }
            
    }
}