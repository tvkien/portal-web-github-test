using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ConversionSetDetailRepository : IRepository<ConversionSetDetail>
    {
        private readonly TestDataContext _testDataContext;
        private readonly Table<ConversionSetDetailEntity> _table;

        public ConversionSetDetailRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _testDataContext = TestDataContext.Get(connectionString);
            _table = _testDataContext.GetTable<ConversionSetDetailEntity>();
        }

        public void Save(ConversionSetDetail item)
        {
            var entity = _table.FirstOrDefault(x => x.ConversionSetDetailID == item.ConversionSetDetailID);
            if (entity == null)
            {
                entity = new ConversionSetDetailEntity();
                _table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);
            _table.Context.SubmitChanges();
            item.ConversionSetDetailID = entity.ConversionSetDetailID;
        }

        public IQueryable<ConversionSetDetail> Select()
        {
            return _table.Select(x => new ConversionSetDetail
            {
                ConversionSetDetailID = x.ConversionSetDetailID,
                ConversionSetID = x.ConversionSetID,
                Input1 = x.Input1,
                Input2 = x.Input2,
                ConvertedScore = x.ConvertedScore,
                ConvertedScore_A = x.ConvertedScore_A
            });
        }

        public void Delete(ConversionSetDetail item)
        {
            var entity = _table.FirstOrDefault(x => x.ConversionSetDetailID == item.ConversionSetDetailID);

            if (entity != null)
            {
                _table.DeleteOnSubmit(entity);
                _table.Context.SubmitChanges();
            }
        }

        #region Private

        private void MapModelToEntity(ConversionSetDetail item, ConversionSetDetailEntity entity)
        {
            entity.ConversionSetID = item.ConversionSetID;
            entity.Input1 = item.Input1;
            entity.Input2 = item.Input2;
            entity.ConvertedScore = item.ConvertedScore;
            entity.ConvertedScore_A = item.ConvertedScore_A;
        }

        #endregion
    }
}
