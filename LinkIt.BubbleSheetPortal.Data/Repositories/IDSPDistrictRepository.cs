using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IDspDistrictRepository : IRepository<DSPDistrict>
    {
        List<int> GetDistricIdbyNetWorkAdmin(int userId);
        List<int> GetUserIdsByDistrictIdAndRoleIds(int districtId, int[] roleIds);
        IQueryable<District> GetDistrictMembers(int organizationDistrictId);
        IQueryable<State> GetStateByDistrictNetWorkAdmin(int organizationDistrictId);
    }
}
