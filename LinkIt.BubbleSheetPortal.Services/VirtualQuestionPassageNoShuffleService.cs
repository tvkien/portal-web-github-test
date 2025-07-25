using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualQuestionPassageNoShuffleService
    {
        private readonly IVirtualQuestionPassageNoShuffleRepository repository;

        public VirtualQuestionPassageNoShuffleService(IVirtualQuestionPassageNoShuffleRepository repository)
        {
            this.repository = repository;
        }
        public IQueryable<VirtualQuestionPassageNoShuffle> Select()
        {
            return repository.Select();
        }
        public void Save(VirtualQuestionPassageNoShuffle newItem)
        {
            repository.Save(newItem);
        }

        public void DeleteAllPassageNoshuffle(int virtualquestionId)
        {
            repository.DeleteAllPassageNoshuffle(virtualquestionId);
        }
    }
}