using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class RosterTypeService
    {
        private readonly IRepository<RosterType> _rosterTypeRepository;

        public RosterTypeService(IRepository<RosterType> rosterTypeRepository)
        {
            _rosterTypeRepository = rosterTypeRepository;
        }
        
        public IQueryable<RosterType> Select()
        {
            return _rosterTypeRepository.Select();
        }

        public void Save(RosterType item)
        {
            _rosterTypeRepository.Save(item);
        }

        public void Delete(RosterType item)
        {
            _rosterTypeRepository.Delete(item);
        }        
    }
}