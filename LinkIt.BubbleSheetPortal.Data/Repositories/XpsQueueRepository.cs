using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.Old.XpsQueue;
using LinkIt.BubbleSheetPortal.Models.Requests;
using System;
using System.Data.Linq;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Configuration;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class XpsQueueRepository : IRepository<XpsQueue>
    {
        private readonly Table<xpsQueueEntity> table;

        public XpsQueueRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<xpsQueueEntity>();
        }

        public void Delete(XpsQueue item) { }

        public void Save(XpsQueue item)
        {
            var entity = table.FirstOrDefault(x => x.xpsQueueID == item.XpsQueueID);

            if (entity.IsNull())
            {
                entity = new xpsQueueEntity();
                table.InsertOnSubmit(entity);
            }
            else
            {
                table.Context.Refresh(RefreshMode.OverwriteCurrentValues, entity);
            }
            MapEntityToModel(entity, item);
            table.Context.SubmitChanges();
            item.XpsQueueID = entity.xpsQueueID;
        }

        public IQueryable<XpsQueue> Select()
        {
            return table.Select(m => new XpsQueue
            {
                ProcessId = m.ProcessId,
                SchedStart = m.SchedStart,
                TimeEnd = m.TimeEnd,
                TimeStart = m.TimeStart,
                XpsDistrictUploadID = m.xpsDistrictUploadID,
                XpsQueueID = m.xpsQueueID,
                XpsQueueResultID = m.xpsQueueResultID,
                XpsQueueStatusID = m.xpsQueueStatusID,
                XpsUpLoadTypeID = m.xpsUpLoadTypeID,
            });
        }

        private void MapEntityToModel(xpsQueueEntity entity, XpsQueue item)
        {
            entity.SchedStart = item.SchedStart;
            entity.TimeStart = item.TimeStart;
            entity.TimeEnd = item.TimeEnd;
            entity.ProcessId = item.ProcessId;
            entity.xpsQueueStatusID = item.XpsQueueStatusID;
            entity.xpsQueueResultID = item.XpsQueueResultID;
            entity.xpsDistrictUploadID = item.XpsDistrictUploadID;
            entity.xpsUpLoadTypeID = item.XpsUpLoadTypeID;
            entity.IsValidation = item.IsValidation;
        }
    }
}
