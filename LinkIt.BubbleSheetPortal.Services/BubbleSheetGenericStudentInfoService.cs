using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleService.Models.Test;
using LinkIt.BubbleService.Services;
using LinkIt.BubbleService.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class BubbleSheetGenericStudentInfoService
    {
        private readonly IReadOnlyRepository<BubbleSheetGenericStudentInfo> repository;

        public BubbleSheetGenericStudentInfoService(IReadOnlyRepository<BubbleSheetGenericStudentInfo> repository)
        {
            this.repository = repository;
        }

        public IQueryable<BubbleSheetGenericStudentInfo> Select()
        {
            return repository.Select();
        }
    }
}