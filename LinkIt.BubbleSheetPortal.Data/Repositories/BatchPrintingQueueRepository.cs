using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Data.Linq;
using System.Linq;
using AutoMapper;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BatchPrintingQueueRepository : IRepository<BatchPrintingQueue>
    {
        private readonly Table<BatchPrintingQueueEntity> _table;

        public BatchPrintingQueueRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<BatchPrintingQueueEntity>();
            Mapper.CreateMap<BatchPrintingQueue, BatchPrintingQueueEntity>();
        }

        public void Delete(BatchPrintingQueue item)
        {
            throw new NotImplementedException();
        }

        public void Save(BatchPrintingQueue item)
        {
            if (item == null) return;
            var entity = _table.FirstOrDefault(o => o.BatchPrintingQueueID == item.BatchPrintingQueueID);
            if (entity == null)
            {
                entity = new BatchPrintingQueueEntity();
                Mapper.Map(item, entity);
                _table.InsertOnSubmit(entity);
                _table.Context.SubmitChanges();
                item.BatchPrintingQueueID = entity.BatchPrintingQueueID;
                return;
            }

            Mapper.Map(item, entity);
            _table.Context.SubmitChanges();
        }

        public IQueryable<BatchPrintingQueue> Select()
        {
            var result = _table.Select(o => new BatchPrintingQueue
            {
                BatchPrintingQueueID = o.BatchPrintingQueueID,
                QTITestClassAssignmentID = o.QTITestClassAssignmentID,
                QTIOnlineTestSessionIDs = o.QTIOnlineTestSessionIDs,
                VirtualTestID = o.VirtualTestID,

                ManuallyGradedOnly = o.ManuallyGradedOnly,
                ManuallyGradedOnlyQuestionIds = o.ManuallyGradedOnlyQuestionIds,
                TeacherFeedback = o.TeacherFeedback,
                TheCoverPage = o.TheCoverPage,
                TheCorrectAnswer = o.TheCorrectAnswer,
                Passages = o.Passages,
                GuidanceAndRationale = o.GuidanceAndRationale,
                TheQuestionContent = o.TheQuestionContent,
                NumberOfColumn = o.NumberOfColumn,
                ShowQuestionPrefix = o.ShowQuestionPrefix,
                ShowBorderAroundQuestions = o.ShowBorderAroundQuestions,
                ExcludeTestScore = o.ExcludeTestScore,
                IncorrectQuestionsOnly = o.IncorrectQuestionsOnly,
                IncludeAttachment = o.IncludeAttachment,

                StudentType = o.StudentType,
                QuestionType = o.QuestionType,
                PrintQuestionIDs = o.PrintQuestionIDs,

                UserID = o.UserID,
                UserName = o.UserName,
                StudentName = o.StudentName,
                DistrictID = o.DistrictID,
                ProcessingStatus = o.ProcessingStatus,
                DownloadPdfID = o.DownloadPdfID,
                CreatedDate = o.CreatedDate
            });
            return result;
        }
    }
}
