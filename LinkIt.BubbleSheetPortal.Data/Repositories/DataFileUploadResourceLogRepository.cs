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
    public class DataFileUploadResourceLogRepository : IRepository<DataFileUploadResourceLog>
    {
        private readonly Table<DataFileUploadResourceLogEntity> table;
        private readonly AssessmentDataContext dbContext;

        public DataFileUploadResourceLogRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = AssessmentDataContext.Get(connectionString);
            table = dbContext.GetTable<DataFileUploadResourceLogEntity>();

        }

        public IQueryable<DataFileUploadResourceLog> Select()
        {
            return table.Select(x => new DataFileUploadResourceLog
            {
                DataFileUploadResourceLogId = x.DataFileUploadResourceLogId,
                DataFileUploadLogId = x.DataFileUploadLogId,
                ResourceFileName = x.ResourceFileName,
                OriginalContent = x.OriginalContent,
                ProcessingStep = x.ProcessingStep,
                IsValidQuestionResourceFile = x.IsValidQuestionResourceFile,
                QtiSchemaId = x.QtiSchemaId,
                XmlContent = x.XmlContent,
                QtiItemId = x.QtiItemId,
                Error = x.Error,
                ErrorDetail = x.ErrorDetail,
                QTI3pItemId = x.QTI3pItemID
            });

        }

        public void Save(DataFileUploadResourceLog item)
        {
            var entity = table.FirstOrDefault(x => x.DataFileUploadResourceLogId.Equals(item.DataFileUploadResourceLogId));

            if (entity.IsNull())
            {
                entity = new DataFileUploadResourceLogEntity();
                table.InsertOnSubmit(entity);
            }
            entity.DataFileUploadLogId = item.DataFileUploadLogId;
            entity.ResourceFileName = item.ResourceFileName;
            entity.OriginalContent = item.OriginalContent;
            entity.InteractionType = item.InteractionType;
            entity.ProcessingStep = item.ProcessingStep;
            entity.IsValidQuestionResourceFile = item.IsValidQuestionResourceFile;
            entity.QtiSchemaId = item.QtiSchemaId;
            entity.XmlContent = item.XmlContent;
            entity.QtiItemId = item.QtiItemId;
            entity.Error = item.Error;
            entity.ErrorDetail = item.ErrorDetail;
            entity.QTI3pItemID = item.QTI3pItemId;

            table.Context.SubmitChanges();
            item.DataFileUploadResourceLogId = entity.DataFileUploadResourceLogId;
        }

        public void Delete(DataFileUploadResourceLog item)
        {
           throw new NotImplementedException();
        }
    }
}