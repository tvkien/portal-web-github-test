using System;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BubbleSheetErrorRepository : IRepository<BubbleSheetError>
    {
        private readonly Table<BubbleSheetErrorEntity> table; 

        public BubbleSheetErrorRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = BubbleSheetDataContext.Get(connectionString).GetTable<BubbleSheetErrorEntity>();
            Mapper.CreateMap<BubbleSheetError, BubbleSheetErrorEntity>();
        }

        public IQueryable<BubbleSheetError> Select()
        {
            return table.Select(x => new BubbleSheetError
                                         {
                                             BubbleSheetErrorId = x.ID,
                                             BubbleSheetId = x.BubbleSheetID,
                                             CreatedDate = x.CreatedDate, 
                                             FileName = x.FileName,
                                             IsCorrected = x.IsCorrected,
                                             Message = x.Message,
                                             RelatedImage = x.RelatedImage,
                                             ErrorCode = x.ErrorCode == null ? -1 : Convert.ToInt32(x.ErrorCode),
                                             RosterPosition = Convert.ToInt32(x.RosterPosition ?? 0),
                                             UserId = x.UserID
                                         });
        }

        public void Save(BubbleSheetError item)
        {
            var entity = table.FirstOrDefault(x => x.ID.Equals(item.BubbleSheetErrorId));

            if (entity.IsNull())
            {
                entity = new BubbleSheetErrorEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.BubbleSheetErrorId = entity.ID;
        }

        public void Delete(BubbleSheetError item)
        {
            var entity = table.FirstOrDefault(x => x.ID.Equals(item.BubbleSheetErrorId));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}