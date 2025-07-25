using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGODataPointRepository : ISGODataPointRepository
    {
        private readonly Table<SGODataPointEntity> table;
        private readonly SGODataContext _context;
        public SGODataPointRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = SGODataContext.Get(connectionString);
            table = SGODataContext.Get(connectionString).GetTable<SGODataPointEntity>();
        }
        public IQueryable<SGODataPoint> Select()
        {
            return table.Select(x => new SGODataPoint
            {
                SGODataPointID = x.SGODataPointID,
                Name = x.Name,
                SubjectName = x.SubjectName,
                GradeID = x.GradeID,
                VirtualTestID = x.VirtualTestID,
                AttachScoreUrl = x.AttachScoreUrl,
                SGOID = x.SGOID,
                Weight = x.Weight,
                TotalPoints = x.TotalPoints,
                Type = x.Type,
                AchievementLevelSettingID = x.AchievementLevelSettingID,
                ResultDate = x.ResultDate,
                RationaleGuidance = x.RationaleGuidance,
                ScoreType = x.ScoreType,
                ImprovementBasedDataPoint = x.ImprovementBasedDataPoint,
                IsTemporary = x.IsTemporary,
            });
        }

        public void Save(SGODataPoint item)
        {
            var entity = table.FirstOrDefault(x => x.SGODataPointID.Equals(item.SGODataPointID));

            if (entity == null)
            {
                entity = new SGODataPointEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);

            table.Context.SubmitChanges();
            item.SGODataPointID = entity.SGODataPointID;
        }

        public void Delete(SGODataPoint item)
        {
            var entity = table.FirstOrDefault(x => x.SGODataPointID.Equals(item.SGODataPointID));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public SGODataPoint GetByID(int sgoDataPointId)
        {
            return _context.SGOGetSGODataPointByID(sgoDataPointId).Select(x => new SGODataPoint
            {
                SGODataPointID = x.SGODataPointID,
                Name = x.Name,
                SubjectName = x.SubjectName,
                GradeID = x.GradeID.GetValueOrDefault(),
                VirtualTestID = x.VirtualTestID,
                AttachScoreUrl = x.AttachScoreUrl,
                SGOID = x.SGOID,
                Weight = x.Weight,
                TotalPoints = x.TotalPoints,
                Type = x.Type,
                AchievementLevelSettingID = x.AchievementLevelSettingID,
                ResultDate = x.ResultDate,
                RationaleGuidance = x.RationaleGuidance,
                ScoreType = x.ScoreType,
                ImprovementBasedDataPoint = x.ImprovementBasedDataPoint,
                DataSetCategoryID = x.DataSetCategoryID.GetValueOrDefault()
            }).FirstOrDefault();
        }

        private void MapModelToEntity(SGODataPoint model, SGODataPointEntity entity)
        {
            entity.Name = model.Name;
            entity.SubjectName = model.SubjectName;
            entity.GradeID = model.GradeID;
            entity.VirtualTestID = model.VirtualTestID;
            entity.AttachScoreUrl = model.AttachScoreUrl;
            entity.SGOID = model.SGOID;
            entity.Weight = model.Weight;
            entity.TotalPoints = model.TotalPoints;
            entity.Type = model.Type;
            entity.AchievementLevelSettingID = model.AchievementLevelSettingID;
            entity.ResultDate = model.ResultDate;
            entity.RationaleGuidance = model.RationaleGuidance;
            entity.ScoreType = model.ScoreType;
            entity.ImprovementBasedDataPoint = model.ImprovementBasedDataPoint;
            entity.IsTemporary = model.IsTemporary;
        }
    }
}
