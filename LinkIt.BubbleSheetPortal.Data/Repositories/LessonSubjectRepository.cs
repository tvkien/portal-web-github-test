using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class LessonSubjectRepository : ILessonSubjectRepository
    {
        private readonly Table<LessonSubjectEntity> table;
        private readonly LearningLibraryDataContext _learningLibraryDataContext;

        public LessonSubjectRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = LearningLibraryDataContext.Get(connectionString).GetTable<LessonSubjectEntity>();
            _learningLibraryDataContext = LearningLibraryDataContext.Get(connectionString);
        }

        public IQueryable<LessonSubject> Select()
        {
            return table.Select(x => new LessonSubject
                                         {
                                             SubjectId = x.SubjectID,
                                             Name = x.Name
                                         });
        }
        
    }
}