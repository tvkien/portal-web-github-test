using LinkIt.BubbleSheetPortal.Models.DTOs.UserGroup;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IXLIAreaGroupModuleRepository
    {
        void Save(XLIAreaGroupModuleDto item);

        bool Delete(int xliGroupId, int xliModuleId);
    }
}
