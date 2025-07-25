using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SGOAttainmentGroupService
    {
        private readonly IRepository<SGOAttainmentGroup> sGOAttainmentGroupRepository;

        public SGOAttainmentGroupService(IRepository<SGOAttainmentGroup> sGOAttainmentGroupRepository)
        {
            this.sGOAttainmentGroupRepository = sGOAttainmentGroupRepository;            
        }

        public IQueryable<SGOAttainmentGroup> GetBySgoGroupIds(List<int> sgoGroupIds)
        {
            return sGOAttainmentGroupRepository.Select().Where(x => sgoGroupIds.Contains(x.SGOGroupId));
        }

        public IQueryable<SGOAttainmentGroup> GetBySgoAttainmentGoalId(int sgoAttainmentGoalId)
        {
            return sGOAttainmentGroupRepository.Select().Where(x => x.SGOAttainmentGoalId == sgoAttainmentGoalId);
        }

        public SGOAttainmentGroup GetBySgoAttainmentGroupId(int sgoAttainmentGroupId)
        {
            return sGOAttainmentGroupRepository.Select().FirstOrDefault(x => x.SGOAttainmentGroupId == sgoAttainmentGroupId);
        }

        public void Save(SGOAttainmentGroup item)
        {
            sGOAttainmentGroupRepository.Save(item);
        }

        public void Delete(SGOAttainmentGroup item)
        {
            sGOAttainmentGroupRepository.Delete(item);
        }
    }
}