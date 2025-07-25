using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using Envoc.Core.Shared.Extensions;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Models.Audit;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AnswerAuditRepository : IRepository<AnswerAuditData>
    {
        private readonly Table<AnswerAuditEntity> table;

        public AnswerAuditRepository(IConnectionString conn)
        {
            var connectionString = conn.GetAdminReportingLogConnectionString();
            table = TestAuditDataContext.Get(connectionString).GetTable<AnswerAuditEntity>();
            Mapper.CreateMap<AnswerAuditData, AnswerAuditEntity>();
        }

        public IQueryable<AnswerAuditData> Select()
        {
            return table.Select(x => new AnswerAuditData
                    {
                       AnswerAuditID = x.AnswerAuditID,
                       AnswerID = x.AnswerID,
                       DateTimeStamp = x.DateTimeStamp,
                       UserID = x.UserID,
                       NewValue = x.NewValue,
                       PreviousValue = x.PreviousValue
                    });
        }

        public void Save(AnswerAuditData item)
        {
            try
            {
                var entity = table.FirstOrDefault(x => x.AnswerAuditID.Equals(item.AnswerAuditID));

                if (entity == null)
                {
                    entity = new AnswerAuditEntity();
                    table.InsertOnSubmit(entity);
                }

                Mapper.Map(item, entity);
                table.Context.SubmitChanges();
                item.AnswerAuditID = entity.AnswerAuditID;
            }
            catch (Exception ex) { }
        }

        public void Delete(AnswerAuditData item)
        {
            if (item.IsNotNull())
            {
                throw new NotImplementedException();
            }
        }
    }
}
