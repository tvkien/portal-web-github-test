using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.LTI;
using System;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.LTI
{
    public class LTIRequestHistoryRepository : IRepository<LTIRequestHistory>, ILTIRequestHistoryRepository
    {
        private readonly Table<LTIRequestHistoryEntity> _table;

        public LTIRequestHistoryRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = DbDataContext.Get(connectionString).GetTable<LTIRequestHistoryEntity>();
            Mapper.CreateMap<LTIRequestHistory, LTIRequestHistoryEntity>();
        }

        public void Delete(LTIRequestHistory item)
        {
            throw new NotImplementedException();
        }

        public void Save(LTIRequestHistory item)
        {
            var entity = _table.FirstOrDefault(x => x.Nonce.Equals(item.Nonce));

            if (entity.IsNull())
            {
                entity = new LTIRequestHistoryEntity();
                _table.InsertOnSubmit(entity);
            }
            Mapper.Map(item, entity);
            entity.UpdatedDate = DateTime.UtcNow;
            _table.Context.SubmitChanges();
        }

        public IQueryable<LTIRequestHistory> Select()
        {
            return _table.Select(o => new LTIRequestHistory
            {
                LTIRequestHistoryID = o.LTIRequestHistoryID,
                ClientID = o.ClientID,
                State = o.State,
                Nonce = o.Nonce
            });
        }

        public void UpdateStatus(string nonce, bool isCompleted)
        {
            var entity = _table.FirstOrDefault(x => x.Nonce.Equals(nonce));

            if (!entity.IsNull())
            {
                entity.UpdatedDate = DateTime.UtcNow;
                entity.IsCompleted = isCompleted;
                _table.Context.SubmitChanges();
            }
        }
    }
}
