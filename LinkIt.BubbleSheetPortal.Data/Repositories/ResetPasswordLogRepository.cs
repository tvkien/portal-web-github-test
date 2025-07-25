using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Data.Linq;
using System.Linq;
namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ResetPasswordLogRepository : IResetPasswordLogRepository
    {
        private readonly Table<ResetPasswordLogEntity> _table;
        private readonly APILogContextDataContext _dbContext;

        public ResetPasswordLogRepository(IConnectionString conn)
        {
            var connectionString = conn.GetAdminReportingLogConnectionString();
            _table = APILogContextDataContext.Get(connectionString).GetTable<ResetPasswordLogEntity>();
            _dbContext = APILogContextDataContext.Get(connectionString);
        }

        public void Delete(Models.ResetPasswordLog item)
        {
            throw new NotImplementedException();
        }

        public void Save(Models.ResetPasswordLog item)
        {
            try
            {
                var entity = _table.FirstOrDefault(x => x.ResetPasswordID == item.ResetPasswordID);
                if (entity.IsNull())
                {
                    entity = new ResetPasswordLogEntity();
                    entity.RequestDate = DateTime.UtcNow;
                    _table.InsertOnSubmit(entity);
                }

                MapModelToEntity(entity, item);
                _table.Context.SubmitChanges();
                item.ResetPasswordID = entity.ResetPasswordID;
            }
            catch (Exception ex) { }
        }

        private void MapModelToEntity(ResetPasswordLogEntity entity, ResetPasswordLog item)
        {
            entity.DistrictCode = item.DistrictCode;
            entity.IPAddress = item.IPAddress;
            entity.UserName = item.UserName;
        }

        public IQueryable<Models.ResetPasswordLog> Select()
        {
            return _table.Select(x => new ResetPasswordLog
            {
                ResetPasswordID = x.ResetPasswordID,
                RequestDate = x.RequestDate,
                DistrictCode = x.DistrictCode,
                IPAddress = x.IPAddress,
                UserName = x.UserName
            });
        }
    }
}
