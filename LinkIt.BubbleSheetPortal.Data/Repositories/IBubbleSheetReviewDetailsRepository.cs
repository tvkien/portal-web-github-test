using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IBubbleSheetReviewDetailsRepository
    {
        List<BubbleSheetClassViewAnswer> GetBubbleSheetClassViewAnswerData(string studentIdList, string ticket,
            int classId);
    }
}
