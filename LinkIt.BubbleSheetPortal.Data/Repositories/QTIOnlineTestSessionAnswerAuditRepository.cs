using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.Audit;
using System;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTIOnlineTestSessionAnswerAuditRepository : IRepository<QTIOnlineTestSessionAnswerAuditData>
    {
        private readonly Table<QTIOnlineTestSessionAnswerAuditEntity> table;

        public QTIOnlineTestSessionAnswerAuditRepository(IConnectionString conn)
        {
            var connectionString = conn.GetAdminReportingLogConnectionString();
            table = TestAuditDataContext.Get(connectionString).GetTable<QTIOnlineTestSessionAnswerAuditEntity>();
            Mapper.CreateMap<QTIOnlineTestSessionAnswerAuditData, QTIOnlineTestSessionAnswerAuditEntity>();
        }

        public void Delete(QTIOnlineTestSessionAnswerAuditData item)
        {
            throw new NotImplementedException();
        }

        public void Save(QTIOnlineTestSessionAnswerAuditData item)
        {
            try
            {
                var entity = table.FirstOrDefault(x => x.QTIOnlineTestSessionAnswerAuditID.Equals(item.QTIOnlineTestSessionAnswerAuditID));

                if (entity == null)
                {
                    entity = new QTIOnlineTestSessionAnswerAuditEntity();
                    table.InsertOnSubmit(entity);
                }

                Mapper.Map(item, entity);
                table.Context.SubmitChanges();
                item.QTIOnlineTestSessionAnswerAuditID = entity.QTIOnlineTestSessionAnswerAuditID;
            }
            catch (Exception ex) { }
        }

        public IQueryable<QTIOnlineTestSessionAnswerAuditData> Select()
        {
            return table.Select(x => new QTIOnlineTestSessionAnswerAuditData
            {
                QTIOnlineTestSessionAnswerAuditID = x.QTIOnlineTestSessionAnswerAuditID,
                QTIOnlineTestSessionAnswerID = x.QTIOnlineTestSessionAnswerID,
                DateTimeStamp = x.DateTimeStamp,
                UserID = x.UserID,
                NewValue = x.NewValue,
                PreviousValue = x.PreviousValue,
                QTIOnlineTestSessionID = x.QTIOnlineTestSessionID,
                VirtualQuestionID = x.VirtualQuestionID
            });
        }
    }
}
