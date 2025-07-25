using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class LessonContentTypeRepository : ILessonContentTypeRepository
    {
        private readonly Table<LessonContentTypeEntity> table;
        private readonly LearningLibraryDataContext _learningLibraryDataContext;

        public LessonContentTypeRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = LearningLibraryDataContext.Get(connectionString).GetTable<LessonContentTypeEntity>();
            _learningLibraryDataContext = LearningLibraryDataContext.Get(connectionString);
        }

        public IQueryable<LessonContentType> Select()
        {
            return table.Select(x => new LessonContentType
                                         {
                                             Id = x.LessonContentTypeID,
                                             Description = x.Description,
                                             ImagePath = x.ImagePath,
                                             Code = x.Code,
                                             Name = x.DisplayText,
                                             SortOrder = x.SortOrder
                                         });
        }
        
    }
}