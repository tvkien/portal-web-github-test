using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;

using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class XLIGroupRepository : IXLIGroupRepository
    {
        private readonly Table<XLIGroupEntity> table;
        private readonly UserDataContext dbContext;

        public XLIGroupRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = UserDataContext.Get(connectionString);
            table = dbContext.GetTable<XLIGroupEntity>();
        }

        public void Add(XLIGroup item)
        {
            var entity = new XLIGroupEntity
            {
                DistrictID = item.DistrictID,
                Name = item.Name,
                InheritRoleFunctionality = item.InheritRoleFunctionality
            };
            table.InsertOnSubmit(entity);
            table.Context.SubmitChanges();
        }

        public bool Delete(int xliGroupId, int districtId)
        {
            var entity = table.FirstOrDefault(x => x.XLIGroupID == xliGroupId && x.DistrictID == districtId);

            if (entity == null)
            {
                return false;
            }

            dbContext.XLIDeleteGroup(xliGroupId);
            return true;
        }

        public IQueryable<XLIGroup> Select()
        {
            return table.Select(x => new XLIGroup()
            {
                XLIGroupID = x.XLIGroupID,
                DistrictID = x.DistrictID,
                Name = x.Name,
                InheritRoleFunctionality = x.InheritRoleFunctionality
            });
        }

        public bool Update(XLIGroup item)
        {
            var entity = table.FirstOrDefault(x => x.XLIGroupID == item.XLIGroupID && x.DistrictID == item.DistrictID);

            if (entity == null)
            {
                return false;
            }

            entity.Name = item.Name;
            entity.InheritRoleFunctionality = item.InheritRoleFunctionality;
            table.Context.SubmitChanges();
            return true;
        }
    }
}
