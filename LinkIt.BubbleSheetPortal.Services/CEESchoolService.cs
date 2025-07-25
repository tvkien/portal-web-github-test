using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class CEESchoolService
    {
        private readonly IRepository<CEESchool> repository;

        public CEESchoolService(IRepository<CEESchool> repository)
        {
            this.repository = repository;
        }

        public CEESchool GetSchoolById(int cEESchoolId)
        {
            return repository.Select().FirstOrDefault(x => x.CEESchoolId.Equals(cEESchoolId));
        }

        public void Save(CEESchool item)
        {
            repository.Save(item);
        }

        public void Delete(CEESchool item)
        {
            repository.Delete(item);
        }

        public IQueryable<CEESchool> Select()
        {
            return repository.Select();
        }

        public IQueryable<CEESchool> GetSchoolsByLocationCode(string locationCode)
        {
            return repository.Select().Where(x => x.LocationCode == locationCode);
        }
    }
}
