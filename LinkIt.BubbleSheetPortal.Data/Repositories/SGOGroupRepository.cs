using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGOGroupRepository : IRepository<SGOGroup>
    {
        private readonly Table<SGOGroupEntity> table;

        public SGOGroupRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = SGODataContext.Get(connectionString).GetTable<SGOGroupEntity>();
        }

        public IQueryable<SGOGroup> Select()
        {
            return table.Select(x => new SGOGroup
                                     {
                                         SGOID = x.SGOID,
                                         Name = x.Name,
                                         Order = x.Order,
                                         SGOGroupID = x.SGOGroupID,
                                         TargetScore = x.TargetScore,
                                         PercentStudentAtTargetScore = x.PercentStudentAtTargetScore,
                                         TeacherSGOScore = x.TeacherSGOScore,
                                         Weight = x.Weight,
                                         TargetScoreCustom = x.TargetScoreCustom,
                                         TeacherSGOScoreCustom = x.TeacherSGOScoreCustom
                                     });
        }

        public void Save(SGOGroup item)
        {
            var entity = table.FirstOrDefault(x => x.SGOGroupID.Equals(item.SGOGroupID));

            if (entity == null)
            {
                entity = new SGOGroupEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);

            table.Context.SubmitChanges();
            item.SGOGroupID = entity.SGOGroupID;
        }

        public void Delete(SGOGroup item)
        {
            var entity = table.FirstOrDefault(x => x.SGOGroupID.Equals(item.SGOGroupID));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(SGOGroup model, SGOGroupEntity entity)
        {
            entity.SGOID = model.SGOID;
            entity.Order = model.Order;
            entity.Name = model.Name;
            entity.TargetScore = model.TargetScore;
            entity.PercentStudentAtTargetScore = model.PercentStudentAtTargetScore;
            entity.TeacherSGOScore = model.TeacherSGOScore;
            entity.Weight = model.Weight;
            entity.TargetScoreCustom = model.TargetScoreCustom;
            entity.TeacherSGOScoreCustom = model.TeacherSGOScoreCustom;
        }
    }
}