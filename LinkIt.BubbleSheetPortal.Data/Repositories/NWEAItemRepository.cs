using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class NWEAItemRepository : IReadOnlyRepository<NWEAItem>
    {
        private readonly Table<NWEAItemView> table;

        public NWEAItemRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<NWEAItemView>();
        }

        public IQueryable<NWEAItem> Select()
        {
            return table.Select(x => new NWEAItem
                                {
                                     XMLContent = x.XmlContent,
                                     Difficulty = x.Difficulty,
                                     BloomsTaxonomy = x.BloomsTaxonomy,
                                     Subject = x.Subject,
                                     Urlpath = x.Urlpath,
                                     Number = x.Number,
                                     QTI3pItemID = x.QTI3pItemID,
                                     BloomsID = x.BloomsID ?? 0,
                                     SubjectID = x.SubjectID ?? 0,
                                     GradeID = x.GradeID ?? 0,
                                     ItemDifficultyID = x.ItemDifficultyID ?? 0,
                                     FleschKincaid = x.FleschKincaid ,
                                     WordCountID = x.WordCountID ?? 0,
                                     TextSubTypeID = x.TextSubTypeID?? 0,
                                     TextTypeID = x.TextTypeID ?? 0,
                                     QTI3pItemPassageID = x.QTI3pItemPassageID 
                                });
        }
    }
}
