using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DelegateTeacherService
    {
        private readonly IRepository<DelegateTeacher> repository;

        public DelegateTeacherService(IRepository<DelegateTeacher> repository)
        {
            this.repository = repository;
        }

        public void Save(DelegateTeacher newDelegate)
        {
            repository.Save(newDelegate);
        }

        public void Delete(DelegateTeacher deletedDelegate)
        {
            repository.Delete(deletedDelegate);
        }

        public IQueryable<DelegateTeacher> GetAllDelegateByUserId(int userId)
        {
            return repository.Select().Where(d => d.UserID.Equals(userId));
        }
    }
}