using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualTestTimingOptionService
    {
         private readonly IReadOnlyRepository<VirtualTestTimingOption> _repository;

         public VirtualTestTimingOptionService(IReadOnlyRepository<VirtualTestTimingOption> repository)
         {
             _repository = repository;
         }

        public List<VirtualTestTimingOption> GetTimingOptionsByDistrictId(int districtid)
        {
            return _repository.Select().Where(o => o.DistrictID == districtid).ToList();
        }
    }
}
