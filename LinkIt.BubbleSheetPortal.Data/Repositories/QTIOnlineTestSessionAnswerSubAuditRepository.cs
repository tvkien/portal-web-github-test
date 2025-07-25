using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.Audit;
using System;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTIOnlineTestSessionAnswerSubAuditRepository : IRepository<QTIOnlineTestSessionAnswerSubAuditData>
    {
        private readonly Table<QTIOnlineTestSessionAnswerSubAuditEntity> table;

        public QTIOnlineTestSessionAnswerSubAuditRepository(IConnectionString conn)
        {
            var connectionString = conn.GetAdminReportingLogConnectionString();
            table = TestAuditDataContext.Get(connectionString).GetTable<QTIOnlineTestSessionAnswerSubAuditEntity>();
            Mapper.CreateMap<QTIOnlineTestSessionAnswerSubAuditData, QTIOnlineTestSessionAnswerSubAuditEntity>();
        }

        public void Delete(QTIOnlineTestSessionAnswerSubAuditData item)
        {
            throw new NotImplementedException();
        }

        public void Save(QTIOnlineTestSessionAnswerSubAuditData item)
        {
            try
            {
                var entity = table.FirstOrDefault(x => x.QTIOnlineTestSessionAnswerSubAuditID.Equals(item.QTIOnlineTestSessionAnswerSubAuditID));

                if (entity == null)
                {
                    entity = new QTIOnlineTestSessionAnswerSubAuditEntity();
                    table.InsertOnSubmit(entity);
                }

                Mapper.Map(item, entity);
                table.Context.SubmitChanges();
                item.QTIOnlineTestSessionAnswerSubAuditID = entity.QTIOnlineTestSessionAnswerSubAuditID;
            }
            catch (Exception ex) { }
        }

        public IQueryable<QTIOnlineTestSessionAnswerSubAuditData> Select()
        {
            return table.Select(x => new QTIOnlineTestSessionAnswerSubAuditData
            {
                QTIOnlineTestSessionAnswerSubAuditID = x.QTIOnlineTestSessionAnswerSubAuditID,
                QTIOnlineTestSessionAnswerSubID = x.QTIOnlineTestSessionAnswerSubID,
                DateTimeStamp = x.DateTimeStamp,
                UserID = x.UserID,
                NewValue = x.NewValue,
                PreviousValue = x.PreviousValue,
                QTIOnlineTestSessionID = x.QTIOnlineTestSessionID,
                VirtualQuestionSubID = x.VirtualQuestionSubID
            });
        }
    }
}
