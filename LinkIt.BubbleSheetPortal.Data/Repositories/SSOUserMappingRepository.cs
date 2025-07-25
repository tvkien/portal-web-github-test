using AutoMapper;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.SSO;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SSOUserMappingRepository : ISSOUserMappingRepository
    {
        private readonly Table<SSOUserMappingEntity> table;
        private readonly UserDataContext _userDataContext;

        public SSOUserMappingRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<SSOUserMappingEntity>();
            _userDataContext = UserDataContext.Get(connectionString);
        }

        public void Delete(SSOUserMapping item)
        {
            throw new System.NotImplementedException();
        }

        public SSOUserMappingEntity GetLinkitUserFromMapping(string adUsername, int districtId, string type)
        {
           return table.Where(m => m.ADUsername == adUsername && m.DistrictID == districtId && m.Type == type).FirstOrDefault();
        }

        public SSOUserMappingEntity GetLinkitUserFromMapping(string adUsername, int districtId, string type, IEnumerable<int> roleIds)
        {
            return table
                .Where(m => m.ADUsername == adUsername && m.DistrictID == districtId && m.Type == type && roleIds.Contains(m.UserEntity.RoleID))
                .FirstOrDefault();
        }

        public void Save(SSOUserMapping item)
        {
            var user = _userDataContext.UserEntities.FirstOrDefault(x => x.UserID == item.UserID);
            if (user.IsNull()) return;

            var roleIds = GetRoleIds(user.RoleID);
            var entity = GetLinkitUserFromMapping(item.ADUsername, item.DistrictID, item.Type, roleIds);

            if (entity.IsNull())
            {
                entity = new SSOUserMappingEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.SSOUserMappingID = entity.SSOUserMappingID;
        }

        public IQueryable<SSOUserMapping> Select()
        {
            throw new System.NotImplementedException();
        }

        private IEnumerable<int> GetRoleIds(int roleId)
        {
            if (roleId == (int)RoleEnum.Student)
            {
                return new List<int> { (int)RoleEnum.Student };
            }

            if (roleId == (int)RoleEnum.Parent)
            {
                return new List<int> { (int)RoleEnum.Parent };
            }

            return new List<int> {
                (int)RoleEnum.Publisher,
                (int)RoleEnum.NetworkAdmin,
                (int)RoleEnum.DistrictAdmin,
                (int)RoleEnum.SchoolAdmin,
                (int)RoleEnum.Teacher,
            };
        }
    }
}
