using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class LessonProviderRepository : IReadOnlyRepository<LessonProvider>
    {
        private readonly Table<LessonProviderDistrictViewEntity> view;

        public LessonProviderRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            view = LearningLibraryDataContext.Get(connectionString).GetTable<LessonProviderDistrictViewEntity>();
        }

        public IQueryable<LessonProvider> Select()
        {
            return view.Select(x => new LessonProvider
                {
                    Id = x.DistrictID,
                    Name = x.Name,
                    LICode = x.LICode,
                    AllDistrict = x.AllDistrict,
                    StateId = x.StateID,
                    StateName = x.StateName
                });
        }
    }
}