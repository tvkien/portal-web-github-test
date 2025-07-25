using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QtiBankDistrictRepository: IRepository<QtiBankDistrict>
    {
        private readonly Table<QTIBankDistrictEntity> _table;
        private readonly TestDataContext _testDataContext;

        public QtiBankDistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _testDataContext = TestDataContext.Get(connectionString);
            _table = _testDataContext.GetTable<QTIBankDistrictEntity>();
        }

        public IQueryable<QtiBankDistrict> Select()
        {
            return _table.Select(x => new QtiBankDistrict
            {
                DistrictId = x.DistrictID,
                EditedByUserId = x.EditedByUserID,
                QtiBankDistrictId = x.QTIBankDistrictID,
                QtiBankId = x.QTIBankID
            });
        }

        public void Save(QtiBankDistrict item)
        {
            var entity = _table.FirstOrDefault(x => x.QTIBankDistrictID.Equals(item.QtiBankDistrictId));
            if (entity == null)
            {
                entity = new QTIBankDistrictEntity();
                BindData(entity, item);
                _table.InsertOnSubmit(entity);
            }
            else
            {
                BindData(entity, item);
            }
            _table.Context.SubmitChanges();
        }

        public void Delete(QtiBankDistrict item)
        {
            var entity = _table.FirstOrDefault(x => x.QTIBankDistrictID.Equals(item.QtiBankDistrictId));
            if (entity != null)
            {
                _table.DeleteOnSubmit(entity);
                _table.Context.SubmitChanges();
            }
        }

        private void BindData(QTIBankDistrictEntity entity, QtiBankDistrict item)
        {
            entity.DistrictID = item.DistrictId;
            entity.EditedByUserID = item.EditedByUserId;
            entity.QTIBankID = item.QtiBankId;
        }
    }
}