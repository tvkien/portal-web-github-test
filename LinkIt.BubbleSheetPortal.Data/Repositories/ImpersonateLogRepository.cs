using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    //public class ImpersonateLogRepository : IRepository<ImpersonateLog>
    public class ImpersonateLogRepository // change to use DB AdminReportingLog
    {
        private readonly Table<ImpersonateLogEntity> table;

        public ImpersonateLogRepository(IConnectionString conn)
        {
            var connectionString = conn.GetAdminReportingLogConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<ImpersonateLogEntity>();
        }

        public IQueryable<ImpersonateLog> Select()
        {
            return table.Select(x => new ImpersonateLog()
                {
                    ImpersonateLogID = x.ImpersonateLogID,
                    SessionCookieGUID = x.SessionCookieGUID,
                    ActionType = x.ActionType,
                    ActionTime = x.ActionTime,
                    OriginalUserId = x.OriginalUserId,
                    CurrentUserId = x.CurrentUserId,
                    ImpersonatedUserId = x.ImpersonatedUserId
                });
        }

        public void Save(ImpersonateLog item)
        {
            try
            {
                var entity = table.FirstOrDefault(x => x.ImpersonateLogID.Equals(item.ImpersonateLogID));

                if (entity.IsNull())
                {
                    entity = new ImpersonateLogEntity();
                    table.InsertOnSubmit(entity);
                }
                //assign value
                entity.SessionCookieGUID = item.SessionCookieGUID;
                entity.ActionType = item.ActionType;
                entity.ActionTime = item.ActionTime;
                entity.OriginalUserId = item.OriginalUserId;
                entity.CurrentUserId = item.CurrentUserId;
                entity.ImpersonatedUserId = item.ImpersonatedUserId;

                table.Context.SubmitChanges();
                item.ImpersonateLogID = entity.ImpersonateLogID;
            }
            catch (Exception ex) { }
        }

        public void Delete(ImpersonateLog item)
        {
            try
            {
                var entity = table.FirstOrDefault(x => x.ImpersonateLogID.Equals(item.ImpersonateLogID));

                if (!entity.IsNull())
                {
                    table.DeleteOnSubmit(entity);
                    table.Context.SubmitChanges();
                }
            }
            catch (Exception ex) { }
        }

    }
}
