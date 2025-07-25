using System.Linq;
using Envoc.Core.Shared.Data;
using System.Transactions;
using LinkIt.BubbleSheetPortal.Models.DataFileUpload;
using LinkIt.BubbleSheetPortal.Common.DataFileUpload;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DataFileUploadLogService
    {
        private readonly IRepository<DataFileUploadLog> _dataFileUploadLogRepository;
        private readonly IRepository<DataFileUploadResourceLog> _dataFileUploadResourceLogRepository;

        public DataFileUploadLogService(IRepository<DataFileUploadLog> dataFileUploadLogRepository,
            IRepository<DataFileUploadResourceLog> dataFileUploadResourceLogRepository)
        {
            this._dataFileUploadLogRepository = dataFileUploadLogRepository;
            this._dataFileUploadResourceLogRepository = dataFileUploadResourceLogRepository;
        }

        public void CreateDataFileUploadLog(DataFileUploadLog dataFileUploadLog)
        {
            _dataFileUploadLogRepository.Save(dataFileUploadLog);
        }

        public void CreateDataFileUploadResourceLog(DataFileUploadResourceLog dataFileUploadResourceLog)
        {
            _dataFileUploadResourceLogRepository.Save(dataFileUploadResourceLog);
        }

        public DataFileUploadLog GetDataFileUploadLogById(int dataFileUploadLogId)
        {
            var data = _dataFileUploadLogRepository.Select().Where(x => x.DataFileUploadLogId == dataFileUploadLogId);
            if (data != null)
                return data.FirstOrDefault();
            return null;
        }

        public IQueryable<DataFileUploadLog> GetDataFileUpload3pLog(int qTI3pSourceId)
        {
            return _dataFileUploadLogRepository.Select().Where(x => x.QTI3pSourceId == qTI3pSourceId && x.QTI3pSourceId > 0 && x.Status == (int)DataFileUploadProcessingEnum.Finish);
        }
        public IQueryable<DataFileUploadResourceLog> GetQTI3pItems(int dataFileUploadLogId)
        {
            return _dataFileUploadResourceLogRepository.Select().Where(x => x.DataFileUploadLogId == dataFileUploadLogId && x.IsValidQuestionResourceFile.HasValue
                && x.IsValidQuestionResourceFile.Value && x.QTI3pItemId.HasValue && x.QTI3pItemId.Value > 0);
        }
        public IQueryable<DataFileUploadResourceLog> GetQTIItems(int dataFileUploadLogId)
        {
            return _dataFileUploadResourceLogRepository.Select().Where(x => x.DataFileUploadLogId == dataFileUploadLogId && x.IsValidQuestionResourceFile.HasValue
                && x.IsValidQuestionResourceFile.Value && x.QtiItemId.HasValue && x.QtiItemId.Value > 0);
        }
        public IQueryable<DataFileUploadResourceLog> Get3PFilesFail(int dataFileUploadLogId)
        {
            return _dataFileUploadResourceLogRepository.Select().Where(x => x.DataFileUploadLogId == dataFileUploadLogId && !x.IsValidQuestionResourceFile.Value && x.QtiSchemaId > 0 && (!x.QTI3pItemId.HasValue || x.QTI3pItemId.Value == 0));
        }
        public IQueryable<DataFileUploadResourceLog> GetItemSetFilesFail(int dataFileUploadLogId)
        {
            return _dataFileUploadResourceLogRepository.Select().Where(x => x.DataFileUploadLogId == dataFileUploadLogId && !x.IsValidQuestionResourceFile.Value && x.QtiSchemaId > 0 && (!x.QtiItemId.HasValue || x.QtiItemId.Value == 0));
        }
        public DataFileUploadLog GetDataFileUploadLogNotProcess()
        {
            //return _dataFileUploadLogRepository.Select().Where(x => x.DataFileUploadLogId == 27).FirstOrDefault();
            using (
                var scope = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
            {
                var result =
                   _dataFileUploadLogRepository.Select().Where(x => x.Status.HasValue && x.Status == (int)DataFileUploadProcessingEnum.NotProcess)
                        .OrderBy(x => x.DataFileUploadLogId);
                var queue = result.FirstOrDefault();
                if (queue != null)
                {
                    try
                    {
                        queue.Status = (int)DataFileUploadProcessingEnum.Processing;
                        _dataFileUploadLogRepository.Save(queue);
                    }
                    catch
                    {
                        return null;
                    }
                }
                scope.Complete();
                return queue;
            }
        }
    }
}