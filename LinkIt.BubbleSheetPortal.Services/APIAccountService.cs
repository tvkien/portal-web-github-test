using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.EDM;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class APIAccountService
    {
        private readonly IReadOnlyRepository<APIAccount> repository;

        public APIAccountService(IReadOnlyRepository<APIAccount> repository)
        {
            this.repository = repository;
        }

        public APIAccount GetAPIAccountByClientAccessKey(string clientAccessKey)
        {
            return repository.Select().FirstOrDefault(x => x.ClientAccessKeyID.Equals(clientAccessKey) && x.Status == 1);
        }

        public APIAccount GetAPIAccountByDistrictId(int districtId)
        {
            return repository.Select().FirstOrDefault(x => x.TargetID == districtId && x.APIAccountTypeID == 1 && x.Status == 1);
        }

        public APIAccount GetAPIAccountById(int id)
        {
            return repository.Select().FirstOrDefault(x => x.APIAccountID == id && x.Status == 1);
        }
    }
}
