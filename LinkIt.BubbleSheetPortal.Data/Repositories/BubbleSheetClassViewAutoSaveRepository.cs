using System;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BubbleSheetClassViewAutoSaveRepository : IBubbleSheetClassViewAutoSaveRepository
    {
        private readonly Table<BubbleSheetClassViewAutoSaveEntity> table;
        private readonly BubbleSheetDataContext _bubbleSheetDataContext;

        public BubbleSheetClassViewAutoSaveRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _bubbleSheetDataContext = BubbleSheetDataContext.Get(connectionString);
            table = _bubbleSheetDataContext.GetTable<BubbleSheetClassViewAutoSaveEntity>();
        }

        public IQueryable<BubbleSheetClassViewAutoSave> Select()
        {
            return table.Select(x => new BubbleSheetClassViewAutoSave
            {
                BubbleSheetClassViewAutoSaveId = x.BubbleSheetClassViewAutoSaveID,
                Ticket = x.Ticket,
                ClassId = x.ClassID ?? 0,
                UserId = x.UserID ?? 0,
                Data = x.Data,
                ActualData = x.ActualData,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate
            });
        }

        public void Save(BubbleSheetClassViewAutoSave item)
        {
            var entity =
                table.FirstOrDefault(x => x.ClassID == item.ClassId && x.Ticket.Equals(item.Ticket) && x.UserID == item.UserId);

            if (entity.IsNull())
            {
                entity = new BubbleSheetClassViewAutoSaveEntity();
                table.InsertOnSubmit(entity);
            }
            MappingBubbleSheetClassViewAutoSave(item, entity);
            table.Context.SubmitChanges();
        }

        public void Delete(BubbleSheetClassViewAutoSave item)
        {
            var entity =
                table.FirstOrDefault(x => x.ClassID == item.ClassId && x.Ticket.Equals(item.Ticket) && x.UserID == item.UserId);
            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MappingBubbleSheetClassViewAutoSave(BubbleSheetClassViewAutoSave item, BubbleSheetClassViewAutoSaveEntity entity)
        {
            entity.Ticket = item.Ticket;
            entity.ClassID = item.ClassId;
            entity.UserID = item.UserId;            
            entity.Data =  item.Data;
            entity.ActualData = item.ActualData;
            if (entity.CreatedDate == null)
                entity.CreatedDate = DateTime.UtcNow;
            entity.UpdatedDate = DateTime.UtcNow;
        }

        public void BubbleSheetDeleteAllAutoSaveData(string ticket, int classId)
        {
            _bubbleSheetDataContext.BubbleSheetDeleteAllAutoSaveData(ticket, classId);
        }
    }
}