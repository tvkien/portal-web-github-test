using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AggregateSubjectMappingRepository : IReadOnlyRepository<AggregateSubjectMapping>
    {
        private readonly Table<AggregateSubjectMappingEntity> table;

        public AggregateSubjectMappingRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<AggregateSubjectMappingEntity>();
        }
        public IQueryable<AggregateSubjectMapping> Select()
        {
            return table.Select(x => new AggregateSubjectMapping
            {
                AggregateSubjectMappingID = x.AggregateSubjectMappingID,
                AggregateSubjectName = x.AggregateSubjectName,
                DistrictID = x.DistrictID
            });
        }
    }
}
