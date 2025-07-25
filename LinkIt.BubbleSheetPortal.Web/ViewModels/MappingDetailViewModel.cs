using System.Collections.Generic;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class MappingDetailViewModel
    {
        public int MapID { get; set; }
        public Dictionary<int, string> SourceColumns { set; get; }
        public List<SelectListItem> DestinationColumnTypes
        {
            get
            {
                var types = new List<SelectListItem>
                {
                    new SelectListItem {Text = "Source Column", Value = ((int)MappingDestinationColumnTypes.SourceColumn).ToString()},
                    new SelectListItem {Text = "Fixed Value", Value = ((int)MappingDestinationColumnTypes.FixedValue).ToString()},
                    new SelectListItem {Text = "Prefix + Source Column", Value = ((int)MappingDestinationColumnTypes.PrefixColumn).ToString()},
                    new SelectListItem {Text = "Lookup Mapping", Value = ((int)MappingDestinationColumnTypes.LookupMapping).ToString()}
                };

                return types;
            }
        }

        public string SelectedType { get; set; }
    }
}