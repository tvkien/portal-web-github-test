using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TestFeedbackService
    {
        private readonly IRepository<TestFeedback> repository;

        public TestFeedbackService(IRepository<TestFeedback> repository)
        {
            this.repository = repository;
        }
        public IQueryable<TestFeedback> GetAll()
        {
            return repository.Select();
        }
        public void Save(TestFeedback item)
        {
            repository.Save(item);
        }
        public void Delete(TestFeedback item)
        {
            repository.Delete(item);
        }
        public TestFeedback GetLasFeedback(int qtiOnlineTestSessionId)
        {
            return repository.Select().Where(x => x.QtiOnlineTestSessionID == qtiOnlineTestSessionId).OrderBy(x => x.TestFeedbackID).FirstOrDefault();
        }

        public TestFeedback GetTestFeedbackById(int testFeedbackId)
        {
            return repository.Select().FirstOrDefault(x => x.TestFeedbackID == testFeedbackId);
        }
    }
}