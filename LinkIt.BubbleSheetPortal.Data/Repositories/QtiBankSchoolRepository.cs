using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QtiBankSchoolRepository: IReadOnlyRepository<QtiBankSchool>, IInsertDeleteRepository<QtiBankSchool>
    {
        private readonly Table<QTIBankSchoolEntity> _table;
        private readonly TestDataContext _testDataContext;

        public QtiBankSchoolRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _testDataContext = TestDataContext.Get(connectionString);
            _table = _testDataContext.GetTable<QTIBankSchoolEntity>();
        }

        public IQueryable<QtiBankSchool> Select()
        {
            return _table.Select(x => new QtiBankSchool
            {
                SchoolId = x.SchoolID,
                EditedByUserId = x.EditedByUserID,
                QtiBankSchoolId = x.QTIBankSchoolID,
                QtiBankId = x.QTIBankID
            });
        }

        public void Save(QtiBankSchool item)
        {
            var entity = _table.FirstOrDefault(x => x.QTIBankSchoolID.Equals(item.QtiBankSchoolId));
            if (entity == null)
            {
                entity = new QTIBankSchoolEntity();
                BindData(entity, item);
                _table.InsertOnSubmit(entity);
            }
            else
            {
                BindData(entity, item);
            }
            _table.Context.SubmitChanges();
        }

        public void Delete(QtiBankSchool item)
        {
            var entity = _table.FirstOrDefault(x => x.QTIBankSchoolID.Equals(item.QtiBankSchoolId));
            if (entity != null)
            {
                _table.DeleteOnSubmit(entity);
                _table.Context.SubmitChanges();
            }
        }

        private void BindData(QTIBankSchoolEntity entity, QtiBankSchool item)
        {
            entity.SchoolID = item.SchoolId;
            entity.EditedByUserID = item.EditedByUserId;
            entity.QTIBankID = item.QtiBankId;
        }

        public void InsertMultipleRecord(List<QtiBankSchool> items)
        {
            foreach (var item in items)
            {
                var entity = new QTIBankSchoolEntity();
                BindData(entity, item);
                _table.InsertOnSubmit(entity);
            }

            _table.Context.SubmitChanges();
        }
    }
}
