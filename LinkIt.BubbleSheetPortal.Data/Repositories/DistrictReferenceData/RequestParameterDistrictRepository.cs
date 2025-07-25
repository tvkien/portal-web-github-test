using System.Linq;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.DistrictReferenceData
{
    public class RequestParameterDistrictRepository:IReadOnlyRepository<RequestParameterDistrict>
    {
        private readonly Table<RequestParameterDistrictView> table;

        public RequestParameterDistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<RequestParameterDistrictView>();
        }

        public IQueryable<RequestParameterDistrict> Select()
        {
            return table.Select(x => new RequestParameterDistrict
            {
                RequestParameterID = x.RequestParameterID,
                RequestID = x.RequestID,
                Value = x.Value,
                Name = x.Name,
                DistrictID = x.DistrictID
            });
        }
    }
}