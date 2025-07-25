using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ItemFeedbackService
    {
        private readonly IRepository<ItemFeedback> repository;

        public ItemFeedbackService(IRepository<ItemFeedback> repository)
        {
            this.repository = repository;
        }
        public IQueryable<ItemFeedback> GetAll ()
        {
            return repository.Select();
        }
        public void Save(ItemFeedback item)
        {
            repository.Save(item);
        }
        public void Delete(ItemFeedback item)
        {
            repository.Delete(item);
        }

        public ItemFeedback GetItemFeedbackById(int itemFeedbackId)
        {
            return repository.Select().FirstOrDefault(x=>x.ItemFeedbackID == itemFeedbackId);
        }
        public ItemFeedback GetFeedbackOfOnlineSessionAnswer(int qtiOnlineTestSessionAnswerId)
        {
            return repository.Select().Where(x => x.QTIOnlineTestSessionAnswerID == qtiOnlineTestSessionAnswerId).OrderBy(x => x.ItemFeedbackID).FirstOrDefault();
        }
        public ItemFeedback GetFeedbackOfAnswer(int answerId)
        {
            return repository.Select().Where(x => x.AnswerID == answerId).OrderBy(x => x.ItemFeedbackID).FirstOrDefault();
        }

    }
}