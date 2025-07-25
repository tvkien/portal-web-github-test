using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class DSPDistrictRepository : IDspDistrictRepository
    {
        private readonly Table<NetworkAdminView> networkAdminView;
        private readonly Table<DSPDistrictEntity> table;
        private readonly Table<UserEntity> userTable;
        private readonly Table<DSPDistrictView> dSPDistrictView;
        private readonly UserDataContext _dataContext;

        public DSPDistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _dataContext = UserDataContext.Get(connectionString);
            networkAdminView = _dataContext.GetTable<NetworkAdminView>();
            table = _dataContext.GetTable<DSPDistrictEntity>();
            userTable = _dataContext.GetTable<UserEntity>();
            dSPDistrictView = _dataContext.GetTable<DSPDistrictView>();
        }

        public IQueryable<DSPDistrict> Select()
        {

            return table.Select(x => new DSPDistrict
            {
                Id = x.DSPDistrictID,
                OrganizationDistrictID = x.OrganizationDistrictID,
                MemberDistrictID = x.MemberDistrictID,
                Type = x.Type
            });
        }

        public List<int> GetDistricIdbyNetWorkAdmin(int userId)
        {
            var results = from a in networkAdminView
                          where a.UserID == userId
                          orderby a.DistrictID
                          select a.DistrictID;
            return results.ToList();
        }

        public void Delete(DSPDistrict item)
        {
            throw new System.NotImplementedException();
        }

        public void Save(DSPDistrict item)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<District> GetDistrictMembers(int organizationDistrictId)
        {
            return dSPDistrictView.Where(x => x.OrganizationDistrictID == organizationDistrictId).Select(x => new District
            {
                Id = x.DistrictID,
                Name = x.Name,
                LICode = x.LICode,
                StateId = x.StateID,
                StateName = x.StateName
            });
        }

        /// <summary>
        /// Get State By OrgizationDistrictID
        /// </summary>
        /// <param name="organizationDistrictId"></param>
        /// <returns></returns>
        public IQueryable<State> GetStateByDistrictNetWorkAdmin(int organizationDistrictId)
        {
            var v = _dataContext.GetStateByDistrictNetWorkAdminID(organizationDistrictId).ToList();
            if (v.Count > 0)
                return v.Select(o => new State()
                {
                    Code = o.Code,
                    Id = o.StateID,
                    Name = o.Name
                }).ToList().AsQueryable();
            return null;
        }

        public List<int> GetUserIdsByDistrictIdAndRoleIds(int districtId, int[] roleIds)
        {
            IQueryable<int> filteredUsers = (from a in networkAdminView
                                             where a.DistrictID == districtId
                                             select a.UserID);
            var results = from filterd in filteredUsers
                          join user in userTable.Where(c => roleIds.Contains(c.RoleID))
                          on filterd equals user.UserID
                          select filterd;

            return results.ToList();
        }
    }
}
