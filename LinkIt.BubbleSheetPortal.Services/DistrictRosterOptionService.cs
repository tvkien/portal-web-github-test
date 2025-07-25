using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DistrictRosterOptionService
    {
        private readonly IRepository<DistrictRosterOption> _districtRosterOptionRepository;

        public DistrictRosterOptionService(IRepository<DistrictRosterOption> districtRosterOptionRepository)
        {
            _districtRosterOptionRepository = districtRosterOptionRepository;
        }
        
        public IQueryable<DistrictRosterOption> Select()
        {
            return _districtRosterOptionRepository.Select();
        }

        public void Save(DistrictRosterOption item)
        {
            _districtRosterOptionRepository.Save(item);
        }

        public void Delete(DistrictRosterOption item)
        {
            _districtRosterOptionRepository.Delete(item);
        }        
    }
}