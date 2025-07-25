using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SchoolStudentDataService
    {
        private readonly IRepository<SchoolStudentData> schoolStudentDataRepository;

        public SchoolStudentDataService(IRepository<SchoolStudentData> repository)
        {
            schoolStudentDataRepository = repository;              
        }

        public void Save(SchoolStudentData item)
        {
            schoolStudentDataRepository.Save(item);
        }
    }
}