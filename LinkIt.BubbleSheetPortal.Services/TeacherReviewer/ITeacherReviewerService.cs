using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services.TeacherReviewer
{
    public interface ITeacherReviewerService
    {
        bool CanGrading(CanGradingModel model);
    }
}
