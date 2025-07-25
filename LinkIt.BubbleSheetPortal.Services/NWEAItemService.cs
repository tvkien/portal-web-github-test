using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class NWEAItemService
    {
        private readonly IReadOnlyRepository<NWEAItem> repository;

        public NWEAItemService(IReadOnlyRepository<NWEAItem> repository)
        {
            this.repository = repository;
        }

        public NWEAItem GetNweaItemById(int id)
        {
            return repository.Select().FirstOrDefault(o => o.QTI3pItemID == id);
        }

        public IQueryable<NWEAItem> GetNweaItems ()
        {
            return repository.Select();
        }
    }
}
