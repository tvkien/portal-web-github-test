using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public  class DistrictSlideRepository : IReadOnlyRepository<DistrictSlide>
    {
        private readonly Table<DistrictSlideEntity> table;

        public DistrictSlideRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<DistrictSlideEntity>();
        }

        public IQueryable<DistrictSlide> Select()
        {
            return table.Select(x => new DistrictSlide
                {
                    DistrictId = x.DistrictID,
                    SlideOrder = x.SlideOrder,
                    ImageName = x.ImageName,
                    LinkTo = x.LinkTo,
                    NewTabOpen = x.NewTabOpen,
                    RoleID = x.RoleID
                });
        }
    }
}