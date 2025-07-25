using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SGOAttainmentGoalService
    {
        private readonly IRepository<SGOAttainmentGoal> sGOAttainmentGoalRepository;

        public SGOAttainmentGoalService(IRepository<SGOAttainmentGoal> sGOAttainmentGoalRepository)
        {
            this.sGOAttainmentGoalRepository = sGOAttainmentGoalRepository;            
        }

        public IQueryable<SGOAttainmentGoal> GetBySgoId(int sgoId)
        {
            return sGOAttainmentGoalRepository.Select().Where(x => x.SGOId == sgoId);
        }

        public SGOAttainmentGoal GetById(int sgoAttainmentGoalId)
        {
            return sGOAttainmentGoalRepository.Select().FirstOrDefault(x => x.SGOAttainmentGoalId == sgoAttainmentGoalId);
        }

        public void Save(SGOAttainmentGoal item)
        {
            sGOAttainmentGoalRepository.Save(item);
        }        
    }
}