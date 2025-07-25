using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Requests;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class RequestRepository : IRepository<Request>
    {
        private readonly Table<RequestEntity> table;
        private readonly Table<RequestNotificationEmailView> requestNotificationEmailView;

        public RequestRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<RequestEntity>();
            requestNotificationEmailView = dataContext.GetTable<RequestNotificationEmailView>();
        }

        public IQueryable<Request> Select()
        {
            //var test = requestNotificationEmailView.Select(x => new Request
            //{
            //    Id = x.RequestID,
            //    DataRequestType = (DataRequestType)x.DataRequestTypeID,
            //    DistrictId = x.DistrictID ?? 0,
            //    UserId = x.UserID,
            //    EmailAddress = x.Email,
            //    ImportedFileName = x.ImportedFileName,
            //    RequestTime = x.RequestTime,
            //    RequestMode = (RequestMode)x.Mode,
            //    RequestStatus = (RequestStatus)x.Status,
            //    IsDeleted = x.IsDeleted,
            //    HasBeenMoved = x.HasBeenMoved,
            //    HasEmailContent = x.HasEmailContent.Equals(true),
            //    RosterType = x.RosterType
            //}).ToList();
            return requestNotificationEmailView.Select(x => new Request
                {
                    Id = x.RequestID,
                    DataRequestType = (DataRequestType) x.DataRequestTypeID,
                    DistrictId = x.DistrictID ?? 0,
                    UserId = x.UserID,
                    EmailAddress = x.Email,
                    ImportedFileName = x.ImportedFileName,
                    RequestTime = x.RequestTime,
                    RequestMode = x.Mode != null ? (RequestMode) x.Mode : RequestMode.Validation,
                    RequestStatus = x.Status != null ? (RequestStatus) x.Status : RequestStatus.Pending,
                    IsDeleted = x.IsDeleted,
                    HasBeenMoved = x.HasBeenMoved,
                    HasEmailContent = x.HasEmailContent.Equals(true),
                    RosterType = x.RosterType
                });
        }

        public void Save(Request item)
        {
            var entity = table.FirstOrDefault(x => x.RequestID.Equals(item.Id));

            if (entity.IsNull())
            {
                entity = new RequestEntity();
                table.InsertOnSubmit(entity);
            }

            MapEntityToModel(entity, item);
            table.Context.SubmitChanges();
            item.Id = entity.RequestID;
        }

        public void Delete(Request item)
        {
            var entity = table.FirstOrDefault(x => x.RequestID.Equals(item.Id));

            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapEntityToModel(RequestEntity entity, Request item)
        {
            entity.RequestID = item.Id;
            entity.DataRequestTypeID = (int) item.DataRequestType;
            entity.DistrictID = item.DistrictId;
            entity.UserID = item.UserId;
            entity.Email = item.EmailAddress;
            entity.ImportedFileName = item.ImportedFileName;
            entity.RequestTime = item.RequestTime;
            entity.Mode = (int) item.RequestMode;
            entity.Status = (int) item.RequestStatus;
            entity.IsDeleted = item.IsDeleted;
            entity.HasBeenMoved = item.HasBeenMoved;
        }
    }
}