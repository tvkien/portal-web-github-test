using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.QuestionGroup;
using System;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QuestionGroupRepository : IRepository<QuestionGroup>
    {
        private readonly Table<QuestionGroupEntity> _tableQuestionGroupEntity;

        public QuestionGroupRepository(IConnectionString conn)
        {            
            var connectionString = conn.GetLinkItConnectionString();
            _tableQuestionGroupEntity = TestDataContext.Get(connectionString).GetTable<QuestionGroupEntity>();
        }

        public void Delete(QuestionGroup item)
        {
            var entity = _tableQuestionGroupEntity.FirstOrDefault(x => x.QuestionGroupID == item.QuestionGroupID);

            if (entity != null)
            {
                _tableQuestionGroupEntity.DeleteOnSubmit(entity);
                _tableQuestionGroupEntity.Context.SubmitChanges();
            }
        }

        public void Save(QuestionGroup item)
        {
            var entity = _tableQuestionGroupEntity.FirstOrDefault(x => x.QuestionGroupID == item.QuestionGroupID);

            if (entity == null)
            {
                entity = new QuestionGroupEntity();
                _tableQuestionGroupEntity.InsertOnSubmit(entity);
            }
            entity.VirtualTestID = item.VirtualTestId;
            entity.VirtualSectionID = item.VirtualSectionID;
            entity.XmlContent = item.XmlContent;
            entity.Order = item.Order;
            entity.DisplayPosition = item.DisplayPosition;
            entity.Title = item.Title;

            _tableQuestionGroupEntity.Context.SubmitChanges();
            item.QuestionGroupID = entity.QuestionGroupID;
        }

        public IQueryable<QuestionGroup> Select()
        {
            return _tableQuestionGroupEntity.Select(
                x => new QuestionGroup
                {
                    Order = x.Order,
                    QuestionGroupID = x.QuestionGroupID,
                    VirtualSectionID = x.VirtualSectionID,
                    VirtualTestId = x.VirtualTestID,
                    XmlContent = x.XmlContent,
                    DisplayPosition = x.DisplayPosition.GetValueOrDefault(),
                    Title = x.Title ?? string.Empty
                });
        }
    }
}
