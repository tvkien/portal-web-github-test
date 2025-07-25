using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class LessonFileTypeRepository : IReadOnlyRepository<LessonFileType>
    {
        private readonly Table<LessonFileTypeEntity> table;
        private readonly LearningLibraryDataContext _learningLibraryDataContext;

        public LessonFileTypeRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = LearningLibraryDataContext.Get(connectionString).GetTable<LessonFileTypeEntity>();
            _learningLibraryDataContext = LearningLibraryDataContext.Get(connectionString);
        }

        public IQueryable<LessonFileType> Select()
        {
            return table.Select(x => new LessonFileType
                                         {
                                             LessonFileTypeId = x.LessonFileTypeID,
                                             Name = x.Name,
                                             Description = string.IsNullOrEmpty(x.Description) ? string.Empty : x.Description,
                                             ThumbnailPath = x.ThumbnailPath
                                         });
        }
        
    }
}