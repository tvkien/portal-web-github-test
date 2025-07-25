using LinkIt.BubbleSheetPortal.Models.DTOs;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class PassageDeleteViewModel : ListItemsViewModel
    {
        public List<VirtualTestIncludeQtiItemDto> VirtualTests { get; set; } = new List<VirtualTestIncludeQtiItemDto>();

        public int VirtualTestCount => VirtualTests.Count;

        public bool CanDelete => VirtualTests.Count < 1;
    }
}
