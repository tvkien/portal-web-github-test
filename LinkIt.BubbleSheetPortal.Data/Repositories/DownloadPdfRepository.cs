using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class DownloadPdfRepository : IRepository<DownloadPdfData>
    {
        private readonly Table<DownloadPdfEntity> _table;
        private readonly TestDataContext _testDataContext;


        public DownloadPdfRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<DownloadPdfEntity>();
            _testDataContext = TestDataContext.Get(connectionString);
        }

        public void Delete(DownloadPdfData item)
        {
            throw new NotImplementedException();
        }

        public void Save(DownloadPdfData item)
        {
            if (item == null) return;
            DownloadPdfEntity entity = null;
            if(item.DownloadPdfID != Guid.Empty) entity = _table.FirstOrDefault(o => o.DownloadPdfID == item.DownloadPdfID);
            if(entity == null)
            {
                entity = new DownloadPdfEntity { DownloadPdfID = Guid.NewGuid() };
                Map(item, entity);
                _table.InsertOnSubmit(entity);
                _table.Context.SubmitChanges();
                item.DownloadPdfID = entity.DownloadPdfID;
                return;
            }

            Map(item, entity);
            _table.Context.SubmitChanges();
        }

        public IQueryable<DownloadPdfData> Select()
        {
            var result = _table.Select(o => new DownloadPdfData
            {
                DownloadPdfID = o.DownloadPdfID,
                FilePath = o.FilePath,
                UserID = o.UserID,
                CreatedDate = o.CreatedDate
            });
            return result;
        }

        internal DownloadPdfData Transform(DownloadPdfEntity entity)
        {
            if (entity == null) return null;
            var result = new DownloadPdfData
            {
                DownloadPdfID = entity.DownloadPdfID,
                FilePath = entity.FilePath,
                UserID = entity.UserID,
                CreatedDate = entity.CreatedDate
            };

            return result;
        }

        internal void Map(DownloadPdfData data, DownloadPdfEntity entity)
        {
            if (data == null || entity == null) return;
            entity.FilePath = data.FilePath;
            entity.UserID = data.UserID;
            entity.CreatedDate = data.CreatedDate;
        }
    }
}
