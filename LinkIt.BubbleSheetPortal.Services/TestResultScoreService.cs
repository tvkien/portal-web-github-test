using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TestResultScoreService
    {
        private readonly IReadOnlyRepository<TestResultScore> _repository;

        public TestResultScoreService(IReadOnlyRepository<TestResultScore> repository)
        {
            this._repository = repository;
        }

        public IQueryable<TestResultScore> GetAll()
        {
            return _repository.Select();
        }        
    }
}
