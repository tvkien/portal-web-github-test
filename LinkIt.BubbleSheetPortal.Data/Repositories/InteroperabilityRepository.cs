using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class InteroperabilityRepository : IReadOnlyRepository<InteroperabilityDto>
    {
        private readonly Table<Interoperability> table;

        public InteroperabilityRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<Interoperability>();
        }

        public IQueryable<InteroperabilityDto> Select()
        {
            return table.Select(x => new InteroperabilityDto
            {
                DistrictID = x.DistrictID,
                InteroperabilityId = x.InteroperabilityId,
                LinkitObjectTypeId = x.LinkitObjectTypeId,
                ObjectId = x.ObjectId,
                SourceId = x.SourceId,
                ThirdPartySourceTypeID = x.ThirdPartySourceTypeID,
            });
        }
    }
}
