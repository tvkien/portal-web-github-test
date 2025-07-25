using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirutalQuestionSubRepository : IRepository<VirtualQuestionSub>
    {
        private readonly Table<VirtualQuestionSubEntity> table;

        public VirutalQuestionSubRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<VirtualQuestionSubEntity>();
        }

        public IQueryable<VirtualQuestionSub> Select()
        {
            return table.Select(x => new VirtualQuestionSub
                {
                    PointsPossible = string.IsNullOrEmpty(x.PointsPossible) ? 0 : Convert.ToInt32(x.PointsPossible),
                    QTIItemSubId = x.QTIItemSubID,
                    VirtualQuestionId = x.VirtualQuestionID,
                    VirtualQuestionSubId = x.VirtualQuestionSubID
                });
        }

        public void Save(VirtualQuestionSub item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualQuestionSubID.Equals(item.VirtualQuestionSubId));

            if (entity == null)
            {
                entity = new VirtualQuestionSubEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(entity, item);
            table.Context.SubmitChanges();
            item.VirtualQuestionSubId = entity.VirtualQuestionSubID;
        }

        public void Delete(VirtualQuestionSub item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualQuestionSubID.Equals(item.VirtualQuestionSubId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(VirtualQuestionSubEntity entity, VirtualQuestionSub item)
        {
            entity.PointsPossible = item.PointsPossible.ToString();
            entity.QTIItemSubID = item.QTIItemSubId;
            entity.VirtualQuestionID = item.VirtualQuestionId;
            entity.VirtualQuestionSubID = item.VirtualQuestionSubId;
        }
    }
}