using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class BatchPrintingQueueService
    {
        private readonly IRepository<BatchPrintingQueue> _batchPrintingQueueRepository;
        public BatchPrintingQueueService(IRepository<BatchPrintingQueue> batchPrintingQueueRepository)
        {
            _batchPrintingQueueRepository = batchPrintingQueueRepository;
        }
        public BatchPrintingQueue GetBatchPrintingNotYetProcess()
        {
            using (
                var scope = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions {IsolationLevel = IsolationLevel.RepeatableRead}))
            {
                var result =
                    _batchPrintingQueueRepository.Select()
                        .Where(x => x.ProcessingStatus == (int) BatchPrintingProcessingStatusEnum.Pending)
                        .OrderBy(x => x.CreatedDate);
                var queue = result.FirstOrDefault();
                if (queue != null)
                {
                    try
                    {
                        queue.ProcessingStatus = (int)BatchPrintingProcessingStatusEnum.Processing;
                        _batchPrintingQueueRepository.Save(queue);
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

        public BatchPrintingQueue GetBatchPrintingByID(int batchPrintingQueueId)
        {
            using (
                var scope = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
            {
                var result =
                    _batchPrintingQueueRepository.Select()
                        .Where(x => x.BatchPrintingQueueID == batchPrintingQueueId)
                        .OrderBy(x => x.CreatedDate);
                var queue = result.FirstOrDefault();
                if (queue != null)
                {
                    try
                    {
                        queue.ProcessingStatus = (int)BatchPrintingProcessingStatusEnum.Processing;
                        _batchPrintingQueueRepository.Save(queue);
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

        public List<BatchPrintingQueue> GetBatchPrintingByTestClassAssignmentId(int testClassAssignmentId, int userId)
        {
            var result = _batchPrintingQueueRepository.Select().Where(x => x.QTITestClassAssignmentID == testClassAssignmentId && x.UserID == userId).ToList();
            return result;
        }
        public void SaveBatchPrintingQueue(BatchPrintingQueue data)
        {
            _batchPrintingQueueRepository.Save(data);
        }
    }
}
