using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Models.Requests;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class RequestParameterRepository : IInsertSelect<RequestParameter>
    {
        private readonly Table<RequestParameterEntity> table;

        public RequestParameterRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<RequestParameterEntity>();
        }

        public IQueryable<RequestParameter> Select()
        {
            return table.Select(x => new RequestParameter
                {
                    Name = x.Name,
                    RequestId = x.RequestID,
                    RequestParameterID = x.RequestParameterID,
                    Value = x.Value
                });
        }

        public void Save(RequestParameter item)
        {
            var entity = table.FirstOrDefault(x => x.RequestParameterID.Equals(item.RequestParameterID));

            if (entity.IsNull())
            {
                entity = new RequestParameterEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(entity, item);
            table.Context.SubmitChanges();
            item.RequestParameterID = entity.RequestParameterID;
        }

        private void MapModelToEntity(RequestParameterEntity entity, RequestParameter item)
        {
            entity.RequestParameterID = item.RequestParameterID;
            entity.RequestID = item.RequestId;
            entity.Name = item.Name;
            entity.Value = item.Value;
        }
    }
}