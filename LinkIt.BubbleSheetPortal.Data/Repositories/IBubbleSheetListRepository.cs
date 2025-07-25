using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetReview;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IBubbleSheetListRepository 
    {
        IQueryable<BubbleSheetListItem> GetBubbleSheetListProc(int districtId, int userId, int roleId, bool archived);

        IQueryable<BubbleSheetListItem> GetBubbleSheetListProcV2(int districtId, int schoolId, int userId, int roleId, bool archived,
            DateTime createdDate);

        GetBubbleSheetReviewResponse GetBubbleSheetListProc_IncludedStatus(GetBubbleSheetReviewRequest request);
    }
}
