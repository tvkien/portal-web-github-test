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
    public class AnswerSubAuditRepository : IRepository<AnswerSubAuditData>
    {
        private readonly Table<AnswerSubAuditEntity> table;

        public AnswerSubAuditRepository(IConnectionString conn)
        {
            var connectionString = conn.GetAdminReportingLogConnectionString();
            table = TestAuditDataContext.Get(connectionString).GetTable<AnswerSubAuditEntity>();
            Mapper.CreateMap<AnswerSubAuditData, AnswerSubAuditEntity>();
        }

        public IQueryable<AnswerSubAuditData> Select()
        {
            return table.Select(x => new AnswerSubAuditData
            {
                AnswerSubAuditID = x.AnswerSubAuditID,
                AnswerSubID = x.AnswerSubID,
                DateTimeStamp = x.DateTimeStamp,
                UserID = x.UserID,
                NewValue = x.NewValue,
                PreviousValue = x.PreviousValue
            });
        }

        public void Save(AnswerSubAuditData item)
        {
            try
            {
                var entity = table.FirstOrDefault(x => x.AnswerSubAuditID.Equals(item.AnswerSubAuditID));

                if (entity == null)
                {
                    entity = new AnswerSubAuditEntity();
                    table.InsertOnSubmit(entity);
                }

                Mapper.Map(item, entity);
                table.Context.SubmitChanges();
                item.AnswerSubAuditID = entity.AnswerSubAuditID;
            }
            catch (Exception ex) { }
        }

        public void Delete(AnswerSubAuditData item)
        {
            if (item.IsNotNull())
            {
                throw new NotImplementedException();
            }
        }
    }
}
