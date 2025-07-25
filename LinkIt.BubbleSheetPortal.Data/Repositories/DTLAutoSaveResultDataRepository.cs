using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DataLocker;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class DTLAutoSaveResultDataRepository : IRepository<DTLAutoSaveResultData>
    {
        DataLockerContextDataContext _context;
        private readonly Table<DTLAutoSaveResultDataEntity> table;
        public DTLAutoSaveResultDataRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = DataLockerContextDataContext.Get(connectionString);
            table = _context.GetTable<DTLAutoSaveResultDataEntity>();
        }
        public IQueryable<DTLAutoSaveResultData> Select()
        {
            return table.Select(x => new DTLAutoSaveResultData
            {
                DTLAutoSaveResultDataId = x.DTLAutoSaveResultDataID,
                VirtualTestId = x.VirtualTestID ?? 0,
                UserId = x.UserID ?? 0,
                ClassId = x.ClassID ?? 0,
                StudentTestResultScoresJson = x.StudentTestResultScoresJson,
                StudentTestResultSubScoresJson = x.StudentTestResultSubScoresJson,
                ActualTestResultScoresJson = x.ActualTestResultScoresJson,
                CreatedDate = x.CreatedDate,
                ResultDate = x.ResultDate
            });
        }
        public void Save(DTLAutoSaveResultData item)
        {
            if (item == null) return;
            var entity = table.FirstOrDefault(o => o.UserID == item.UserId && o.ClassID == item.ClassId && o.VirtualTestID == item.VirtualTestId);
            if (entity == null)
            {
                entity = new DTLAutoSaveResultDataEntity() { CreatedDate = DateTime.UtcNow };
                table.InsertOnSubmit(entity);
            }

            Map(item, entity);
            table.Context.SubmitChanges();
        }

        internal void Map(DTLAutoSaveResultData data, DTLAutoSaveResultDataEntity entity)
        {
            if (data == null || entity == null) return;
            entity.VirtualTestID = data.VirtualTestId;
            entity.UserID = data.UserId;
            entity.ClassID = data.ClassId;
            entity.StudentTestResultScoresJson = data.StudentTestResultScoresJson;
            entity.StudentTestResultSubScoresJson = data.StudentTestResultSubScoresJson;
            entity.ActualTestResultScoresJson = data.ActualTestResultScoresJson;
            entity.UpdatedDate = DateTime.UtcNow;
            entity.ResultDate = data.ResultDate;
        }

        public void Delete(DTLAutoSaveResultData item)
        {
            DTLAutoSaveResultDataEntity entity;
            if(item.ResultDate.HasValue)
                entity = table.FirstOrDefault(x => x.VirtualTestID == item.VirtualTestId && x.ClassID == item.ClassId && x.UserID == item.UserId && x.ResultDate == item.ResultDate);
            else
                entity = table.FirstOrDefault(x => x.VirtualTestID == item.VirtualTestId && x.ClassID == item.ClassId && x.UserID == item.UserId);

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
