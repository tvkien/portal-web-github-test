using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualSectionQuestionRepository : IVirtualSectionQuestionRepository
    {
        private readonly TestDataContext _testDataContext;

        private readonly Table<VirtualSectionQuestionEntity> _tableVirtualSectionQuestionEntity;
        private readonly Table<VirtualSectionQuestionView> view;

        public VirtualSectionQuestionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _testDataContext = TestDataContext.Get(connectionString);
            _tableVirtualSectionQuestionEntity = _testDataContext.GetTable<VirtualSectionQuestionEntity>();
            view = _testDataContext.GetTable<VirtualSectionQuestionView>();
        }

        public IQueryable<VirtualSectionQuestion> Select()
        {
            return view.Select(
                x => new VirtualSectionQuestion
                         {
                             Order = x.Order,
                             VirtualQuestionId = x.VirtualQuestionID,
                             VirtualSectionId = x.VirtualSectionID,
                             VirtualSectionQuestionId = x.VirtualSectionQuestionID,
                             QuestionOrder = x.QuestionOrder,
                             VirtualTestId = x.VirtualTestID,
                             PointsPossible = x.PointsPossible,
                             QtiItemId = x.QTIItemID??0,
                             XmlContent = x.XmlContent
                         }
                );
        }

        public void Save(VirtualSectionQuestion item)
        {
            var entity = _tableVirtualSectionQuestionEntity.FirstOrDefault(x => x.VirtualSectionQuestionID.Equals(item.VirtualSectionQuestionId));

            if (entity == null)
            {
                entity = new VirtualSectionQuestionEntity();
                _tableVirtualSectionQuestionEntity.InsertOnSubmit(entity);
            }

            MapModelToEntity(entity, item);
            _tableVirtualSectionQuestionEntity.Context.SubmitChanges();
            item.VirtualSectionId = entity.VirtualSectionID;
        }

        public void Delete(VirtualSectionQuestion item)
        {
            var entity = _tableVirtualSectionQuestionEntity.FirstOrDefault(x => x.VirtualSectionQuestionID.Equals(item.VirtualSectionQuestionId));

            if (entity != null)
            {
                _tableVirtualSectionQuestionEntity.DeleteOnSubmit(entity);
                _tableVirtualSectionQuestionEntity.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(VirtualSectionQuestionEntity entity, VirtualSectionQuestion item)
        {
            entity.Order = item.Order;
            entity.VirtualQuestionID = item.VirtualQuestionId;
            entity.VirtualSectionID = item.VirtualSectionId;
            entity.VirtualSectionQuestionID = item.VirtualSectionQuestionId;
        }

        public void InsertMultipleRecord(List<VirtualSectionQuestion> items)
        {
            foreach (var item in items)
            {
                var entity = new VirtualSectionQuestionEntity
                {
                    Order = item.Order,
                    VirtualQuestionID = item.VirtualQuestionId,
                    VirtualSectionID = item.VirtualSectionId,
                    VirtualSectionQuestionID = item.VirtualSectionQuestionId
                };

                _tableVirtualSectionQuestionEntity.InsertOnSubmit(entity);
            }

            _tableVirtualSectionQuestionEntity.Context.SubmitChanges();
        }
    }
}
