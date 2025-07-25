using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Service;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TeacherService : PersistableModelService<Teacher>
    {
        public TeacherService(IRepository<Teacher> repository) : base(repository)
        {
        }

        public Teacher GetTeacherByTeacherId(int teacherId)
        {
            return repository.Select().FirstOrDefault(x => x.Id.Equals(teacherId));
        }

        public Teacher GetTeacherByUserId(int userId)
        {
            return repository.Select().FirstOrDefault(x => x.UserId.Equals(userId));
        }
    }
}