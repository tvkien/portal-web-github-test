using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class UserDistrictSectorService
    {
        private readonly IReadOnlyRepository<UserDistrictSector> _repository;

        public UserDistrictSectorService(IReadOnlyRepository<UserDistrictSector> repository)
        {
            _repository = repository;
        }

        public UserDistrictSector GetByCodeAndSector(string code, string sector)
        {
            return
                _repository.Select().FirstOrDefault(
                        o =>o.Code.ToLower() == code.ToLower() && o.Sector.ToLower() == sector.ToLower());
        }

        public UserDistrictSector GetByEmailAndSector(string email, string sector)
        {
            return
                _repository.Select().FirstOrDefault(
                        o => o.Email.ToLower() == email.ToLower() && o.Sector.ToLower() == sector.ToLower());
        }

        public UserDistrictSector GetSectorByCode(string sectorCode)
        {
            return _repository.Select().FirstOrDefault(o => o.Sector.ToLower() == sectorCode.ToLower());
        }
    }
}
