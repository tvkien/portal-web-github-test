using LinkIt.BubbleSheetPortal.Models.DTOs.UserGroup;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IXLIAreaGMRoleRepository
    {
        void Add(params XLIAreaGMRoleDto[] items);

        bool Delete(int xliGroupId, int xliModuleId);
    }
}
