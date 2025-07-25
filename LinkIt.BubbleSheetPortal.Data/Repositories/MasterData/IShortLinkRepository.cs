using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.MasterData
{
    public interface IShortLinkRepository
    {
        string GetFullLinkByCode(string code);
        void Add(ShortLinkDto model);
    }
}
