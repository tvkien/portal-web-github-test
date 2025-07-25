using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Models.DataFileUpload;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class DataFileUploadLogRepository : IRepository<DataFileUploadLog>
    {
        private readonly Table<DataFileUploadLogEntity> table;
        private readonly AssessmentDataContext dbContext;

        public DataFileUploadLogRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = AssessmentDataContext.Get(connectionString);
            table = dbContext.GetTable<DataFileUploadLogEntity>();
        }

        public IQueryable<DataFileUploadLog> Select()
        {
            return table.Select(x => new DataFileUploadLog
            {
                DataFileUploadLogId = x.DataFileUploadLogId,
                CurrentUserId = x.CurrentUserId,
                DataFileUploadTypeId = x.DataFileUploadTypeID,
                FileName = x.FileName,
                QtiGroupId = x.QtiGroupId,
                ExtractedFoler = x.ExtractedFoler,
                ItemSetPath = x.ItemSetPath,
                DateStart = x.DateStart,
                DateEnd = x.DateEnd,
                Result = x.Result,
                Error = x.Error,
                QTI3pSourceId = x.Qti3pSourceID ?? 0,
                Status = x.Status
            });
        }

        public void Save(DataFileUploadLog item)
        {
            var entity = table.FirstOrDefault(x => x.DataFileUploadLogId.Equals(item.DataFileUploadLogId));

            if (entity.IsNull())
            {
                entity = new DataFileUploadLogEntity();
                table.InsertOnSubmit(entity);
            }
            entity.CurrentUserId = item.CurrentUserId;
            entity.DataFileUploadTypeID = item.DataFileUploadTypeId;
            entity.FileName = item.FileName;
            entity.QtiGroupId = item.QtiGroupId;
            entity.ExtractedFoler = item.ExtractedFoler;
            entity.ItemSetPath = item.ItemSetPath;
            entity.DateStart = item.DateStart;
            entity.DateEnd = item.DateEnd;
            entity.Result = item.Result;
            entity.Error = item.Error;
            entity.Qti3pSourceID = item.QTI3pSourceId;
            entity.Status = item.Status;

            table.Context.SubmitChanges();
            item.DataFileUploadLogId = entity.DataFileUploadLogId;
        }

        public void Delete(DataFileUploadLog item)
        {
            throw new NotImplementedException();
        }
    }
}