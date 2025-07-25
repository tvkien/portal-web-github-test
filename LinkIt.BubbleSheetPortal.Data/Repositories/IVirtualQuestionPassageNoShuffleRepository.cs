using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IVirtualQuestionPassageNoShuffleRepository : IRepository<VirtualQuestionPassageNoShuffle>
    {
        void DeleteAllPassageNoshuffle(int virtualquestionId);
    }
}