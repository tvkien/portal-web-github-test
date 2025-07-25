using System;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.QuestionGroup;
using System.Data.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualQuestionGroupRepository : IRepository<VirtualQuestionGroup>
    {
        private readonly Table<VirtualQuestionGroupEntity> _tableVirtualQuestionGroupEntity;

        public VirtualQuestionGroupRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _tableVirtualQuestionGroupEntity = TestDataContext.Get(connectionString).GetTable<VirtualQuestionGroupEntity>();
        }

        public void Delete(VirtualQuestionGroup item)
        {
            var entity = _tableVirtualQuestionGroupEntity.FirstOrDefault(x => x.VirtualQuestionGroupID == item.VirtualQuestionGroupID);

            if (entity != null)
            {
                _tableVirtualQuestionGroupEntity.DeleteOnSubmit(entity);
                _tableVirtualQuestionGroupEntity.Context.SubmitChanges();
            }
        }

        public void Save(VirtualQuestionGroup item)
        {
            var entity = _tableVirtualQuestionGroupEntity.FirstOrDefault(x => x.VirtualQuestionGroupID == item.VirtualQuestionGroupID);

            if (entity == null)
            {
                entity = new VirtualQuestionGroupEntity();
                _tableVirtualQuestionGroupEntity.InsertOnSubmit(entity);
            }
            entity.Order = item.Order;
            entity.QuestionGroupID = item.QuestionGroupID;
            entity.VirtualQuestionID = item.VirtualQuestionID;            

            _tableVirtualQuestionGroupEntity.Context.SubmitChanges();
            item.VirtualQuestionGroupID = entity.VirtualQuestionGroupID;
        }

        public IQueryable<VirtualQuestionGroup> Select()
        {
            return _tableVirtualQuestionGroupEntity.Select(
               x => new VirtualQuestionGroup
               {
                   Order = x.Order,
                   QuestionGroupID = x.QuestionGroupID,
                   VirtualQuestionID = x.VirtualQuestionID,
                   VirtualQuestionGroupID = x.VirtualQuestionGroupID
               });
        }
    }
}
