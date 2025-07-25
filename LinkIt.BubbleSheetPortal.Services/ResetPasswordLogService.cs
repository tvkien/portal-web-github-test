using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ResetPasswordLogService
    {
        private readonly IResetPasswordLogRepository _repository;

        public ResetPasswordLogService(IResetPasswordLogRepository repository)
        {
            _repository = repository;
        }
     
        public IQueryable<ResetPasswordLog> GetResetPasswordLogs()
        {
            return _repository.Select();
        }

        public void Save(ResetPasswordLog item)
        {
            _repository.Save(item);
        }
    }
}
