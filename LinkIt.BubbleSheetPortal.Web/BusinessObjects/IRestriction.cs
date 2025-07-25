using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.RestrictionDTO;
using LinkIt.BubbleSheetPortal.Web.Models.RestrictionDTO;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.BusinessObjects
{
    public interface IRestriction
    {
        List<ListItem> FilterBanks(FilterBankQueryDTO query, List<RestrictionDTO> restrictions = null);
        List<ListItem> FilterTests(FilterTestQueryDTO query, List<RestrictionDTO> restrictions = null);
    }
}
